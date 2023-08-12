namespace SimcToBrConverter.Conditions
{
    /// <summary>
    /// Handles the conversion of conditions related to cooldowns.
    /// </summary>
    public class CooldownConditionConverter : BaseConditionConverter
    {
        // Specifies the prefix for conditions related to cooldowns.
        protected override string ConditionPrefix => "cooldown.";

        /// <summary>
        /// Converts specific tasks related to cooldowns.
        /// </summary>
        /// <param name="spell">The spell associated with the cooldown.</param>
        /// <param name="task">The specific cooldown task to convert.</param>
        /// <param name="command">The action command associated with the condition.</param>
        /// <returns>A tuple containing the converted condition, a flag indicating if the condition should be negated, and a flag indicating if the conversion was successful.</returns>
        public override (string Result, bool Negate, bool Converted) ConvertTask(string spell, string task, string command)
        {
            string result;
            bool negate = false;
            bool converted = true;
            switch (task)
            {
                case "charges":
                    // Converts to a check for the number of charges available for the spell.
                    result = $"charges.{spell}.count()";
                    break;
                case "full_recharge_time":
                    // Converts to a check for the time until the spell is fully recharged.
                    result = $"charges.{spell}.timeTillFull()";
                    break;
                case "remains":
                    // Converts to a check for the remaining cooldown time for the spell.
                    result = $"cd.{spell}.remains()";
                    break;
                default:
                    // Handle unknown tasks
                    result = "";
                    converted = false;
                    break;
            }

            return (result, negate, converted);
        }
    }
}
