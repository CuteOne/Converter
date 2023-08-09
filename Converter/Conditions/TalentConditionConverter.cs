namespace SimcToBrConverter.Conditions
{
    public class TalentConditionConverter : BaseConditionConverter
    {
        // Override the ConditionPrefix property to specify the correct prefix
        protected override string ConditionPrefix => "talent.";

        public override (string Result, bool Negate, bool Converted) ConvertTask(string spell, string task, string command)
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
                    result = ""; // Return an empty string or handle the default case as needed
                    converted = false;
                    break;
            }

            return (result, negate, converted);
        }
    }
}
