namespace DnDDMBlazorAI.DnDClassActions
{
    public static class SorcererDiceActions
    {
        public static List<string> GetSuggestions(string context)
        {
            var lowered = context.ToLowerInvariant();
            var suggestions = new List<string>();

            if (lowered.Contains("cast") || lowered.Contains("wild"))
                suggestions.Add("Unleash chaotic arcana — roll a D20 for spell control.");

            if (lowered.Contains("focus") || lowered.Contains("channel"))
                suggestions.Add("Harness inner magic — roll a D8 to amplify energy.");

            if (lowered.Contains("burst") || lowered.Contains("blast"))
                suggestions.Add("Release a surge of power — roll a D6 for area impact.");

            if (!suggestions.Any())
                suggestions.Add("Let chaos choose your path — roll a D6 for wild influence.");

            return suggestions;
        }
    }
}
