namespace DnDDMBlazorAI.DnDClassActions
{
    public static class ArtificerDiceActions
    {
        public static List<string> GetSuggestions(string context)
        {
            var lowered = context.ToLowerInvariant();
            var suggestions = new List<string>();

            if (lowered.Contains("build") || lowered.Contains("craft"))
                suggestions.Add("Assemble a device — roll a D8 for engineering precision.");

            if (lowered.Contains("enhance") || lowered.Contains("modify"))
                suggestions.Add("Augment your gear — roll a D6 for magical infusion.");

            if (lowered.Contains("tinker") || lowered.Contains("analyze"))
                suggestions.Add("Inspect the mechanism — roll a D20 for diagnostic insight.");

            if (!suggestions.Any())
                suggestions.Add("Experiment boldly — roll a D6 to see what sparks.");

            return suggestions;
        }
    }
}
