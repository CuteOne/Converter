namespace SimcToBrConverter.Conditions
{
    /// <summary>
    /// Handles the conversion of conditions related to talents.
    /// </summary>
    public class TalentConditionConverter : BaseConditionConverter
    {
        /// <summary>
        /// Specifies the prefix for talent-related conditions.
        /// </summary>
        protected override string ConditionPrefix => "talent.";

        /// <summary>
        /// Converts the given task related to a talent into its corresponding representation.
        /// </summary>
        /// <param name="spell">The talent associated with the task.</param>
        /// <param name="task">The task to convert.</param>
        /// <param name="command">The action command.</param>
        /// <returns>A tuple containing the converted task, a flag indicating if negation is needed, and a flag indicating if the conversion was successful.</returns>
        public override (string Result, bool Negate, bool Converted) ConvertTask(string conditionType, string spell, string task, string command, string op)
        {
            string result;
            bool negate = false;
            bool converted = true;
            switch (task)
            {
                case "enabled":
                case "":
                    result = $"talent.{spell}";
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
