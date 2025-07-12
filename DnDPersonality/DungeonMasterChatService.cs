using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI.Chat;

public class DungeonMasterChatService
{
    private readonly IConfiguration _config;
    private readonly List<ChatMessage> _chatHistory = new();
    private readonly List<ChatLogMessage> _chatLog = new();
    private string _playerClass = string.Empty;
    private string? _lastAbilityContext = null;
    public event Action? OnChatUpdated;

    // keeps track of the player class
    public void SetPlayerClass(string playerClass) => _playerClass = playerClass.Trim();

    // applies context to the last rolled value
    public void SetSkillContext(string context) => _lastAbilityContext = context.Trim();

    // AI initialization prompts
    public static class DungeonMasterPrompts
    {
        public const string SystemPrompt =
            "You are a Dungeons & Dragons Dungeon Master guiding a {playerClass}. " +
            "Narrate adventures, describe settings, control NPCs, and guide the player through imaginative scenarios. " +
            "Be creative, engaging, and always stay in character as a DM. " +
            "Use a max of 10 sentences and make them easy to read. Put each idea on a new line.";

        public const string FallbackClassPrompt =
            "Before we begin, tell me your class so I can tailor your adventures. " +
            "Are you a Rogue, Wizard, Paladin, Ranger, or something else?";
    }

    // Method to enrich the AI prompt based on player message and class. Also includes formatting.
    private string BuildEnrichedPrompt(string userMessage)
    {
        var enrichedRollMessage = $"{userMessage}\n\nClass Context: {_playerClass}\nSuggested Actions:\n\nPlease expand narratively based on the player's intent and class abilities. " +
            $"If the player provides a dice roll, interpret it as a test of chance or skill." +
            $"\r\nAbility Context: {_lastAbilityContext} Use the roll’s number and class abilities to shape dramatic outcomes." +
            $"\r\nPrompt rolls from the correct sided dice. Available dice to the player are D20, D12, D10, D8, D6, and D4";

        return enrichedRollMessage;
    }

    // Constructor for the ChatService
    public DungeonMasterChatService(IConfiguration config)
    {
        _config = config;
        InitializeSession(); // call for the initilization session
    }

    // initialize the Chat session method
    private void InitializeSession()
    {
        _chatHistory.Add(new SystemChatMessage(DungeonMasterPrompts.SystemPrompt));

        if (string.IsNullOrWhiteSpace(_playerClass))
        { 
            _chatLog.Add(new ChatLogMessage
            { 
                Role = ChatRole.DM,
                Content = DungeonMasterPrompts.FallbackClassPrompt    
            });
        }   
    }

    // This task is what connects the chatbot. The ENPOINT string is what pulls the user secret 
    public async Task<string> SendMessageAsync(string userMessage)
    {
        var endpoint = new Uri(_config["AZURE_OPENAI_ENDPOINT"]!);
        var client = new AzureOpenAIClient(endpoint, new DefaultAzureCredential());
        var chatClient = client.GetChatClient("gpt-4o-mini");

        OnChatUpdated?.Invoke();

        var userMsg = new UserChatMessage(BuildEnrichedPrompt(userMessage));

        _chatHistory.Add(userMsg);
        _chatLog.Add(new ChatLogMessage { Role = ChatRole.Player, Content = userMessage });

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
        _chatLog.Add(new ChatLogMessage { Role = ChatRole.DM, Content = aiResponse });

        return aiResponse;
    }

    // Returns a user-friendly chat log with explicit roles
    public IReadOnlyList<ChatLogMessage> GetChatLog() => _chatLog.AsReadOnly();

    // Returns the raw OpenAI chat history (for context)
    public IReadOnlyList<ChatMessage> GetHistory() => _chatHistory.AsReadOnly();

    // Clear current page on start up and add the initial prompt to get the AI into character
    // Since we are not saving sessions, this allows a consitent but new experience on each run
    public void ClearHistory()
    {
        _chatHistory.Clear();
        _chatLog.Clear();
        InitializeSession(); // initialize a new session after clearing the current one
    }
}

//Type safe roles with enum
public enum ChatRole { Player, DM }
// Custom message class for UI display
public class ChatLogMessage
{
    public ChatRole Role { get; set; } // "Player" or "DM"
    public string Content { get; set; } = string.Empty;
}
//Chat Role Extentions to populate the desired name
public static class ChatRoleExtension
{
    public static string ToDisplayName(this ChatRole role)
        => role switch
        {
            ChatRole.Player => "Player",
            ChatRole.DM => "Dungeon Master",
            _ => role.ToString()
        };
}




