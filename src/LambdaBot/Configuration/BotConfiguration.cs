using System;

namespace LambdaBot.Configuration;

internal class BotConfiguration
{
    public Uri PirateApiBaseUri { get; set; }
    public string BotToken { get; init; }
}