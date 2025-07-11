namespace DnDDMBlazorAI.DnDClassActions
{
    public static class BarbarianDiceActions
    {
        public static List<string> GetSuggestions(string context)
        {
            var lowered = context.ToLowerInvariant();
            var suggestions = new List<string>();

            if (lowered.Contains("rage") || lowered.Contains("furious"))
                suggestions.Add("Unleash primal fury — roll a D12 to determine your rage impact.");

            if (lowered.Contains("attack") || lowered.Contains("charge"))
                suggestions.Add("Strike with brute force — roll a D20 to test your strength.");

            if (lowered.Contains("endure") || lowered.Contains("survive"))
                suggestions.Add("Withstand the assault — roll a D8 for resilience.");

            if (!suggestions.Any())
                suggestions.Add("Let instinct guide you — roll a D6 to decide your feral action.");

            return suggestions;
        }
    }
}
