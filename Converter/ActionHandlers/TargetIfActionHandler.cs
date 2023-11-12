using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.ActionHandlers
{
    public class TargetIfActionHandler : BaseActionHandler
    {
        public TargetIfActionHandler() : base() { }

        public override bool CanHandle(ActionLine actionLine)
        {
            return actionLine.SpecialHandling.Contains("target_if=");
        }

        protected override ActionLine CheckHandling(ActionLine actionLine)
        {
            var targetIfValue = actionLine.SpecialHandling["target_if=".Length..].Trim();
            var modifiedCondition = actionLine.Condition;
            if (!targetIfValue.Contains("max:") && !targetIfValue.Contains("min:"))
            {
                if (!string.IsNullOrEmpty(actionLine.Condition))
                    modifiedCondition = StringUtilities.CheckForOr(targetIfValue) + "&" + StringUtilities.CheckForOr(actionLine.Condition);
                else
                    modifiedCondition = StringUtilities.CheckForOr(targetIfValue);
            }
            actionLine.Condition = modifiedCondition;

            return actionLine;
        }
    }
}
