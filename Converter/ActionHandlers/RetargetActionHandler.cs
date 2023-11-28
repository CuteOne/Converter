using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.ActionHandlers
{
    internal class RetargetActionHandler : BaseActionHandler
    {
        public override bool CanHandle()
        {
            return Program.currentActionLine.Action.Contains("retarget");
        }

        public override void Handle()
        {
            Program.currentActionLine.Action = Program.currentActionLine.Action.Replace("retarget_","");
            Program.currentActionLine.Type = ActionType.Retarget;
        }
    }
}
