using SimcToBrConverter.Utilities;
using System.Text.RegularExpressions;

namespace SimcToBrConverter.Conditions
{
    public abstract class BaseConditionConverter : IConditionConverter
    {
        // Virtual property that derived classes can override to specify the prefix
        protected virtual string ConditionPrefix => "";

        // Virtual method that derived classes can override if they need custom logic
        public virtual bool CanConvert(string condition)
        {
            return condition.StartsWith(ConditionPrefix) || condition.StartsWith("!" + ConditionPrefix);
        }

        public string Convert(string condition)
        {
            bool isNegated = condition.StartsWith("!");
            var parts = condition.TrimStart('!').Split('.');
            var conditionType = parts[0]; // e.g., "buff", "talent", etc.
            var spell = StringUtilities.ConvertToCamelCase(parts[1]);
            var task = parts.Length > 2 ? parts[2] : ""; // If no task is present, assign ""

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
            if (!string.IsNullOrEmpty(comparisonOperator))
                result += " " + comparisonOperator + " " + comparisonValue;
                

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
