using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.ActionHandlers
{
    internal class WaitActionHandler : BaseActionHandler
    {
        public override bool CanHandle()
        {
            return Program.currentActionLine.Action.Contains("wait");
        }

        public override void Handle()
        {
            Program.currentActionLine.Type = ActionType.Wait;
            Program.currentActionLine.Condition = $"{Program.currentActionLine.SpecialHandling.Replace("sec=", "").Trim()}+=&{Program.currentActionLine.Condition}";
        }
    }
}
