using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.ActionHandlers
{
    public class PoolActionHandler : BaseActionHandler
    {
        public PoolActionHandler() : base() { }

        public override bool CanHandle()
        {
            return Program.currentActionLine.Action.Contains("pool_resource");
        }

        public override void Handle()
        {
            Program.currentActionLine.Type = ActionType.Pool;
        }

    }
}