namespace DnDDMBlazorAI.DnDClassActions
{
    public static class WarlockDiceActions
    {
        public static List<string> GetSuggestions(string context)
        {
            var lowered = context.ToLowerInvariant();
            var suggestions = new List<string>();

            if (lowered.Contains("pact") || lowered.Contains("eldritch"))
                suggestions.Add("Invoke eldritch pact magic — roll a D10 for arcane potency.");

            if (lowered.Contains("curse") || lowered.Contains("whisper"))
                suggestions.Add("Speak a cursed incantation — roll a D8 for corruption.");

            if (lowered.Contains("hex") || lowered.Contains("dark"))
                suggestions.Add("Release a shadow hex — roll a D20 to bind fate.");

            if (!suggestions.Any())
                suggestions.Add("Let your patron guide you — roll a D6 to discover intent.");

            return suggestions;
        }
    }
}
