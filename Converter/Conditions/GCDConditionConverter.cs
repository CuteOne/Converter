namespace SimcToBrConverter.Conditions
{
    /// <summary>
    /// Handles conditions related to the Global Cooldown (GCD) of abilities.
    /// </summary>
    public class GCDConditionConverter : BaseConditionConverter
    {
        // Specifies the prefix for conditions this converter can handle
        protected override string ConditionPrefix => "gcd.";

        /// <summary>
        /// Converts the given task related to the GCD into the appropriate format.
        /// </summary>
        /// <param name="spell">The spell or ability in question.</param>
        /// <param name="task">The specific task or condition to check.</param>
        /// <param name="command">The action command associated with the condition.</param>
        /// <returns>A tuple containing the converted condition, whether to negate the result, and whether the conversion was successful.</returns>
        public override (string Result, bool Negate, bool Converted) ConvertTask(string conditionType, string spell, string task, string command)
        {
            string result;
            bool negate = false;
            bool converted = true;
            if (string.IsNullOrEmpty(task))
            {
                task = spell;
                spell = command;
            }
            switch (task)
            {
                case "remains":
                    result = $"cd.{spell}.remains()";
                    break;
                default:
                    result = ""; // Handle unknown tasks
                    converted = false;
                    break;
            }

            return (result, negate, converted);
        }
    }
}
