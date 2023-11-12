using SimcToBrConverter.ActionLines;

namespace SimcToBrConverter.ActionHandlers
{
    public class VariableActionHandler : BaseActionHandler
    {
        public VariableActionHandler() : base() { }

        public override bool CanHandle(ActionLine actionLine)
        {
            return actionLine.ListName.Contains("variables");
        }

        protected override ActionLine CheckHandling(ActionLine actionLine)
        {
            var nameValue = actionLine.SpecialHandling.Replace("name=", "").Trim();
            var opValue = actionLine.Condition.Replace("op=", "").Trim();

            actionLine.Action = $"var.{nameValue}";
            actionLine.Condition = opValue;

            return actionLine;
        }
    }
}
