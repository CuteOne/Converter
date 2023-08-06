namespace SimcToBrConverter.Conditions
{
    public interface IConditionConverter
    {
        bool CanConvert(string condition);
        public abstract string Convert(string condition);
    }
}
