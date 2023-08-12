namespace SimcToBrConverter.Conditions
{
    /// <summary>
    /// Handles the conversion of conditions that start with "dot." or "debuff." prefixes.
    /// </summary>
    public class DebuffConditionConverter : BaseConditionConverter
    {
        /// <summary>
        /// Determines if the given condition starts with "dot." or "debuff." prefixes.
        /// </summary>
        /// <param name="condition">The condition string to check.</param>
        /// <returns>The matched prefix if the condition starts with either "dot." or "debuff.", and null otherwise.</returns>
        public override string? CanConvert(string condition)
        {
            return condition switch
            {
                string s when s.StartsWith("dot.") => "dot.",
                string s when s.StartsWith("debuff.") => "debuff.",
                string s when s.StartsWith("persistent_multiplier") => "persistent_multiplier",
                string s when s.StartsWith("refreshable") => "refreshable",
                _ => null
            };
        }

        /// <summary>
        /// Converts the given task related to a spell into its corresponding representation.
        /// </summary>
        /// <param name="spell">The spell associated with the task.</param>
        /// <param name="task">The task to convert.</param>
        /// <param name="command">The action command.</param>
        /// <returns>A tuple containing the converted task, a flag indicating if negation is needed, and a flag indicating if the conversion was successful.</returns>
        public override (string Result, bool Negate, bool Converted) ConvertTask(string spell, string task, string command)
        {
            string result;
            bool negate = false;
            bool converted = true;
            switch (task)
            {
                case "up":
                case "react":
                    result = $"debuff.{spell}.exists(PLACEHOLDER)";
                    break;
                case "down":
                    result = $"debuff.{spell}.exists(PLACEHOLDER)";
                    negate = true; // Reverse the negation for "down"
                    break;
                case "remains":
                    result = $"debuff.{spell}.remains(PLACEHOLDER)";
                    break;
                case "stack":
                case "value":
                    result = $"debuff.{spell}.count(PLACEHOLDER)";
                    break;
                case "refreshable":
                    result = $"debuff.{spell}.refresh(PLACEHOLDER)";
                    break;
                case "pmultiplier":
                    result = $"debuff.{spell}.pmultiplier(PLACEHOLDER)";
                    break;
                case "persistent_multiplier":
                    result = $"debuff.{command}.applied(PLACEHOLDER)";
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
