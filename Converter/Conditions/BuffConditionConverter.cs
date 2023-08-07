namespace SimcToBrConverter.Conditions
{
    public class BuffConditionConverter : BaseConditionConverter
    {
        public override bool CanConvert(string condition)
        {
            return condition.StartsWith("buff.") || condition.StartsWith("!buff.");
        }

        protected override (string Result, bool Negate) ConvertTask(string spell, string task)
        {
            string result;
            bool negate = false;
            switch (task)
            {
                case "up":
                case "react":
                    result = $"buff.{spell}.exists()";
                    break;
                case "down":
                    result = $"buff.{spell}.exists()";
                    negate = true; // Reverse the negation for "down"
                    break;
                case "remains":
                    result = $"buff.{spell}.remains()";
                    break;
                case "stack":
                case "value":
                    result = $"buff.{spell}.count()";
                    break;
                default:
                    result = ""; // Unknown task
                    break;
            }

            return (result, negate);
        }
    }
}
