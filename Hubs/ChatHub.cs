// File: ChatHub.cs
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics; // Important for debugging
namespace ChatBackend.Hubs;


public class ChatHub : Hub
{
    private readonly IModerationService _moderationService;

    public ChatHub(IModerationService moderationService)
    {
        _moderationService = moderationService;
    }

    // This is the method called by your React frontend.
    public async Task SendMessage(string userId, string text)
    {
        var moderationResult = await _moderationService.ModerateAsync(text);

        if (moderationResult.IsFlagged)
        {
            // If moderated, send a rejection only to the person who sent it.
            await Clients.Caller.SendAsync("MessageRejected", moderationResult.Reason);
        }
        else
        {
            // --- THIS IS THE CRITICAL FIX ---
            // 1. Create a NEW instance of our complete ChatMessage class.
            var messageObject = new ChatMessage
            {
                UserId = userId, // Set the UserId from the input
                Text = text      // Set the Text from the input
            };
            // The Id and SentUtc properties are set automatically by the class itself.

            // 2. (For Debugging) Log the object on the backend to PROVE it has the SentUtc property.
            Debug.WriteLine($"---> Sending message. User: {messageObject.UserId}, SentUTC: {messageObject.SentUtc}");

            // 3. Send the COMPLETE messageObject to all clients.
            await Clients.All.SendAsync("ReceiveMessage", messageObject);
        }
    }
}