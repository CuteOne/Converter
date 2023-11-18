namespace SimcToBrConverter.Conditions
{
    /// <summary>
    /// Handles the conversion of conditions related to buffs.
    /// </summary>
    public class BuffConditionConverter : BaseConditionConverter
    {
        // Override the ConditionPrefix property to specify the correct prefix
        protected override string ConditionPrefix => "buff.";

        /// <summary>
        /// Converts specific tasks related to buffs.
        /// </summary>
        /// <param name="spell">The name of the buff.</param>
        /// <param name="task">The specific task or condition to check for the buff.</param>
        /// <param name="command">The action command associated with the condition.</param>
        /// <returns>A tuple containing the converted condition, a flag indicating if the condition should be negated, and a flag indicating if the conversion was successful.</returns>
        public override (string Result, bool Negate, bool Converted) ConvertTask(string conditionType, string spell, string task, string command, string op)
        {
            string result;
            bool negate = false;
            bool converted = true;
            switch (task)
            {
                case "up":
                case "react":
                    if (spell.StartsWith("bt"))
                    {
                        // Special handling for the Bloodtalons buff
                        spell = spell.Replace("bt", "");
                        spell = char.ToLower(spell[0]) + spell[1..];
                        result = $"btGen.{spell}";
                        break;
                    }
                    else
                    { 
                        // Checks if the buff is currently active on the target.
                        result = $"buff.{spell}.exists()";
                        break;
                    }
                case "down":
                    if (spell.StartsWith("bt"))
                    {
                        // Special handling for the Bloodtalons buff
                        spell = spell.Replace("bt", "");
                        spell = char.ToLower(spell[0]) + spell[1..];
                        result = $"btGen.{spell}";
                        negate = true; // Reverse the condition to check for buff absence.
                        break;
                    }
                    else
                    {
                        // Checks if the buff is not active on the target.
                        result = $"buff.{spell}.exists()";
                        negate = true; // Reverse the condition to check for buff absence.
                        break;
                    }
                case "remains":
                    // Gets the remaining duration of the buff on the target.
                    result = $"buff.{spell}.remains()";
                    break;
                case "stack":
                    // Gets the current stack count of the buff on the target.
                    result = $"buff.{spell}.stack()";
                    break;
                case "value":
                    // Gets the current stack count or value of the buff on the target.
                    result = $"buff.{spell}.count()";
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
