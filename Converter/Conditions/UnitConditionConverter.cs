namespace SimcToBrConverter.Conditions
{
    public class UnitConditionConverter : BaseConditionConverter
    {
        public override bool CanConvert(string condition)
        {
            return condition.StartsWith("target.");
        }

        protected override (string Result, bool Negate) ConvertTask(string spell, string task)
        {
            string result;
            bool negate = false;
            switch (task)
            {
                case "time_to_die":
                    result = "unit.ttd(PLACEHOLDER)";
                    break;
                default:
                    result = ""; // Return an empty string or handle the default case as needed
                    break;
            }

            return (result, negate);
        }
    }
}
