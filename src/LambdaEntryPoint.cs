using Amazon.Lambda.AspNetCoreServer;

namespace StoreApp.Api;

/// <summary>
/// Entry point for AWS Lambda. The function handler string in aws-lambda-tools-defaults.json
/// must point to: StoreApp.Api::StoreApp.Api.LambdaEntryPoint::FunctionHandlerAsync
/// </summary>
public class LambdaEntryPoint : APIGatewayProxyFunction
{
    protected override void Init(IWebHostBuilder builder)
    {
        builder.UseStartup<Startup>();
    }

    protected override void Init(IHostBuilder builder) { }
}
