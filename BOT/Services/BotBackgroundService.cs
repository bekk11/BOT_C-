using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BOT.Services;

public class BotBackgroundService(
    ILogger<BotBackgroundService> _logger,
    TelegramBotClient _botClient,
    IUpdateHandler _handler) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        User bot = await _botClient.GetMeAsync(cancellationToken: stoppingToken);

        _logger.LogInformation($"Bot started successfully: {bot.Username}");

        _botClient.StartReceiving(
            _handler.HandleUpdateAsync,
            _handler.HandlePollingErrorAsync,
            new ReceiverOptions
            {
                ThrowPendingUpdates = true 
            },
            cancellationToken: stoppingToken
        );
    }
}