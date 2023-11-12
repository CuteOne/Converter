using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Conditions;
using SimcToBrConverter.Utilities;
using System.Text;

public class ConditionConversionService
{
    private readonly List<IConditionConverter> _conditionConverters;
    public HashSet<string> Locals { get; private set; } = new HashSet<string>();

    public ConditionConversionService(List<IConditionConverter> conditionConverters)
    {
        _conditionConverters = conditionConverters;
    }
    private void AddToLocalList(string local)
    {
        // Remove the "not " prefix
        if (local.StartsWith("not "))
        {
            local = local.Substring(4);
        }

        // Remove any # characters
        local = local.Replace("#", "");

        // Trim whitespace
        local = local.Trim();

        if (!string.IsNullOrWhiteSpace(local))
        {
            Locals.Add(local);
        }
    }

    public (ActionLine UpdatedActionLine, List<string> NotConvertedConditions) ConvertCondition(ActionLine actionLine)
    {
        var notConvertedConditions = new List<string>();
        var convertedConditions = new StringBuilder();

        // Split the condition into parts
        var conditionParts = ConditionConverterUtility.SplitCondition(actionLine.Condition);

        foreach (var conditionPart in conditionParts)
        {
            // If the condition part is an operator, number, or parenthesis, append it directly
            if (ConditionConverterUtility.IsOperatorOrNumber(conditionPart))
            {
                convertedConditions.Append(conditionPart);
                continue;
            }

            bool wasConverted = false;

            foreach (var converter in _conditionConverters)
            {
                if (converter.CanConvert(conditionPart) != null)
                {
                    var (convertedPart, notConvertedParts) = converter.ConvertPart(conditionPart, actionLine.Action);
                    var local = convertedPart.Split('.')[0];
                    AddToLocalList(local);
                    convertedConditions.Append(convertedPart);
                    notConvertedConditions.AddRange(notConvertedParts);
                    wasConverted = true;
                    break; // Exit the loop once a converter has successfully converted the part
                }
            }

            if (!wasConverted)
            {
                notConvertedConditions.Add(conditionPart);
            }
        }

        // Convert logical operators to their Lua equivalents
        string checkConditions = StringUtilities.CheckForOr(convertedConditions.ToString());
        var finalConvertedCondition = ConditionConverterUtility.ConvertOperatorsToLua(checkConditions);
        actionLine.Condition = finalConvertedCondition;

        return (actionLine, notConvertedConditions);
    }
}
