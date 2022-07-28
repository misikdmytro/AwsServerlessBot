using LambdaBot.Clients;
using LambdaBot.Configuration;
using LambdaBot.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

builder.Services.Configure<BotConfiguration>(builder.Configuration);
builder.Services.AddTransient<IUpdateService, UpdateService>();

builder.Services
    .AddHttpClient("tgwebhook")
    .AddTypedClient<ITelegramBotClient>((client, sp) =>
    {
        var configuration = sp.GetRequiredService<IOptionsMonitor<BotConfiguration>>();
        return new TelegramBotClient(configuration.CurrentValue.BotToken, client);
    });

builder.Services
    .AddHttpClient("Pirate API", (sp, client) =>
    {
        var configuration = sp.GetRequiredService<IOptionsMonitor<BotConfiguration>>();
        client.BaseAddress = configuration.CurrentValue.PirateApiBaseUri;
    })
    .AddTypedClient<IPirateTranslatorClient>((client, sp) =>
    {
        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

        var settings = new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.None
        };

        return new PirateTranslatorClient(client, settings);
    });

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();