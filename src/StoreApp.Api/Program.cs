using StoreApp.Api.Extensions;

namespace StoreApp.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddServices(builder.Configuration);
        var app = builder.Build();

        var enableSwagger = app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true";

        if (enableSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();
        app.Run();
    }
}
