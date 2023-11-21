namespace SimcToBrConverter.Conditions
{
    /// <summary>
    /// Handles the conversion of conditions related to the number of targets a spell will hit.
    /// </summary>
    public class SpellTargetsConditionConverter : BaseConditionConverter
    {
        /// <summary>
        /// Determines if the given condition starts with listed string prefix(es).
        /// </summary>
        /// <param name="condition">The condition string to check.</param>
        /// <returns>True if the condition starts with listed string(s), and false otherwise.</returns>
        public override bool CanConvert(string condition)
        {
            return condition.StartsWith("spell_targets.");
        }

        /// <summary>
        /// Converts the given task related to a spell into its corresponding representation.
        /// </summary>
        /// <param name="spell">The spell associated with the task.</param>
        /// <param name="task">The task to convert.</param>
        /// <param name="command">The action command.</param>
        /// <returns>A tuple containing the converted task, a flag indicating if negation is needed, and a flag indicating if the conversion was successful.</returns>
        public override (string Result, bool Negate, bool Converted) ConvertTask(string conditionType, string spell, string task, string command, string op)
        {
            string result;
            bool negate = false;
            bool converted = true;
            result = $"#enemies.yards0"; // Using 0 as a placeholder for the range
            //switch (task)
            //{
            //    case "":
            //        result = $"#enemies.yards0"; // Using 0 as a placeholder for the range
            //        break;
            //    default:
            //        result = ""; // Return an empty string or handle the default case as needed
            //        converted = false;
            //        break;
            //}            

            return (result, negate, converted);
        }
    }
}
