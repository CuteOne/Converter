namespace SimcToBrConverter.SpecialHandlers
{
    internal interface ISpecialHandler
    {
        bool CanHandle();
        void Handle();
    }
}
