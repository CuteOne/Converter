namespace SimcToBrConverter.Conditions
{
    /// <summary>
    /// Handles the conversion of conditions related to units, specifically the target.
    /// </summary>
    public class UnitConditionConverter : BaseConditionConverter
    {
        /// <summary>
        /// Determines if the given condition starts with listed string prefix(es).
        /// </summary>
        /// <param name="condition">The condition string to check.</param>
        /// <returns>True if the condition starts with listed string(s), and false otherwise.</returns>
        public override bool CanConvert(string condition)
        {
            return condition.StartsWith("fight_remains") ||
                    condition.StartsWith("fight_style") ||
                    condition.StartsWith("in_combat") ||
                    condition.StartsWith("target.") ||
                    condition.StartsWith("time");
        }

        /// <summary>
        /// Converts the given task related to a unit into its corresponding representation.
        /// </summary>
        /// <param name="spell">The unit associated with the task.</param>
        /// <param name="task">The task to convert.</param>
        /// <param name="command">The action command.</param>
        /// <returns>A tuple containing the converted task, a flag indicating if negation is needed, and a flag indicating if the conversion was successful.</returns>
        public override (string Result, bool Negate, bool Converted) ConvertTask(string conditionType, string spell, string task, string command, string op)
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
                    if (spell == "dungeonslice") spell = "party";
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
