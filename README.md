# DnDDMBlazorAI

Welcome adventurer!
This tutorial will guide you through how to summon your own AI chatbot to a Blazor page, complete with it's own personality. In this case, an interactive D&D Dungeon Master!  


## Requirements:

- [ ] NuGet packages required for this application:

```bash
dotnet add package Azure.AI.Agents.Persistent 
dotnet add package Azure.AI.OpenAI
```

- [ ] AI Foundry chatbot. I used a gpt-4o-mini basic for this project.
- [ ] Get the AI resource name and replace <your-resource-name> with it.
- [ ] Example: https://ai-resource-name.cognitiveservices.azure.com/
- [ ] The json code with the Endpoint will go in User Secrets.
- [ ] Right click on the project and select **Manage User Secrets**.
- [ ] Copy and paste the below json code.

```json
{
  "AZURE_OPENAI_ENDPOINT": "https://<your-resource-name>.cognitiveservices.azure.com/"
}
``` 

## Installation

- [ ] Clone the repo to your prefered location
```bash
git clone https://github.com/Kyle-Griffitts/DnDDMBlazorAI.git
```
