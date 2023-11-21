namespace SimcToBrConverter.Conditions
{
    public class DruidConditionConverter : BaseConditionConverter
    {
        /// <summary>
        /// Determines if the given condition is related to druid specific resources.
        /// </summary>
        /// <param name="condition">The condition string to check.</param>
        /// <returns>The matched prefix if the condition is related to a druid specific resource, and null otherwise.</returns>
        public override bool CanConvert(string condition)
        {
            return condition switch
            {
                string s when s.StartsWith("active_bt_triggers") => "active_bt_triggers",
                // Add other power-related conditions as needed
                _ => null
            };
        }

        public override (string Result, bool Negate, bool Converted) ConvertTask(string conditionType, string spell, string task, string command, string op)
        {
            string result;
            bool negate = false;
            bool converted = true;
            if (conditionType == "active_bt_triggers")
            {
                task = conditionType;
            }

            switch (task)
            {
                case "active_bt_triggers":
                    result = $"var.btGen.triggers";
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
