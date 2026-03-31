using Amazon.Lambda.AspNetCoreServer.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using StoreApp.Api.Data;
using StoreApp.Api.Mappings;
using StoreApp.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.CommandTimeout(30);
        npgsql.EnableRetryOnFailure(3);
    }));

builder.Services.AddAutoMapper(config => config.AddProfile<MappingProfile>());

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "StoreApp API", Version = "v1", Description = "API para tienda de fragancias" });
});

builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddLambdaHosting();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StoreApp API v1"));

app.UseCors();
app.UseMiddleware<ExceptionMiddleware>();
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
