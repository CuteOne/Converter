using SimcToBrConverter.Conditions;
using System.Text;
using System.Text.RegularExpressions;
using static SimcToBrConverter.ActionLineParser;

namespace SimcToBrConverter.ActionHandlers
{
    public abstract class BaseActionHandler : IActionHandler
    {
        protected readonly List<IConditionConverter> _conditionConverters;

        protected BaseActionHandler(List<IConditionConverter> conditionConverters)
        {
            _conditionConverters = conditionConverters;
        }

        public abstract bool CanHandle(ActionLine action);

        public string Handle(ActionLine actionLine)
        {
            if (string.IsNullOrEmpty(actionLine.Action))
            {
                // Handle error scenario here
                return string.Empty;
            }

            // Parse the action
            var parsedAction = ParseAction(actionLine.Action);

            // Convert the condition
            var (updatedActionLine, notConvertedConditions) = ConvertCondition(parsedAction);

            // Generate the Lua code
            return LuaCodeGenerator.GenerateActionLineLuaCode(parsedAction, updatedActionLine.Condition, notConvertedConditions);
        }


        protected abstract ActionLine ParseAction(string action);

        protected (ActionLine UpdatedActionLine, List<string> NotConvertedConditions) ConvertCondition(ActionLine parsedAction)
        {
            var notConvertedConditions = new List<string>();
            var convertedConditions = new StringBuilder();

            // Check if there are no conditions
            if (string.IsNullOrWhiteSpace(parsedAction.Condition))
            {
                return (parsedAction, notConvertedConditions);
            }

            // Split the condition string by the & and | symbols, and parentheses, keeping the delimiters
            var originalConditions = Regex.Split(parsedAction.Condition, @"([&|\(\)!])");

            for (int i = 0; i < originalConditions.Length; i++)
            {
                var trimmedCondition = originalConditions[i].Trim();

                switch (trimmedCondition)
                {
                    case "&":
                        convertedConditions.Append(" and ");
                        break;
                    case "|":
                        convertedConditions.Append(" or ");
                        break;
                    case "!":
                        convertedConditions.Append("not ");
                        break;
                    case "(":
                    case ")":
                        convertedConditions.Append(trimmedCondition);
                        break;
                    default:
                        var wasConverted = false;

                        foreach (var converter in _conditionConverters)
                        {
                            if (converter.CanConvert(trimmedCondition))
                            {
                                var (convertedPart, notConvertedParts) = converter.Convert(trimmedCondition, parsedAction.Action, _conditionConverters);
                                convertedConditions.Append(convertedPart);
                                notConvertedConditions.AddRange(notConvertedParts);
                                wasConverted = true;
                                break;
                            }
                        }

                        if (!wasConverted)
                        {
                            notConvertedConditions.Add(trimmedCondition);
                        }
                        break;
                }
            }

            // Create a new ActionLine with the converted condition
            var updatedAction = new ActionLine(parsedAction.ListName, parsedAction.Action, convertedConditions.ToString());

            return (updatedAction, notConvertedConditions);
        }
    }
}
