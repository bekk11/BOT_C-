using System.Globalization;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BOT.Handlers;

public partial class BotUpdateHandler
{
    private async Task HandleMessageAsync(ITelegramBotClient botClient, Message? message, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);

        var from = message.From;
        _logger.LogInformation("Received a message from {from?.FirstName}", from?.FirstName);

        var handler = message.Type switch
        {
            MessageType.Text => HandleTextMessageAsync(botClient, message, cancellationToken),
            _ => HandleUnsupportedMessageAsync(botClient, message, cancellationToken)
        };

        await handler;
    }

    private async Task HandleTextMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        if (message.Text!.Contains("uz", StringComparison.CurrentCultureIgnoreCase))
        {
            var res = await _userService.UpdateLanguageCodeAsync(message.From?.Id, "uz-Uz");
        } else if (message.Text!.Contains("en", StringComparison.CurrentCultureIgnoreCase))
        {
            var res = await _userService.UpdateLanguageCodeAsync(message.From?.Id, "en-US");
        } else if (message.Text!.Contains("ru", StringComparison.CurrentCultureIgnoreCase))
        {
            var res = await _userService.UpdateLanguageCodeAsync(message.From?.Id, "ru-Ru");
        }
        
        var from = message.From;
        _logger.LogInformation("Received a text message from {from?.FirstName}", from?.FirstName);
        
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: _localizer["greeting"],
            cancellationToken: cancellationToken
        );
    }

    private Task HandleUnsupportedMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received an unsupported message type: {message.Type}", message.Type);

        return Task.CompletedTask;
    }
}