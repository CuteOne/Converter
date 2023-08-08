namespace SimcToBrConverter.Conditions
{
    public class DotConditionConverter : BaseConditionConverter
    {
        // Override the ConditionPrefix property to specify the correct prefix
        protected override string ConditionPrefix => "dot.";

        public override (string Result, bool Negate) ConvertTask(string spell, string task, string command)
        {
            string result;
            bool negate = false;
            switch (task)
            {
                case "up":
                case "react":
                    result = $"dot.{spell}.exists()";
                    break;
                case "down":
                    result = $"dot.{spell}.exists()";
                    negate = true; // Reverse the negation for "down"
                    break;
                case "remains":
                    result = $"dot.{spell}.remains()";
                    break;
                case "stack":
                case "value":
                    result = $"dot.{spell}.count()";
                    break;
                case "refreshable":
                    result = $"dot.{spell}.refresh()";
                    break;
                case "pmultiplier":
                    result = $"dot.{spell}.pmultiplier()";
                    break;
                default:
                    result = ""; // Unknown task
                    break;
            }

            return (result, negate);
        }
    }
}
