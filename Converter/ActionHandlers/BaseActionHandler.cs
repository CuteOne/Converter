using SimcToBrConverter.ActionLines;

namespace SimcToBrConverter.ActionHandlers
{
    public abstract class BaseActionHandler : IActionHandler
    {
        protected BaseActionHandler() {}

        public virtual bool CanHandle(ActionLine actionLine)
        {
            // Default behavior: always return true
            return true;
        }

        public ActionLine Handle(ActionLine actionLine)
        {
            // Parse the action
            return CheckHandling(actionLine);
        }

        protected abstract ActionLine CheckHandling(ActionLine actionLine);
    }
}
