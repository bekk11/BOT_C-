namespace BOT.Entities;

public class MyUser
{
    public long UserId { get; set; }
    public long ChatId { get; set; }
    public bool IsBot { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Username { get; set; }
    public string? LanguageCode { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastInteractionAt { get; set; }
}