namespace SimcToBrConverter.Conditions
{
    public class DotConditionConverter : BaseConditionConverter
    {
        // Override the CanConvert method to handle both "dot" and "debuff" prefixes
        public override bool CanConvert(string condition)
        {
            return condition.StartsWith("dot.") || condition.StartsWith("debuff.");
        }

        public override (string Result, bool Negate, bool Converted) ConvertTask(string spell, string task, string command)
        {
            string result;
            bool negate = false;
            bool converted = true;
            switch (task)
            {
                case "up":
                case "react":
                    result = $"debuff.{spell}.exists()";
                    break;
                case "down":
                    result = $"debuff.{spell}.exists()";
                    negate = true; // Reverse the negation for "down"
                    break;
                case "remains":
                    result = $"debuff.{spell}.remains()";
                    break;
                case "stack":
                case "value":
                    result = $"debuff.{spell}.count()";
                    break;
                case "refreshable":
                    result = $"debuff.{spell}.refresh()";
                    break;
                case "pmultiplier":
                    result = $"debuff.{spell}.pmultiplier()";
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
