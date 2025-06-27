// File: IModerationService.cs

public class ModerationResult
{
    public bool IsFlagged { get; set; }
    public string? Reason { get; set; }
}

public interface IModerationService
{
    Task<ModerationResult> ModerateAsync(string message);
}