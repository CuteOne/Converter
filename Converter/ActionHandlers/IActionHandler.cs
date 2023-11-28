namespace SimcToBrConverter.ActionHandlers
{
    public interface IActionHandler
    {
        bool CanHandle();
        void Handle();
    }
}
