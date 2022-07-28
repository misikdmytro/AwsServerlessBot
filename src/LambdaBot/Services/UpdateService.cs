using System;
using System.Threading;
using System.Threading.Tasks;

using LambdaBot.Clients;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace LambdaBot.Services;

public interface IUpdateService
{
    Task HandleUpdate(Update update, CancellationToken cancellationToken = default);
}

internal class UpdateService : IUpdateService
{
    private const string WelcomeMessage = "Welcome to Pirate Translator Bot! To translate some text just type it here!";
    private const string FailedToTranslateMessage = @"Cannot transalte right now, please try later";

    private readonly ITelegramBotClient _botClient;
    private readonly IPirateTranslatorClient _translatorClient;

    private readonly ILogger<UpdateService> _logger;

    public UpdateService(ITelegramBotClient botClient,
        IPirateTranslatorClient translatorClient,
        ILogger<UpdateService> logger)
    {
        _botClient = botClient;
        _translatorClient = translatorClient;
        _logger = logger;
    }

    public async Task HandleUpdate(Update update, CancellationToken cancellationToken = default)
    {
        var message = update?.Message?.Text;
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        try
        {
            if (message.Equals("/start") == true)
            {
                await _botClient.SendTextMessageAsync(update.Message.Chat.Id,
                    WelcomeMessage,
                    cancellationToken: cancellationToken);

                return;
            }

            try
            {
                var response = await _translatorClient.Translate(message, cancellationToken);
                var translated = response?.Contents?.Translated;

                if (string.IsNullOrEmpty(translated))
                {
                    throw new Exception("Translated content is null or empty");
                }

                await _botClient.SendTextMessageAsync(update.Message.Chat.Id,
                    translated,
                    replyToMessageId: update.Message.MessageId,
                    cancellationToken: cancellationToken);
            }
            catch (Exception)
            {
                await _botClient.SendTextMessageAsync(update.Message.Chat.Id,
                    FailedToTranslateMessage,
                    replyToMessageId: update.Message.MessageId,
                    cancellationToken: cancellationToken);

                throw;
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Unknown error", new
            {
                Message = message
            });
        }
    }
}