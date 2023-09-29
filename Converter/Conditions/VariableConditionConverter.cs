namespace SimcToBrConverter.Conditions
{
    /// <summary>
    /// Handles the conversion of conditions related to variables.
    /// </summary>
    public class VariableConditionConverter : BaseConditionConverter
    {
        /// <summary>
        /// Specifies the prefix for variable-related conditions.
        /// </summary>
        protected override string ConditionPrefix => "variable.";

        /// <summary>
        /// Converts the given task related to a variable into its corresponding representation.
        /// </summary>
        /// <param name="spell">The variable associated with the task.</param>
        /// <param name="task">The task to convert.</param>
        /// <param name="command">The action command.</param>
        /// <returns>A tuple containing the converted task, a flag indicating if negation is needed, and a flag indicating if the conversion was successful.</returns>
        public override (string Result, bool Negate, bool Converted) ConvertTask(string conditionType, string spell, string task, string command)
        {
            string result;
            bool negate = false;
            bool converted = true;
            switch (task)
            {
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
