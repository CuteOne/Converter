namespace SimcToBrConverter.Conditions
{
    public interface IConditionConverter
    {
        bool CanConvert(string condition);
        public abstract (string ConvertedCondition, List<string> NotConvertedParts) Convert(string condition, string command, List<IConditionConverter> conditionConverters);
        (string Result, bool Negate, bool Converted) ConvertTask(string spell, string task, string command);
    }
}
