namespace SimcToBrConverter.Conditions
{
    public class TalentConditionConverter : BaseConditionConverter
    {
        // Override the ConditionPrefix property to specify the correct prefix
        protected override string ConditionPrefix => "talent.";

        protected override (string Result, bool Negate) ConvertTask(string spell, string task)
        {
            string result;
            bool negate = false;
            switch (task)
            {
                case "enabled":
                case "":
                    result = $"talent.{spell}";
                    break;
                default:
                    result = ""; // Return an empty string or handle the default case as needed
                    break;
            }

            return (result, negate);
        }
    }
}
