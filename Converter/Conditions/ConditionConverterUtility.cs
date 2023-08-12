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
            return Regex.Split(condition, @"([&|\(\)!])")
                        .Select(part => part.Trim())
                        .Where(trimmedPart => !string.IsNullOrEmpty(trimmedPart))
                        .ToList();
        }

        /// <summary>
        /// Handles the conversion of an individual condition part.
        /// </summary>
        /// <param name="conditionPart">The condition part to convert.</param>
        /// <param name="actionCommand">The action command associated with the condition.</param>
        /// <param name="conditionConverters">The list of available condition converters.</param>
        /// <returns>A tuple containing the converted condition part, a flag indicating if the conversion was successful, and a list of not converted parts.</returns>
        public static (string ConvertedPart, bool WasConverted, List<string> NotConvertedParts)
            HandleConditionPart(string conditionPart, ActionLine actionLine, List<IConditionConverter> conditionConverters)
        {
            foreach (var converter in conditionConverters)
            {
                var conditionPrefix = converter.CanConvert(conditionPart);
                if (!string.IsNullOrEmpty(conditionPrefix))
                {
                    var (convertedPart, notConvertedParts) = converter.Convert(conditionPart, actionLine.Action, conditionConverters);
                    return (convertedPart, true, notConvertedParts);
                }
            }
            return (conditionPart, false, DefaultNotConverted);
        }
    }
}
