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
            return condition.StartsWith(ConditionPrefix);
        }

        private static readonly HashSet<string> TaskConditionTypes = new HashSet<string>
        {
            "target" // Add other condition types that consider the second part as a task
        };


        public (string ConvertedCondition, List<string> NotConvertedParts) Convert(string condition, string command, List<IConditionConverter> conditionConverters)
        {
            var formattedCommand = StringUtilities.ConvertToCamelCase(command);

            // Split the condition into parts based on comparison operators
            var parts = Regex.Split(condition, @"([<>=]+)");
            var convertedParts = new List<string>();
            var notConvertedParts = new List<string>();

            // Convert each part separately
            foreach (var part in parts)
            {
                if (part.Length == 0) continue;

                // Check if the part is a comparison operator
                if (Regex.IsMatch(part, @"^[<>=]+$"))
                {
                    convertedParts.Add(part);
                }
                // Check if the part is a number
                else if (double.TryParse(part, out _))
                {
                    convertedParts.Add(part);
                }
                else
                {
                    // Split the part into conditionType, spell, and task
                    var subparts = part.Split('.');
                    var conditionType = subparts[0];
                    var isTask = TaskConditionTypes.Contains(conditionType);
                    var spell = "";
                    var task = "";

                    // Handle different formats
                    if (subparts.Length == 1)
                    {
                        task = conditionType;
                        conditionType = ""; // Reset conditionType if it's actually a task
                    }
                    else if (subparts.Length == 2)
                    {
                        if (isTask)
                        {
                            task = subparts[1];
                        }
                        else
                        {
                            spell = StringUtilities.ConvertToCamelCase(subparts[1]);
                        }
                    }
                    else if (subparts.Length > 2)
                    {
                        spell = StringUtilities.ConvertToCamelCase(subparts[1]);
                        task = subparts[2];
                    }

                    // Convert the part using the appropriate condition converter
                    string result = "";
                    bool negate = false;
                    bool converted = true;
                    foreach (var converter in conditionConverters)
                    {
                        if (converter.CanConvert(part))
                        {
                            (result, negate, converted) = converter.ConvertTask(spell, task, formattedCommand);
                            break;
                        }
                    }
                    if (!converted)
                        notConvertedParts.Add(part);
                    if (negate) // XOR to combine the two negation flags
                    {
                        result = $"not {result}";
                    }
                    convertedParts.Add(result);
                }
            }

            // Combine the converted parts
            var finalResult = string.Join(" ", convertedParts);
            return (finalResult, notConvertedParts);
        }

        public abstract (string Result, bool Negate, bool Converted) ConvertTask(string spell, string task, string command);
    }
}
