using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.ConditionConverters
{
    /// <summary>
    /// Handles the conversion of conditions related to power resources like combo points, energy, mana, etc.
    /// </summary>
    public class PowerConditionConverter : BaseConditionConverter
    {
        /// <summary>
        /// Determines if the given condition is related to power resources.
        /// </summary>
        /// <param name="condition">The condition string to check.</param>
        /// <returns>True if the condition is related to a power resource, and false otherwise.</returns>
        public override bool CanConvert(string condition)
        {
            return PowerType.IsPowerType(condition);
        }

        /// <summary>
        /// Converts the given task related to a power resource into its corresponding representation.
        /// </summary>
        /// <param name="spell">The spell associated with the task.</param>
        /// <param name="task">The task to convert.</param>
        /// <param name="command">The action command.</param>
        /// <returns>A tuple containing the converted task, a flag indicating if negation is needed, and a flag indicating if the conversion was successful.</returns>
        public override (string Result, bool Negate, bool Converted) ConvertTask(string conditionType, string spell, string task, string command, string op)
        {
            string result = "";
            bool negate = false;
            bool converted = false;

            foreach (var powerType in PowerType.PowerTypes)
            {
                if (conditionType.StartsWith(powerType.SimCText))
                {
                    if (!string.IsNullOrEmpty(spell))
                    {
                        result = $"{powerType.BrText}.{spell}()";
                    }
                    else
                    {
                        result = $"{powerType.BrText}()";
                    }
                    break;
                }
            }
            if (!string.IsNullOrEmpty(result))
                converted = true;

            return (result, negate, converted);
        }
    }
}
