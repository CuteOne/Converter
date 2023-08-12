using SimcToBrConverter.ActionLines;
using System.Text.RegularExpressions;

namespace SimcToBrConverter.Conditions
{
    public static class ConditionConverterUtility
    {
        private static readonly List<string> DefaultNotConverted = new List<string>();

        // Split the condition string by the & and | symbols, and parentheses, keeping the delimiters
        public static List<string> SplitCondition(string condition)
        {
            return Regex.Split(condition, @"([&|\(\)!])")
                    .Select(part => part.Trim())
                    .Where(trimmedPart => !string.IsNullOrEmpty(trimmedPart))
                    .ToList();
        }

        // This method can be used to handle individual parts of the condition
        public static (string ConvertedPart, bool WasConverted, List<string> NotConvertedParts) HandleConditionPart(string conditionPart, ActionLine actionLine, List<IConditionConverter> conditionConverters)
        {
            foreach (var converter in conditionConverters)
            {
                if (converter.CanConvert(conditionPart))
                {
                    var (convertedPart, notConvertedParts) = converter.Convert(conditionPart, actionLine.Action, conditionConverters);
                    return (convertedPart, true, notConvertedParts);
                }
            }
            return (conditionPart, false, DefaultNotConverted);
        }
    }

}
