namespace DnDDMBlazorAI.DnDClassActions
{
    public static class MonkDiceActions
    {
        public static List<string> GetSuggestions(string context)
        {
            var lowered = context.ToLowerInvariant();
            var suggestions = new List<string>();

            if (lowered.Contains("strike") || lowered.Contains("martial"))
                suggestions.Add("Deliver a flurry of blows — roll a D8 for precision.");

            if (lowered.Contains("focus") || lowered.Contains("ki"))
                suggestions.Add("Channel inner energy — roll a D6 to manipulate ki.");

            if (lowered.Contains("dodge") || lowered.Contains("deflect"))
                suggestions.Add("React with practiced discipline — roll a D20 for evasion.");

            if (!suggestions.Any())
                suggestions.Add("Trust your training — roll a D6 for instinct.");

            return suggestions;
        }
    }
}
