// File: ChatMessage.cs

public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public string Text { get; set; }

    // --- THE FIX IS HERE ---
    // 1. We changed the type from DateTime to string.
    // 2. We format the date into the ISO 8601 round-trip ("o") format.
    //    This format is guaranteed to be understood by JavaScript's new Date().
    public string SentUtc { get; set; } = DateTime.UtcNow.ToString("o");
}