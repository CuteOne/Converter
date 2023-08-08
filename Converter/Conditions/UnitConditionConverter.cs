namespace SimcToBrConverter.Conditions
{
    public class UnitConditionConverter : BaseConditionConverter
    {
        // Override the ConditionPrefix property to specify the correct prefix
        protected override string ConditionPrefix => "target.";

        public override (string Result, bool Negate) ConvertTask(string spell, string task, string command)
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
