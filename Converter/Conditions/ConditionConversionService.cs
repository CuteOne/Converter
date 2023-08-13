using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Conditions;
using System.Text;

public class ConditionConversionService
{
    private readonly List<IConditionConverter> _conditionConverters;

    public ConditionConversionService(List<IConditionConverter> conditionConverters)
    {
        _conditionConverters = conditionConverters;
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
        var finalConvertedCondition = ConditionConverterUtility.ConvertLogicalOperatorsToLua(convertedConditions.ToString());
        actionLine.Condition = finalConvertedCondition;

        return (actionLine, notConvertedConditions);
    }
}
