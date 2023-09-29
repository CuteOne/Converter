using SimcToBrConverter.ActionLines;

namespace SimcToBrConverter.Conditions
{
    public interface IConditionConverter
    {
        // Determines if the converter can handle the given condition part
        string? CanConvert(string conditionPart);

        // Converts a specific condition part, using the entire ActionLine for context
        (string ConvertedConditionPart, List<string> NotConvertedParts) ConvertPart(string conditionPart, string action);

        // Convert specific tasks related to conditions
        (string Result, bool Negate, bool Converted) ConvertTask(string conditionType, string spell, string task, string command);
    }
}
