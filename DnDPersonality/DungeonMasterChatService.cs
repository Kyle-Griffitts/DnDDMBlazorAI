using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI.Chat;

public class DungeonMasterChatService
{
    private readonly IConfiguration _config;
    private readonly List<ChatMessage> _chatHistory = new();
    private readonly List<ChatLogMessage> _chatLog = new();
    private string _playerClassOrSpecies = string.Empty;
    private string? _storyContext;
    private string? _lastAbilityContext = null;
    public event Action? OnChatUpdated;

    // keeps track of the player class
    public void SetPlayerClass(string playerClassOrSpecies) => _playerClassOrSpecies = playerClassOrSpecies.Trim();

    //Keeps track of story context
    public void SetStoryContext(string context) => _storyContext = context;

    // applies context to the last rolled value
    public void SetSkillContext(string context) => _lastAbilityContext = context.Trim();

    // AI initialization prompts
    public static class DungeonMasterPrompts
    {
        public const string SystemPrompt = @"
        You are a Dungeons & Dragons Dungeon Master guiding a {playerClassOrSpecies}.
        Narrate adventures, describe settings, control NPCs, and guide the player through imaginative scenarios.
        Be creative, engaging, and always stay in character as a DM.
        Use a max of 15 sentences and make them easy to read. Put each idea on a new line.";

        public const string FallbackClassPrompt =
        "Before we begin, tell me what you are! Adventurer, beast, undead... Are you a Rogue, a Mindflayer, a sentient potion, or something else?";
    }

    // Method to enrich the AI prompt based on player message and class. Also includes formatting.
    private string BuildEnrichedPrompt(string userMessage)
    {
        var enrichedRollMessage = $@"
        Story: 
        Continue this {_storyContext} as a Dungeon Master narrating an immersive Dungeons & Dragons adventure.
        Build continuity so the player experiences an evolving storyline across messages.
        Maintain consistent tone, setting, and escalating tension based on choices.

        Class and Identity Context:
        {_playerClassOrSpecies}

        Ability Context:
        {_lastAbilityContext}

        Suggested Actions with Dice Roll Recommendations:
        Provide 2–3 contextually relevant next steps based on the player’s identity and intent.
        If the player has rolled a die, interpret it as a test of chance or skill.
        Use the roll’s number and class traits to shape dramatic outcomes.
        Available dice to the player are: D20, D12, D10, D8, D6, and D4.

        Player Message:
        {userMessage}
        ";

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

        if (string.IsNullOrWhiteSpace(_playerClassOrSpecies))
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




