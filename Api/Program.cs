using Api.Mapping;
using Api.Swagger;
using Api.Utils;
using Application.Services;
using dotenv.net;
using EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;


#region EnvVars
DotEnv.Load();
string dbConnectionString = EnvVarUtils.TryGetEnvVar("DATABASE_CONNECTION_STRING");
#endregion


var builder = WebApplication.CreateBuilder(args);


#region AddServices
builder.Services.AddDbContext<UniversityDbContext>(options =>
    options.UseNpgsql(dbConnectionString)
);
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<MappingProfile>();
});

builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<AdministratorService>();


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
