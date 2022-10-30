// <auto-generated/>

using System.Reflection;
using MediatR;
using RecSys.Platform.Data.FluentMigrator;
using RecSys.Platform.Extensions;
using RecSys.Platform.Middlewares;

var builder = WebApplication
    .CreateBuilder(args)
    .UsePlatform();
var services = builder.Services;
var configuration = builder.Configuration;

#region DI

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwagger("rec-sys-api", useJwtAuth: true);
services.AddSerilogLogger();

services.AddMediatR(typeof(Program));

#endregion

var app = builder.Build();

#region App

ExceptionMiddleware.ReturnStackTrace = false;
app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

#endregion

await app.RunOrMigrateAsync(args);
