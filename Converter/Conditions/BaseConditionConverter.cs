using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.Conditions
{
    public abstract class BaseConditionConverter : IConditionConverter
    {
        // Virtual property that derived classes can override to specify the prefix for conditions they handle.
        protected virtual string ConditionPrefix => "";

        // Determines if the converter can handle the given condition based on its prefix.
        public virtual string? CanConvert(string conditionPart)
        {
            return conditionPart.StartsWith(ConditionPrefix) ? ConditionPrefix : null;
        }

        // Splits the condition part into its constituent components: conditionType, spell, and task.
        private static (string ConditionType, string Spell, string Task, string AdditionalParts) SplitConditionPart(string part)
        {
            if (part.StartsWith("cooldown"))
            {
                part = part;
            }
            var subparts = part.Split('.');
            var conditionType = subparts[0];
            var spell = subparts.Length > 1 ? subparts[1] : string.Empty;
            var task = subparts.Length > 2 ? subparts[2] : string.Empty;
            var additionalParts = subparts.Length > 3 ? subparts[3] : string.Empty;
            //if (string.IsNullOrEmpty(spell) && string.IsNullOrEmpty(task))
            //    task = conditionType;
            //if (string.IsNullOrEmpty(task) && !string.IsNullOrEmpty(spell))
            //    task = spell;
            return (conditionType, spell, task, additionalParts);
        }

        // Converts the given condition part using the appropriate condition converter.
        public (string ConvertedConditionPart, List<string> NotConvertedParts) ConvertPart(string conditionPart, string action)
        {
            var formattedCommand = StringUtilities.ConvertToCamelCase(action);
            var notConvertedParts = new List<string>();

            if (string.IsNullOrWhiteSpace(conditionPart))
            {
                return (string.Empty, notConvertedParts);
            }

            var (conditionType, spell, task, additionalParts) = SplitConditionPart(conditionPart);

            // Convert the spell to camelCase
            spell = StringUtilities.ConvertToCamelCase(spell);

            var (convertedPart, negate, converted) = ConvertTask(conditionType, spell, task, formattedCommand, additionalParts);

            if (negate)
            {
                convertedPart = $"not {convertedPart}";
            }

            if (!converted)
            {
                notConvertedParts.Add(conditionPart);
            }

            return (convertedPart, notConvertedParts);
        }

        // Abstract method that derived classes must implement to specify how to convert a task.
        public abstract (string Result, bool Negate, bool Converted) ConvertTask(string conditionType, string spell, string task, string command, string op);
    }
}
