namespace SimcToBrConverter.Conditions
{
    /// <summary>
    /// Handles the conversion of conditions related to actions.
    /// </summary>
    public class ActionConditionConverter : BaseConditionConverter
    {
        // Override the ConditionPrefix property to specify the correct prefix
        protected override string ConditionPrefix => "action.";

        /// <summary>
        /// Converts specific tasks related to actions.
        /// </summary>
        /// <param name="spell">The name of the action or spell.</param>
        /// <param name="task">The specific task or condition to check for the action.</param>
        /// <param name="command">The action command associated with the condition.</param>
        /// <returns>A tuple containing the converted condition, a flag indicating if the condition should be negated, and a flag indicating if the conversion was successful.</returns>
        public override (string Result, bool Negate, bool Converted) ConvertTask(string conditionType, string spell, string task, string command)
        {
            string result;
            bool negate = false;
            bool converted = true;
            switch (task)
            {
                // Checks if the action or spell is ready to be cast.
                case "ready":
                    result = $"cast.able.{spell}()";
                    break;
                // Gets the potential damage value of the action or spell.
                case "damage":
                    result = $"cast.damage.{spell}()";
                    break;
                case "in_flight":
                    result = $"cast.inFlight.{spell}()";
                    break;
                default:
                    // Handles unknown tasks by setting the result to an empty string.
                    result = "";
                    converted = false;
                    break;
            }

            return (result, negate, converted);
        }
    }
}
