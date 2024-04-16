using Telegram.Bot;
using Telegram.Bot.Types;

namespace BOT.Handlers;

public partial class BotUpdateHandler
{
    private async Task HandleEditedMessageAsync(ITelegramBotClient botClient, Message? message, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);

        var from = message.From;
        
        _logger.LogInformation("Received a edited message from {from?.FirstName}: {message.Text}", from?.FirstName, message.Text);
    }
}