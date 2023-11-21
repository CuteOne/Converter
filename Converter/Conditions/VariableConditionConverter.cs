namespace SimcToBrConverter.Conditions
{
    /// <summary>
    /// Handles the conversion of conditions related to variables.
    /// </summary>
    public class VariableConditionConverter : BaseConditionConverter
    {
        /// <summary>
        /// Determines if the given condition is related to variable-related conditions.
        /// </summary>
        /// <param name="condition">The condition string to check.</param>
        /// <returns>The matched prefix if the condition is related to variable-related conditions, and null otherwise.</returns>
        public override bool CanConvert(string condition)
        {
            return condition switch
            {
                string s when s.StartsWith("variable.") => "variable.",
                string s when s.StartsWith("reset") => "reset",
                // Add other power-related conditions as needed
                _ => null
            };
        }


        /// <summary>
        /// Converts the given task related to a variable into its corresponding representation.
        /// </summary>
        /// <param name="spell">The variable associated with the task.</param>
        /// <param name="task">The task to convert.</param>
        /// <param name="command">The action command.</param>
        /// <returns>A tuple containing the converted task, a flag indicating if negation is needed, and a flag indicating if the conversion was successful.</returns>
        public override (string Result, bool Negate, bool Converted) ConvertTask(string conditionType, string spell, string task, string command, string op)
        {
            string result;
            bool negate = false;
            bool converted = true;
            if (conditionType == "reset")
            {
                task = conditionType;
            }
            switch (task)
            {
                case "reset":
                    result = $"false";
                    break;
                case "":
                    result = $"var.{spell}";
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
