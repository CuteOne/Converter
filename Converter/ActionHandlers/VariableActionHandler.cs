using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.ActionHandlers
{
    public class VariableActionHandler : BaseActionHandler
    {
        public override bool CanHandle()
        {
            return Program.currentActionLine.ListName.Contains("variables") || Program.currentActionLine.Action.Contains("variable");
        }

        public override void Handle()
        {
            Program.currentActionLine.Type = ActionType.Variable;
            var nameValue = Program.currentActionLine.SpecialHandling.Replace("name=", "").Trim();
            // Check if the first character is a digit
            if (char.IsDigit(nameValue[0]))
            {
                nameValue = $"value{nameValue}";
            }
            var opValue = Program.currentActionLine.Condition.Replace("op=", "").Trim();

            Program.currentActionLine.Action = $"var.{nameValue}";
            Program.currentActionLine.Condition = opValue;
        }
    }
}
