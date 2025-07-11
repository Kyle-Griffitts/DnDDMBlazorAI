namespace DnDDMBlazorAI.DnDClassActions
{
    public static class RangerDiceActions
    {
        public static List<string> GetSuggestions(string context)
        {
            var lowered = context.ToLowerInvariant();
            var suggestions = new List<string>();

            if (lowered.Contains("track") || lowered.Contains("hunt"))
                suggestions.Add("Follow enemy trails — roll a D8 for tracking success.");

            if (lowered.Contains("shoot") || lowered.Contains("arrow"))
                suggestions.Add("Take a ranged shot — roll a D20 for precision.");

            if (lowered.Contains("survive") || lowered.Contains("forage"))
                suggestions.Add("Endure the wilderness — roll a D6 for survival instincts.");

            if (!suggestions.Any())
                suggestions.Add("Trust your surroundings — roll a D6 to adapt instinctively.");

            return suggestions;
        }
    }
}
