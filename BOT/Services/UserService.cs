using System.Globalization;
using BOT.Data;
using BOT.Entities;
using Microsoft.EntityFrameworkCore;

namespace BOT.Services;

public class UserService(ILogger<UserService> _logger, BotDbContext _dbContext)
{
    public async Task<(bool IsSuccess, string? ErrorMessage)> AddUserAsync(MyUser user)
    {
        if (await Exists(user.UserId))
        {
            return (false, "User already exists");
        }
        
        try
        {
            await _dbContext.Users!.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
        
    }
    public async Task<MyUser?> GetUserAsync(long? userId)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(_dbContext.Users);

        return await _dbContext.Users.FindAsync(userId);
    }
    
    public async Task<string?> GetMyUserLanguageCodeAsync(long userId)
    {
        var user = await GetUserAsync(userId);

        return user?.LanguageCode;
    }

    public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateLanguageCodeAsync(long? userId, string? languageCode)
    {
        ArgumentNullException.ThrowIfNull(languageCode);
        
        var user = await GetUserAsync(userId);

        if (user is null)
        {
            return (false, "User not found");
        }
        
        user.LanguageCode = languageCode;
        _dbContext.Users?.Update(user);
        await _dbContext.SaveChangesAsync();

        var culture = new CultureInfo(languageCode);
        
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        return (true, null);
    }
    
    public async Task<bool> Exists(long userId) => await _dbContext.Users!.AnyAsync(b => b.UserId == userId);
}