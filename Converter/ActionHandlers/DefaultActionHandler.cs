using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.ActionHandlers
{

    /// <summary>
    /// DefaultActionHandler serves as a catch-all handler for actions that are not specifically
    /// handled by other specialized action handlers. While other handlers are designed to process
    /// specific types of actions based on certain criteria, the DefaultActionHandler ensures that
    /// any action not caught by these specialized handlers is still processed.
    /// 
    /// This handler is crucial for the system's flexibility and robustness. Without it, any action
    /// not explicitly defined in other handlers would be ignored, potentially leading to missing
    /// or incomplete output.
    /// 
    /// Note: Always ensure that DefaultActionHandler is the last handler in the list of action handlers.
    /// This ensures that it only processes actions that haven't been handled by other handlers.
    /// </summary>

    public class DefaultActionHandler : BaseActionHandler
    {
        public DefaultActionHandler() : base() { }

        public override bool CanHandle()
        {
            // Always return true, or implement specific logic if needed
            return true;
        }

        public override void Handle()
        {
            if (Program.currentActionLine.Action.Contains("pick_up_fragment"))
                Program.currentActionLine.Type = ActionType.Ignore;

            return;
        }
    }
}