namespace SimcToBrConverter.ActionHandlers
{
    public interface IActionHandler
    {
        bool CanHandle(string action);
        string Handle(string listName, string action);
    }
}
