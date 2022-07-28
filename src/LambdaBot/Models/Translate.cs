namespace LambdaBot.Models;

internal class TranslateRequest
{
    public string Text { get; init; }
}

public class TranslateContent
{
    public string Translated { get; init; }
}

public class TranslateResponse
{
    public TranslateContent Contents { get; init; }
}