using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.ActionHandlers
{
    public class ActionListActionHandler : BaseActionHandler
    {
        public ActionListActionHandler() : base() { }

        public override bool CanHandle()
        {
            return Program.currentActionLine.Action.Contains("run_action_list") || Program.currentActionLine.Action.Contains("call_action_list");
        }

        public override void Handle()
        {
            if (Program.currentActionLine.Action.Contains("run_action_list") || Program.currentActionLine.Action.Contains("call_action_list"))
            {
                Program.currentActionLine.Type = ActionType.ActionList;
                // Extract the name of the action list from the SpecialHandling property
                var actionListName = Program.currentActionLine.SpecialHandling.Replace("name=", "").Trim();
                Program.currentActionLine.Action = $"actionList.{actionListName}";
            }
        }

    }
}