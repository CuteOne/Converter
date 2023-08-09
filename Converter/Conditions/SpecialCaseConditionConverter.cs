namespace SimcToBrConverter.Conditions
{
    public class SpecialCaseConditionConverter : BaseConditionConverter
    {
        // Override the CanConvert method to specify the conditions that this converter can handle
        public override bool CanConvert(string condition)
        {
            return condition.StartsWith("persistent_multiplier") 
                || condition.StartsWith("refreshable") 
                || condition.StartsWith("combo_points") 
                /* || other special cases */;
        }

        public override (string Result, bool Negate, bool Converted) ConvertTask(string spell, string task, string command)
        {
            string result;
            bool negate = false;
            bool converted = true;
            switch (task)
            {
                case "persistent_multiplier":
                    result = $"dot.{command}.applied(PLACEHOLDER)";
                    break;
                case "refreshable":
                    result = $"dot.{command}.refresh(PLACEHOLDER)";
                    break;
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
