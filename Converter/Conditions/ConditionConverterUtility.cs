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

        /// <summary>
        /// Handles the conversion of an individual condition part.
        /// </summary>
        /// <param name="conditionPart">The condition part to convert.</param>
        /// <param name="actionCommand">The action command associated with the condition.</param>
        /// <param name="conditionConverters">The list of available condition converters.</param>
        /// <returns>A tuple containing the converted condition part, a flag indicating if the conversion was successful, and a list of not converted parts.</returns>
        public static (string ConvertedPart, bool WasConverted, List<string> NotConvertedParts)
            HandleConditionPart(string conditionPart, ActionLine actionLine, IConditionConverter converter)
        {
            var conditionPrefix = converter.CanConvert(conditionPart);
            if (!string.IsNullOrEmpty(conditionPrefix))
            {
                var (convertedPart, notConvertedParts) = converter.ConvertPart(conditionPart, actionLine.Action);
                return (convertedPart, true, notConvertedParts);
            }
            return (conditionPart, false, DefaultNotConverted);
        }

    }
}
