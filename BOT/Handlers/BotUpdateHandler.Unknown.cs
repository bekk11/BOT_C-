using Telegram.Bot;
using Telegram.Bot.Types;

namespace BOT.Handlers;

public partial class BotUpdateHandler
{
    private Task HandleUnknownUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _logger.LogWarning($"Unknown update type: {update.Type}");
        return Task.CompletedTask;
    }
}