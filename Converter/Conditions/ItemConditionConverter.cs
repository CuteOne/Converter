using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.Conditions
{
    public class ItemConditionConverter : BaseConditionConverter
    {
        /// <summary>
        /// Determines if the given condition starts with listed string prefix(es).
        /// </summary>
        /// <param name="condition">The condition string to check.</param>
        /// <returns>True if the condition starts with listed string(s), and false otherwise.</returns>
        public override bool CanConvert(string condition)
        {
            return condition.StartsWith("set_bonus.") ||
                    condition.StartsWith("equipped") ||
                    condition.StartsWith("trinket");
        }

        public override (string Result, bool Negate, bool Converted) ConvertTask(string conditionType, string spell, string task, string command, string op)
        {
            string result;
            bool negate = false;
            bool converted = true;
            if (conditionType == "set_bonus")
            {
                task = spell;
            }
            if (conditionType == "equipped")
            {
                op = task;
                if (!string.IsNullOrEmpty(op))
                {
                    // Add 12 to the slot number to convert from SimC Trinket Slot ID to WoW Trinket Slot ID
                    op = (int.Parse(op) + 12).ToString();
                }
                task = conditionType;
            }
            //if (conditionType == "trinket")
            //{
            //    task = conditionType;
            //}

            switch (task)
            {
                case "equipped":
                    result = $"equiped.{spell}({op})";
                    break;
                case "tier304Pc":
                    result = $"equiped.tier(30) >= 4";
                    break;
                case "tier312Pc":
                    result = $"equiped.tier(31) >= 2";
                    break;
                case "tier314Pc":
                    result = $"equiped.tier(31) >= 4";
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
