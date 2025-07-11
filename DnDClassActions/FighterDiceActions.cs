namespace DnDDMBlazorAI.DnDClassActions
{
    public static class FighterDiceActions
    {
        public static List<string> GetSuggestions(string context)
        {
            var lowered = context.ToLowerInvariant();
            var suggestions = new List<string>();

            if (lowered.Contains("strike") || lowered.Contains("attack"))
                suggestions.Add("Deliver a precise blow — roll a D10 for weapon efficiency.");

            if (lowered.Contains("defend") || lowered.Contains("block"))
                suggestions.Add("Guard your allies — roll a D8 to test your reaction.");

            if (lowered.Contains("tactic") || lowered.Contains("plan"))
                suggestions.Add("Execute a battle maneuver — roll a D20 for success.");

            if (!suggestions.Any())
                suggestions.Add("React based on training — roll a D6 for instinctual combat.");

            return suggestions;
        }
    }
}
