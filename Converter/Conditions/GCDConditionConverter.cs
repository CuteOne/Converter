namespace SimcToBrConverter.Conditions
{
    public class GCDConditionConverter : BaseConditionConverter
    {
        // Override the ConditionPrefix property to specify the correct prefix
        protected override string ConditionPrefix => "gcd.";

        public override (string Result, bool Negate, bool Converted) ConvertTask(string spell, string task, string command)
        {
            string result;
            bool negate = false;
            bool converted = true;
            switch (task)
            {
                case "remains":
                    result = $"cd.{spell}.remains()";
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
