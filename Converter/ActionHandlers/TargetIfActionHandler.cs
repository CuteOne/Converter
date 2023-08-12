using Converter.ActionHandlers;
using SimcToBrConverter;
using SimcToBrConverter.Conditions;
using System.Text.RegularExpressions;
using static SimcToBrConverter.ActionLineParser;

namespace SimcToBrConverter.ActionHandlers
{
    public class TargetIfActionHandler : BaseActionHandler
    {
        public TargetIfActionHandler(List<IConditionConverter> conditionConverters) : base(conditionConverters) { }

        public override bool CanHandle(ActionLine actionLine)
        {
            return actionLine.SpecialHandling.Contains("target_if=");
        }

        protected override ActionLine ParseAction(string action)
        {
            var parsedAction = ActionLineParser.ParseActionLine(action);

            if (parsedAction.SpecialHandling.Contains("target_if="))
            {
                var targetIfValue = parsedAction.SpecialHandling.Replace("target_if=", "").Trim();
                string newCondition = parsedAction.Condition;

                // Handle the targetIfValue as needed
                // For example, if targetIfValue is "refreshable", you can modify the newCondition.
                if (targetIfValue == "refreshable")
                {
                    newCondition = "refreshable&" + parsedAction.Condition;
                }
                // Add more handling logic for other possible values of targetIfValue if needed.

                return new ActionLine(parsedAction.ListName, parsedAction.Action, newCondition, parsedAction.SpecialHandling);
            }

            return parsedAction;
        }


        /*protected override bool UseLoopAction(string action)
        {
            return !(action.Contains("min:") || action.Contains("max:"));
        }*/
    }
}
