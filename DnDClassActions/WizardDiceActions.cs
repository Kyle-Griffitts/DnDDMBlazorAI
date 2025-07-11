namespace DnDDMBlazorAI.DnDClassActions
{
    public static class WizardDiceActions
    {
        public static List<string> GetSuggestions(string context)
        {
            var lowered = context.ToLowerInvariant();
            var suggestions = new List<string>();

            if (lowered.Contains("study") || lowered.Contains("ritual"))
                suggestions.Add("Consult your tome — roll a D6 for scholarly insight.");

            if (lowered.Contains("cast") || lowered.Contains("spell"))
                suggestions.Add("Cast with precision — roll a D20 for arcane success.");

            if (lowered.Contains("prepare") || lowered.Contains("focus"))
                suggestions.Add("Ready the magical sequence — roll a D8 for channeling control.");

            if (!suggestions.Any())
                suggestions.Add("Test magical intuition — roll a D6 for raw insight.");

            return suggestions;
        }
    }
}
