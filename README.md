# DnDDMBlazorAI

Welcome adventurer!
This tutorial will guide you through how to summon your own AI chatbot to a Blazor page, complete with it's own personality. In this case, an interactive D&D Dungeon Master!  

## 1. Summon the Source (Installation):

- Clone the repo to your preferred location.
      
```bash
git clone https://github.com/Kyle-Griffitts/DnDDMBlazorAI.git
```

## 2. Gather Arcane Components (Requirements):

I. NuGet packages required for this application.

```bash
dotnet add package Azure.AI.Agents.Persistent 
dotnet add package Azure.AI.OpenAI
```

II. AI Foundry chatbot. I used a gpt-4o-mini for this project.
- The json code with the Endpoint will go in User Secrets.
- Right click on the project and select **Manage User Secrets**.
- Copy and paste the below json code.
- Get the AI resource name and replace <your-resource-name> with it.
- Example: `https://ai-resource-name.cognitiveservices.azure.com/`

```json
{
  "AZURE_OPENAI_ENDPOINT": "https://<your-resource-name>.cognitiveservices.azure.com/"
}
```
For more details, check out the [ASP.NET Core User Secrets documentation](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-9.0).

## 3. Craft the Personality Scroll (Character Prompts):

I. There are two areas in the **DungeonMasterChatService.cs** where you can customize your own personality. First is the intro prompt.

- You can type anything to give your AI context about their personality here.
- The **SystemPrompt** string will not be displayed. It exists to put the AI chat bot into character before any interaction.
- The **FallbackClassPrompt** string is the initial message to be displayed in the chat window to get you started on your adventure.
      
```cs
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
```

II. The next section is an prompt enricher to give more context to the AI once someone declares a class, writes messages, or interacts with the dice rolls.

```cs
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
```

## 4. Additional Resources:
For Microsoft's official content check out [Tutorial: Build a chatbot with Azure App Service and Azure OpenAI (.NET)](https://learn.microsoft.com/en-us/azure/app-service/tutorial-ai-openai-chatbot-dotnet).
