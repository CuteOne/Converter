namespace SimcToBrConverter.Conditions
{
    public interface IConditionConverter
    {
        bool CanConvert(string condition);
        public abstract string Convert(string condition, string command, List<IConditionConverter> conditionConverters);
        (string Result, bool Negate) ConvertTask(string spell, string task, string command);
    }
}
