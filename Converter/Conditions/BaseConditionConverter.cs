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

        public string Convert(string condition, string command, List<IConditionConverter> conditionConverters)
        {
            bool isNegated = condition.StartsWith("!");
            condition = condition.TrimStart('!');

            // Split the condition into parts based on comparison operators
            var parts = Regex.Split(condition, @"([<>=!]+)");
            var convertedParts = new List<string>();

            // Convert each part separately
            foreach (var part in parts)
            {
                if (part.Length == 0) continue;

                // Check if the part is a comparison operator
                if (Regex.IsMatch(part, @"^[<>=!]+$"))
                {
                    convertedParts.Add(part);
                }
                else
                {
                    // Split the part into conditionType, spell, and task
                    var subparts = part.Split('.');
                    var conditionType = subparts[0];
                    var spell = subparts.Length > 1 ? StringUtilities.ConvertToCamelCase(subparts[1]) : "";
                    var task = subparts.Length > 2 ? subparts[2] : "";
                    if (string.IsNullOrEmpty(spell) && string.IsNullOrEmpty(task))
                        task = conditionType;

                    // Convert the part using the appropriate condition converter
                    string result = "";
                    bool negate = false;
                    foreach (var converter in conditionConverters)
                    {
                        if (converter.CanConvert(part))
                        {
                            (result, negate) = converter.ConvertTask(spell, task, command);
                            break;
                        }
                    }
                    if (isNegated ^ negate) // XOR to combine the two negation flags
                    {
                        result = $"not {result}";
                    }
                    convertedParts.Add(result);
                }
            }

            // Combine the converted parts
            var finalResult = string.Join(" ", convertedParts);
            return finalResult;
        }

        public abstract (string Result, bool Negate) ConvertTask(string spell, string task, string command);
    }
}
