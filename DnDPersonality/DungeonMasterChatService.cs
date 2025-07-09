using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

public class DungeonMasterChatService
{
    private readonly IConfiguration _config;

    public DungeonMasterChatService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<string> GetDungeonMasterResponseAsync(string userMessage)
    {
        var endpoint = new Uri(_config["AZURE_OPENAI_ENDPOINT"]!);
        var client = new AzureOpenAIClient(endpoint, new DefaultAzureCredential());
        var chatClient = client.GetChatClient("gpt-4o-mini");

        string aiResponse = string.Empty;

        var chatUpdates = chatClient.CompleteChatStreamingAsync(
            [
                new SystemChatMessage("You are a Dungeons & Dragons Dungeon Master. Narrate adventures, describe settings, control NPCs, and guide the player through imaginative scenarios. Be creative, engaging, and always stay in character as a DM."),
                new UserChatMessage(userMessage)
            ]);

        await foreach (var chatUpdate in chatUpdates)
        {
            foreach (var contentPart in chatUpdate.ContentUpdate)
            {
                aiResponse += contentPart.Text;
            }
        }

        return aiResponse;
    }
}