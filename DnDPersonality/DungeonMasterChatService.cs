using Azure.AI.OpenAI;
using Azure.Identity;
using DnDDMBlazorAI.DnDClassActions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.Collections.Generic;

public class DungeonMasterChatService
{
    private readonly IConfiguration _config;
    private readonly List<ChatMessage> _chatHistory = new();
    private readonly List<ChatLogMessage> _chatLog = new();
    private string? _lastFallbackSuggestion = null;
    private string _playerClass = string.Empty;

    public event Action? OnChatUpdated;

    public void SetPlayerClass(string playerClass)
    {
        _playerClass = playerClass.Trim();
    }

    public DungeonMasterChatService(IConfiguration config)
    {
        _config = config;
        // Add the system prompt as the first message in the history
        _chatHistory.Add(new SystemChatMessage(
             "You are a Dungeons & Dragons Dungeon Master guiding a {playerClass}. Narrate adventures, describe settings, control NPCs, and guide the player through imaginative scenarios. Be creative, engaging, and always stay in character as a DM. Use a max of 10 sentences and make them easy to read. Put each idea on a new line."
         ));

        if (string.IsNullOrWhiteSpace(_playerClass))
        {
            var prompt = "Before we begin, tell me your class so I can tailor your adventures. Are you a Rogue, Wizard, Paladin, Ranger, or something else?";
            _chatLog.Add(new ChatLogMessage { Role = "DM", Content = prompt });
        }

    }

    public async Task<string> SendMessageAsync(string userMessage)
    {
        var endpoint = new Uri(_config["AZURE_OPENAI_ENDPOINT"]!);
        var client = new AzureOpenAIClient(endpoint, new DefaultAzureCredential());
        var chatClient = client.GetChatClient("gpt-4o-mini");

        OnChatUpdated?.Invoke();
        // Append DM-style actionable suggestions
        var classSuggestions = DiceActionRegistry.GetSuggestions(_playerClass, userMessage);
        var formatted = string.Join("\n", classSuggestions.Select((s, i) => $"{i + 1}. {s}"));
        var enrichedPrompt = $"{userMessage}\n\nClass Context: {_playerClass}\nSuggested Actions:\n{formatted}\n\nPlease expand narratively based on the player's intent and class abilities.";

        var userMsg = new UserChatMessage(enrichedPrompt);
        _chatHistory.Add(userMsg);
        _chatLog.Add(new ChatLogMessage { Role = "Player", Content = userMessage });

        string aiResponse = string.Empty;

        var chatUpdates = chatClient.CompleteChatStreamingAsync(_chatHistory);
        await foreach (var chatUpdate in chatUpdates)
        {
            foreach (var contentPart in chatUpdate.ContentUpdate)
            {
                aiResponse += contentPart.Text;
            }
        }

        // Update history and return
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
            "You are a Dungeons & Dragons Dungeon Master guiding a {playerClass}. Narrate adventures, describe settings, control NPCs, and guide the player through imaginative scenarios. Be creative, engaging, and always stay in character as a DM. Use a max of 10 sentences and make them easy to read. Put each idea on a new line."
        ));
    }

}

// Custom message class for UI display
public class ChatLogMessage
{
    public string Role { get; set; } = string.Empty; // "Player" or "DM"
    public string Content { get; set; } = string.Empty;
}

public static class DiceActionRegistry
{
    private static readonly Dictionary<string, Func<string, List<string>>> _registry =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "Barbarian", BarbarianDiceActions.GetSuggestions },
            { "Bard", BardDiceActions.GetSuggestions },
            { "Cleric", ClericDiceActions.GetSuggestions },
            { "Druid", DruidDiceActions.GetSuggestions },
            { "Fighter", FighterDiceActions.GetSuggestions },
            { "Monk", MonkDiceActions.GetSuggestions },
            { "Paladin", PaladinDiceActions.GetSuggestions },
            { "Ranger", RangerDiceActions.GetSuggestions },
            { "Rogue", RogueDiceActions.GetSuggestions },
            { "Sorcerer", SorcererDiceActions.GetSuggestions },
            { "Warlock", WarlockDiceActions.GetSuggestions },
            { "Wizard", WizardDiceActions.GetSuggestions },
            { "Artificer", ArtificerDiceActions.GetSuggestions },
        };

    public static List<string> GetSuggestions(string playerClass, string context)
    {
        return _registry.TryGetValue(playerClass, out var provider)
            ? provider(context)
            : new List<string> { "Choose your path — roll a D6 for fate." };
    }
}



