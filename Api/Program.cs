using Api.Mapping;
using Api.Swagger;
using Api.Utils;
using Application.Objects.Configurations;
using Application.Services;
using dotenv.net;
using EFCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#region EnvVars
DotEnv.Load();
string dbConnectionString = EnvVarUtils.TryGetEnvVar("DATABASE_CONNECTION_STRING");
string jwtKey = EnvVarUtils.TryGetEnvVar("JWT_KEY");
#endregion


#region appsettings.json vars
var jwtConfig = builder.Configuration.GetSection("JwtSettings").Get<JwtConfiguration>();
var refreshTokenConfig = builder.Configuration.GetSection("RefreshTokenSettings").Get<RefreshTokenConfiguration>();
if (jwtConfig is null || refreshTokenConfig is null) {
    throw new InvalidOperationException("Missing required environment variables.");
}

jwtConfig = jwtConfig with { Key = jwtKey };
#endregion



#region AddServices
builder.Services.AddDbContext<UniversityDbContext>(options =>
    options.UseNpgsql(dbConnectionString)
);
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<MappingProfile>();
});

builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<AdministratorService>();
builder.Services.AddScoped<CourseService>();


builder.Services.AddControllers();
#endregion


#region SecuritySetup
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x => {
        x.TokenValidationParameters = new() {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key))
        };
    });


builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins(builder.Configuration.GetValue<string>("Frontend:Url"))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

builder.Services.AddSingleton(jwtConfig);
builder.Services.AddSingleton(refreshTokenConfig);
#endregion


#region SwaggerSetup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo {
        Version = "v1",
        Title = "API",
        Description = "REST API",
    });
});
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureOptions>();
#endregion


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
