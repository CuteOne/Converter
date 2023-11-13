using SimcToBrConverter.ActionLines;

namespace SimcToBrConverter.ActionHandlers
{
    public class PoolAndWaitActionHandler : BaseActionHandler
    {
        public PoolAndWaitActionHandler() : base() { }

        public override bool CanHandle(ActionLine actionLine)
        {
            return actionLine.Action.Contains("pool_resource") || actionLine.Action.Contains("wait");
        }

        protected override ActionLine CheckHandling(ActionLine actionLine)
        {
            if (actionLine.Action.Contains("pool_resource"))
            {
                actionLine.Type = ActionType.Pool;
                // Extract the name of the action list from the SpecialHandling property
                //var actionListName = actionLine.SpecialHandling.Replace("name=", "").Trim();
                //actionLine.Action = $"actionList.{actionListName}";

                // Return a new ActionLine with the extracted action list name as the action
                return actionLine;
            }

            if (actionLine.Action.Contains("wait"))
            {
                actionLine.Type = ActionType.Wait;
                actionLine.Condition = $"{actionLine.SpecialHandling.Replace("sec=", "").Trim()}+=&{actionLine.Condition}";
                return actionLine;
            }

            // If the action doesn't match the expected patterns, return the original ActionLine
            return actionLine;
        }

    }
}