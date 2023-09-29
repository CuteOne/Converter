using SimcToBrConverter.ActionLines;
using System.Text.RegularExpressions;

namespace SimcToBrConverter.Conditions
{
    public static class ConditionConverterUtility
    {
        private static readonly List<string> DefaultNotConverted = new List<string>();

        /// <summary>
        /// Splits the provided condition string by the & and | symbols, and parentheses, keeping the delimiters.
        /// </summary>
        /// <param name="condition">The condition string to split.</param>
        /// <returns>A list of individual condition parts.</returns>
        public static List<string> SplitCondition(string condition)
        {
            if (string.IsNullOrEmpty(condition))
            {
                return new List<string>();
            }
            return Regex.Split(condition, @"([&|\(\)!]|<=|>=|<|>|=|!=|\+|\-|\*|/)")
                        .Select(part => part.Trim())
                        .Where(trimmedPart => !string.IsNullOrEmpty(trimmedPart))
                        .ToList();
        }

        public static string ConvertLogicalOperatorsToLua(string condition)
        {
            // Convert logical operators to Lua equivalent
            condition = condition.Replace("&", " and ");
            condition = condition.Replace("|", " or ");
            condition = condition.Replace("!", "not ");

            return condition;
        }

        public static bool IsOperatorOrNumber(string conditionPart)
        {
            return new[] { "&", "|", "(", ")", "!", "<=", ">=", "<", ">", "=", "!=", "+", "-", "*", "/" }.Contains(conditionPart)
                   || double.TryParse(conditionPart, out _);
        }
    }
}
