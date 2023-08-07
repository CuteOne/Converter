using SimcToBrConverter.Utilities;
using System.Text.RegularExpressions;

namespace SimcToBrConverter.Conditions
{
    public abstract class BaseConditionConverter : IConditionConverter
    {
        public abstract bool CanConvert(string condition);

        public string Convert(string condition)
        {
            bool isNegated = condition.StartsWith("!");
            var parts = condition.TrimStart('!').Split('.');
            var spell = StringUtilities.ConvertToCamelCase(parts[0]);
            var task = parts[1];
            if (parts.Length > 2)
            {
                spell = StringUtilities.ConvertToCamelCase(parts[1]);
                task = parts[2];
            }

            // Extract any comparison operator and value from the task
            string comparisonOperator = "";
            string comparisonValue = "";
            var match = Regex.Match(task, @"(\w+)([<>=!]+)(\d+)");
            if (match.Success)
            {
                task = match.Groups[1].Value;
                comparisonOperator = match.Groups[2].Value;
                comparisonValue = match.Groups[3].Value;
            }

            (string result, bool negate) = ConvertTask(spell, task);

            // Append the comparison operator and value if present
            result += comparisonOperator + comparisonValue;

            // Apply negation if necessary
            if (isNegated ^ negate) // XOR to combine the two negation flags
            {
                result = $"not {result}";
            }

            return result;
        }

        protected abstract (string Result, bool Negate) ConvertTask(string spell, string task);
    }
}
