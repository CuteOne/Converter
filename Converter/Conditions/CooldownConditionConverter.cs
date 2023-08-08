namespace SimcToBrConverter.Conditions
{
    public class CooldownConditionConverter : BaseConditionConverter
    {
        // Override the ConditionPrefix property to specify the correct prefix
        protected override string ConditionPrefix => "cooldown.";

        public override (string Result, bool Negate) ConvertTask(string spell, string task, string command)
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
