namespace SimcToBrConverter.Conditions
{
    public class ChargesConditionConverter : BaseConditionConverter
    {
        public override bool CanConvert(string condition)
        {
            return condition.StartsWith("cooldown.") && condition.Contains(".full_recharge_time");
        }

        protected override (string Result, bool Negate) ConvertTask(string spell, string task)
        {
            string result;
            bool negate = false;
            switch (task)
            {
                case "full_recharge_time":
                    result = $"charges.{spell}.timeTillFull()";
                    break;
                default:
                    result = ""; // Return an empty string or handle the default case as needed
                    break;
            }

            return (result, negate);
        }
    }
}
