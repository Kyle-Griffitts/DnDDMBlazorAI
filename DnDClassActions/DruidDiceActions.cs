namespace DnDDMBlazorAI.DnDClassActions
{
    public static class DruidDiceActions
    {
        public static List<string> GetSuggestions(string context)
        {
            var lowered = context.ToLowerInvariant();
            var suggestions = new List<string>();

            if (lowered.Contains("nature") || lowered.Contains("forest"))
                suggestions.Add("Read the signs of nature — roll a D6 for environmental clarity.");

            if (lowered.Contains("shape") || lowered.Contains("transform"))
                suggestions.Add("Shift into a beast form — roll a D8 to determine control.");

            if (lowered.Contains("cast") || lowered.Contains("weather"))
                suggestions.Add("Manipulate natural forces — roll a D20 for spellcasting.");

            if (!suggestions.Any())
                suggestions.Add("Draw power from the wild — roll a D6 for instinctive reaction.");

            return suggestions;
        }
    }
}
