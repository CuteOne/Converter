using SimcToBrConverter.ActionLines;

namespace SimcToBrConverter.ActionHandlers
{
    public interface IActionHandler
    {
        bool CanHandle(ActionLine actionLine);
        ActionLine Handle(ActionLine actionLine);
    }
}
