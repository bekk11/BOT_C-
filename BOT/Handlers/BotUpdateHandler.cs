using System.Globalization;
using BOT.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BOT.Handlers;

partial class BotUpdateHandler(ILogger<BotUpdateHandler> _logger, IServiceScopeFactory _scopeFactory) : IUpdateHandler
{
    private IStringLocalizer _localizer = null!;
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // SetCultureForUser();
        var culture = new CultureInfo("en-Us");
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        
        using var scope = _scopeFactory.CreateScope();
        _localizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer<BotLocalizer>>();
        
        var result = update.Type switch
        {
            UpdateType.Message => HandleMessageAsync(botClient, update.Message, cancellationToken),
            UpdateType.EditedMessage => HandleEditedMessageAsync(botClient, update.EditedMessage, cancellationToken),
            
            _ => HandleUnknownUpdateAsync(botClient, update, cancellationToken)
        };

        try
        {
            await result;
        }
        catch (Exception e)
        {
            await HandlePollingErrorAsync(botClient, e, cancellationToken);
        }
    }
    

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An error occurred while polling updates");
        return Task.CompletedTask;
    }
}