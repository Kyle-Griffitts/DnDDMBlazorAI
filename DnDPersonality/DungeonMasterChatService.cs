using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.Collections.Generic;

public class DungeonMasterChatService
{
    private readonly IConfiguration _config;
    private readonly List<ChatMessage> _chatHistory = new();
    private readonly List<ChatLogMessage> _chatLog = new();

    public DungeonMasterChatService(IConfiguration config)
    {
        _config = config;
        // Add the system prompt as the first message in the history
        _chatHistory.Add(new SystemChatMessage(
             "You are a Dungeons & Dragons Dungeon Master. Narrate adventures, describe settings, control NPCs, and guide the player through imaginative scenarios. Be creative, engaging, and always stay in character as a DM. Use a max of 10 sentences and make them easy to read. Put each idea on a new line."
         ));
    }

    public async Task<string> SendMessageAsync(string userMessage)
    {
        var endpoint = new Uri(_config["AZURE_OPENAI_ENDPOINT"]!);
        var client = new AzureOpenAIClient(endpoint, new DefaultAzureCredential());
        var chatClient = client.GetChatClient("gpt-4o-mini");

        // Add the user's message to the history and log
        var userMsg = new UserChatMessage(userMessage);
        _chatHistory.Add(userMsg);
        _chatLog.Add(new ChatLogMessage { Role = "Player", Content = userMessage });

        string aiResponse = string.Empty;

        // Pass the full chat history to the model
        var chatUpdates = chatClient.CompleteChatStreamingAsync(_chatHistory);

        await foreach (var chatUpdate in chatUpdates)
        {
            foreach (var contentPart in chatUpdate.ContentUpdate)
            {
                aiResponse += contentPart.Text;
            }
        }

        // Add the AI's response to the history and log
        var aiMsg = new AssistantChatMessage(aiResponse);
        _chatHistory.Add(aiMsg);
        _chatLog.Add(new ChatLogMessage { Role = "DM", Content = aiResponse });

        return aiResponse;
    }

    // Returns a user-friendly chat log with explicit roles
    public IReadOnlyList<ChatLogMessage> GetChatLog() => _chatLog.AsReadOnly();

    // Returns the raw OpenAI chat history (for context)
    public IReadOnlyList<ChatMessage> GetHistory() => _chatHistory.AsReadOnly();

    public void ClearHistory()
    {
        _chatHistory.Clear();
        _chatLog.Clear();
        _chatHistory.Add(new SystemChatMessage(
            "You are a Dungeons & Dragons Dungeon Master. Narrate adventures, describe settings, control NPCs, and guide the player through imaginative scenarios. Be creative, engaging, and always stay in character as a DM. Use a max of 10 sentences and make them easy to read. Put each idea on a new line."
        ));
    }
}

// Custom message class for UI display
public class ChatLogMessage
{
    public string Role { get; set; } = string.Empty; // "Player" or "DM"
    public string Content { get; set; } = string.Empty;
}