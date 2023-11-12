namespace SimcToBrConverter.Conditions
{
    /// <summary>
    /// Handles the conversion of conditions related to units, specifically the target.
    /// </summary>
    public class UnitConditionConverter : BaseConditionConverter
    {
        /// <summary>
        /// Determines if the given condition is related to druid specific resources.
        /// </summary>
        /// <param name="condition">The condition string to check.</param>
        /// <returns>The matched prefix if the condition is related to a druid specific resource, and null otherwise.</returns>
        public override string? CanConvert(string condition)
        {
            return condition switch
            {
                string s when s.StartsWith("fight_remains") => "fight_remains",
                string s when s.StartsWith("fight_style") => "fight_style",
                string s when s.StartsWith("in_combat") => "in_combat",
                string s when s.StartsWith("target.") => "target.",
                string s when s.StartsWith("time") => "time",
                // Add other power-related conditions as needed
                _ => null
            };
        }

        /// <summary>
        /// Converts the given task related to a unit into its corresponding representation.
        /// </summary>
        /// <param name="spell">The unit associated with the task.</param>
        /// <param name="task">The task to convert.</param>
        /// <param name="command">The action command.</param>
        /// <returns>A tuple containing the converted task, a flag indicating if negation is needed, and a flag indicating if the conversion was successful.</returns>
        public override (string Result, bool Negate, bool Converted) ConvertTask(string conditionType, string spell, string task, string command)
        {
            string result;
            bool negate = false;
            bool converted = true;
            if (conditionType == "fight_remains" || conditionType == "fight_style" || conditionType == "in_combat" || conditionType == "time")
            {
                task = conditionType;
            }
            if (conditionType == "target")
            {
                task = spell;
                spell = command;
            }

            switch (task)
            {
                case "fight_remains":
                    result = "unit.ttdGroup(40)";
                    break;
                case "fight_style":
                    result = $"unit.instance(\"{spell}\")";
                    break;
                case "in_combat":
                    result = "unit.inCombat()";
                    break;
                case "time":
                    result = "unit.combatTime()";
                    break;
                case "timeToDie":
                case "time_to_die":
                case "":
                    result = "unit.ttd(PLACEHOLDER)";
                    break;
                default:
                    result = ""; // Unknown task
                    converted = false;
                    break;
            }

            return (result, negate, converted);
        }
    }
}
