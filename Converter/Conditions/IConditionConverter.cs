namespace SimcToBrConverter.Conditions
{
    public interface IConditionConverter
    {
        // Determines if the converter can handle the given condition part
        string? CanConvert(string conditionPart);

        // Converts the given condition part, using the entire ActionLine for context and the list of condition converters
        (string ConvertedConditionPart, List<string> NotConvertedParts) Convert(string conditionPart, string action, List<IConditionConverter> conditionConverters);

        // Convert specific tasks related to conditions
        (string Result, bool Negate, bool Converted) ConvertTask(string spell, string task, string command);
    }
}
