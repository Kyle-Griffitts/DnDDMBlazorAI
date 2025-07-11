namespace DnDDMBlazorAI.DnDClassActions
{
    public static class ClericDiceActions
    {
        public static List<string> GetSuggestions(string context)
        {
            var lowered = context.ToLowerInvariant();
            var suggestions = new List<string>();

            if (lowered.Contains("heal") || lowered.Contains("restore"))
                suggestions.Add("Channel divine energy — roll a D8 for healing power.");

            if (lowered.Contains("smite") || lowered.Contains("undead"))
                suggestions.Add("Call upon sacred wrath — roll a D20 for holy judgment.");

            if (lowered.Contains("pray") || lowered.Contains("bless"))
                suggestions.Add("Invoke a blessing — roll a D6 for favor.");

            if (!suggestions.Any())
                suggestions.Add("Seek guidance from above — roll a D6 to test divine insight.");

            return suggestions;
        }
    }
}
