namespace DnDDMBlazorAI.DnDClassActions
{
    public static class PaladinDiceActions
    {
        public static List<string> GetSuggestions(string context)
        {
            var lowered = context.ToLowerInvariant();
            var suggestions = new List<string>();

            if (lowered.Contains("smite") || lowered.Contains("justice"))
                suggestions.Add("Smite evil in the name of virtue — roll a D20 for divine power.");

            if (lowered.Contains("protect") || lowered.Contains("shield"))
                suggestions.Add("Guard your allies — roll a D8 for defense.");

            if (lowered.Contains("bless") || lowered.Contains("oath"))
                suggestions.Add("Invoke your sacred vow — roll a D6 for holy influence.");

            if (!suggestions.Any())
                suggestions.Add("Let righteousness guide you — roll a D6 to test your resolve.");

            return suggestions;
        }
    }
}
