// <auto-generated/>

using System.Reflection;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RecSys.Api.Infrastructure;
using RecSys.Api.Jobs;
using RecSys.Customs.Client;
using RecSys.ML.Client;
using RecSys.Platform.Data.Extensions;
using RecSys.Platform.Data.FluentMigrator;
using RecSys.Platform.Extensions;
using RecSys.Platform.Middlewares;

var builder = WebApplication
    .CreateBuilder(args)
    .UsePlatform();
var services = builder.Services;
var configuration = builder.Configuration;

#region DI

services.AddHttpClient(nameof(CustomsClient), client => client.BaseAddress = new Uri("http://stat.customs.gov.ru/"));
services.AddHttpClient(nameof(MlClient), client => client.BaseAddress = new Uri("http://localhost:8000/"));
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwagger("rec-sys-api", useJwtAuth: true);
services.AddSerilogLogger();
services.AddPostgres();
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});
services.AddAuthorization();
services.AddScoped<CustomsClient>();
services.AddScoped<MlClient>();
services.AddSingleton<CustomsDataCollectingProcessor>();
services.AddSingleton<DataProcessingProcessor>();
services.AddHostedService<MainHostedService>();
services.AddMigrator(typeof(Program).Assembly);
services.AddMediatR(typeof(Program));

#endregion

var app = builder.Build();

#region App

ExceptionMiddleware.ReturnStackTrace = false;
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(
    x =>
    {
        x.AllowAnyHeader();
        x.AllowAnyMethod();
        x.AllowAnyOrigin();
    });
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

#endregion

await app.RunOrMigrateAsync(args);
