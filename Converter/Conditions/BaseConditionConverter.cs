using SimcToBrConverter.Utilities;
using System.Text.RegularExpressions;

namespace SimcToBrConverter.Conditions
{
    public abstract class BaseConditionConverter : IConditionConverter
    {
        // Virtual property that derived classes can override to specify the prefix for conditions they handle.
        protected virtual string ConditionPrefix => "";

        // Determines if the converter can handle the given condition based on its prefix.
        public virtual string? CanConvert(string condition)
        {
            return condition.StartsWith(ConditionPrefix) ? ConditionPrefix : null;
        }

        // Checks if the given part is either a comparison operator or a number.
        private bool IsComparisonOrNumber(string part, out string result)
        {
            if (Regex.IsMatch(part, @"^[<>=]+$") || double.TryParse(part, out _))
            {
                result = part;
                return true;
            }
            result = string.Empty;
            return false;
        }

        // Splits the condition part into its constituent components: conditionType, spell, and task.
        private (string ConditionType, string Spell, string Task) SplitConditionPart(string part)
        {
            var subparts = part.Split('.');
            var conditionType = subparts[0];
            var spell = subparts.Length > 1 ? subparts[1] : string.Empty;
            var task = subparts.Length > 2 ? subparts[2] : string.Empty;
            return (conditionType, spell, task);
        }

        // Converts the given condition part using the appropriate condition converter.
        public (string ConvertedConditionPart, List<string> NotConvertedParts) Convert(string conditionPart, string action, List<IConditionConverter> conditionConverters)
        {
            var formattedCommand = StringUtilities.ConvertToCamelCase(action);
            var convertedParts = new List<string>();
            var notConvertedParts = new List<string>();

            // Check if the condition is empty
            if (string.IsNullOrWhiteSpace(conditionPart))
            {
                return (string.Empty, notConvertedParts);
            }

            // Split the condition into parts based on comparison operators
            var parts = Regex.Split(conditionPart, @"([<>=]+)");

            // Create a lookup for the converters
            var converterLookup = conditionConverters
                .Select(converter => new { Key = converter.CanConvert(conditionPart), Converter = converter })
                .Where(item => item.Key != null)
                .ToDictionary(item => item.Key!, item => item.Converter);


            // Convert each part separately
            foreach (var part in parts)
            {
                if (part.Length == 0) continue;

                if (IsComparisonOrNumber(part, out var result))
                {
                    convertedParts.Add(result);
                }
                else
                {
                    var (conditionType, spell, task) = SplitConditionPart(part);
                    string convertedPart;
                    bool negate;
                    bool converted;

                    // Use the lookup to find the appropriate condition converter
                    if (converterLookup.TryGetValue(part, out var converter))
                    {
                        (convertedPart, negate, converted) = converter.ConvertTask(spell, task, formattedCommand);
                        if (negate)
                        {
                            convertedPart = $"not {convertedPart}";
                        }
                        convertedParts.Add(convertedPart);
                        if (!converted)
                        {
                            notConvertedParts.Add(part);
                        }
                    }
                }
            }

            // Combine the converted parts
            var finalResult = string.Join(" ", convertedParts);
            return (finalResult, notConvertedParts);
        }


        // Abstract method that derived classes must implement to specify how to convert a task.
        public abstract (string Result, bool Negate, bool Converted) ConvertTask(string spell, string task, string command);
    }
}
