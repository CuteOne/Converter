using static SimcToBrConverter.ActionLineParser;

namespace SimcToBrConverter.ActionHandlers
{
    public interface IActionHandler
    {
        bool CanHandle(ActionLine actionLine);
        string Handle(ActionLine actionLine);
    }
}
