namespace DnDDMBlazorAI.DnDClassActions
{
    public static class BardDiceActions
    {
        public static List<string> GetSuggestions(string context)
        {
            var lowered = context.ToLowerInvariant();
            var suggestions = new List<string>();

            if (lowered.Contains("sing") || lowered.Contains("charm"))
                suggestions.Add("Perform a captivating song — roll a D20 for charisma.");

            if (lowered.Contains("mock") || lowered.Contains("taunt"))
                suggestions.Add("Use cutting words — roll a D8 to affect morale.");

            if (lowered.Contains("support") || lowered.Contains("boost"))
                suggestions.Add("Inspire an ally — roll a D6 to determine effectiveness.");

            if (!suggestions.Any())
                suggestions.Add("Improvise dramatically — roll a D6 to steal the spotlight.");

            return suggestions;
        }
    }
}
