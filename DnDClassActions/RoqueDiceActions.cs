namespace DnDDMBlazorAI.DnDClassActions
{
    public static class RogueDiceActions
    {
        public static List<string> GetSuggestions(string context)
        {
            var lowered = context.ToLowerInvariant();
            var suggestions = new List<string>();

            if (lowered.Contains("sneak") || lowered.Contains("hide"))
                suggestions.Add("Slip past unnoticed — roll a D8 for stealth.");

            if (lowered.Contains("attack") || lowered.Contains("stab") || lowered.Contains("ambush"))
                suggestions.Add("Strike from the shadows — roll a D20 for finesse.");

            if (lowered.Contains("search") || lowered.Contains("trap"))
                suggestions.Add("Disarm or detect traps — roll a D6 for perception.");

            if (!suggestions.Any())
                suggestions.Add("Make a calculated move — roll a D6 to test luck.");

            return suggestions;
        }
    }
}
