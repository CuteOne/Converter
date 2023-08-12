﻿namespace SimcToBrConverter.Conditions
{
    /// <summary>
    /// Handles the conversion of conditions related to units, specifically the target.
    /// </summary>
    public class UnitConditionConverter : BaseConditionConverter
    {
        /// <summary>
        /// Specifies the prefix for target-related conditions.
        /// </summary>
        protected override string ConditionPrefix => "target.";

        /// <summary>
        /// Converts the given task related to a unit into its corresponding representation.
        /// </summary>
        /// <param name="spell">The unit associated with the task.</param>
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
                case "time_to_die":
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
