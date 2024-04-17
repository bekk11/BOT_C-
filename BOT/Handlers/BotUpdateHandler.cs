using System.Globalization;
using BOT.Entities;
using BOT.Resources;
using BOT.Services;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BOT.Handlers;

partial class BotUpdateHandler(ILogger<BotUpdateHandler> _logger, IServiceScopeFactory _scopeFactory) : IUpdateHandler
{
    private IStringLocalizer _localizer = null!;
    private UserService _userService = null!;
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        _userService = scope.ServiceProvider.GetRequiredService<UserService>();
        _localizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer<BotLocalizer>>();
        
        var culture = await GetCultureForUser(update);
        
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        
        
        
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

    private async Task<CultureInfo> GetCultureForUser(Update update)
    {
        var from = update.Type switch
        {
            UpdateType.Message => update.Message?.From,
            UpdateType.EditedMessage => update.EditedMessage?.From,
            UpdateType.CallbackQuery => update.CallbackQuery?.From,
            UpdateType.InlineQuery => update.InlineQuery?.From,
            _ => update.Message?.From
        };

        var res = await _userService.AddUserAsync(new MyUser()
        {
            Firstname = from.FirstName,
            Lastname = from.LastName,
            UserId = from.Id,
            Username = from.Username,
            LanguageCode = from.LanguageCode,
            CreatedAt = DateTimeOffset.UtcNow,
            LastInteractionAt = DateTimeOffset.UtcNow
        });

        if (res.IsSuccess)
        {
            _logger.LogInformation("User added successfully");
        }
        else
        {
            _logger.LogError(res.ErrorMessage);
        }

        var lang = await _userService.GetMyUserLanguageCodeAsync(from!.Id);
        
        return new CultureInfo(lang ?? "uz-Uz");
    }
}