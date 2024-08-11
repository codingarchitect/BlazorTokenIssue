namespace BlazorApp.Data.Chat;

public class ChatMessageModel
{
    public string Username { get; set; }
    public string Page { get; set; }

    public bool IsAllowed { get; set; }
}
