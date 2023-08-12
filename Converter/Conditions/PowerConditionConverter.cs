namespace SimcToBrConverter.Conditions
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
        /// <returns>The matched prefix if the condition is related to a power resource, and null otherwise.</returns>
        public override string? CanConvert(string condition)
        {
            return condition switch
            {
                string s when s.StartsWith("combo_points") => "combo_points",
                // Add other power-related conditions as needed
                _ => null
            };
        }

        /// <summary>
        /// Converts the given task related to a power resource into its corresponding representation.
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
                case "combo_points":
                    result = $"comboPoints";
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
