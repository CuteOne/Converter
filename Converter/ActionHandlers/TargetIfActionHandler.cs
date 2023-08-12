using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Conditions;

namespace SimcToBrConverter.ActionHandlers
{
    public class TargetIfActionHandler : BaseActionHandler
    {
        public TargetIfActionHandler(List<IConditionConverter> conditionConverters) : base(conditionConverters) { }

        public override bool CanHandle(ActionLine actionLine)
        {
            return actionLine.SpecialHandling.Contains("target_if=");
        }

        protected override ActionLine CheckHandling(ActionLine actionLine)
        {
            var targetIfValue = actionLine.SpecialHandling.Replace("target_if=", "").Trim();
            var modifiedCondition = actionLine.Condition;

            // Handle the targetIfValue as needed
            switch (targetIfValue)
            {
                case "refreshable":
                    modifiedCondition = "refreshable&" + actionLine.Condition;
                    break;
                    // Add more cases as needed.
            }

            return new ActionLine(actionLine.ListName, actionLine.Action, modifiedCondition, actionLine.SpecialHandling);
        }
    }
}
