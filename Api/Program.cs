using System.Text;
using Api.Mapping;
using Api.OpenApi;
using Api.Swagger;
using Api.Utils;
using Application.Configurations;
using Application.Objects.Configurations;
using Application.Services;
using Application.Services.Auth;
using Application.Services.Users;
using dotenv.net;
using EFCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

#region EnvVars

DotEnv.Load();
var dbConnectionString = EnvVarUtils.TryGetEnvVar("DATABASE_CONNECTION_STRING");

#endregion


var jwtConfig = builder.Configuration.GetSection("JwtSettings").Get<JwtConfiguration>();
var refreshTokenConfig = builder.Configuration.GetSection("RefreshTokenSettings").Get<RefreshTokenConfiguration>();

if (jwtConfig is null || refreshTokenConfig is null)
    throw new InvalidOperationException("Missing required environment variables.");


#region AddServices

builder.Services.AddTransient<JwtTokenService>();
builder.Services.AddTransient<RefreshTokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IAuthUserService, AuthUserService>();

builder.Services.AddDbContext<UniversityDbContext>(options =>
    options.UseNpgsql(dbConnectionString)
);
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); });

builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<AdministratorService>();
builder.Services.AddScoped<CourseService>();


builder.Services.AddControllers();

#endregion


#region SecuritySetup

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddCors(options =>
{
    var frontendUrl = builder.Configuration.GetValue<string>("Frontend:Url") ?? "http://localhost:5173";
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins(frontendUrl)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddSingleton(jwtConfig);
builder.Services.AddSingleton(refreshTokenConfig);

#endregion


#region SwaggerSetup

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureOptions>();

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
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