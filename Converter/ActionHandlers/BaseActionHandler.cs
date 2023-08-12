using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Conditions;
using System.Text;
using System.Text.RegularExpressions;

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
            var handledActionLine = CheckHandling(actionLine);

            // Convert the condition
            var (updatedActionLine, notConvertedConditions) = ConvertCondition(handledActionLine);

            // Generate the Lua code
            return LuaCodeGenerator.GenerateActionLineLuaCode(handledActionLine, updatedActionLine.Condition, notConvertedConditions);
        }


        protected abstract ActionLine CheckHandling(ActionLine actionLine);

        protected (ActionLine UpdatedActionLine, List<string> NotConvertedConditions) ConvertCondition(ActionLine parsedAction)
        {
            var notConvertedConditions = new List<string>();
            var convertedConditions = new StringBuilder();

            // Check if there are no conditions
            if (string.IsNullOrWhiteSpace(parsedAction.Condition))
            {
                return (parsedAction, notConvertedConditions);
            }

            // Use the utility to split the condition string
            var originalConditions = ConditionConverterUtility.SplitCondition(parsedAction.Condition);

            foreach (var conditionPart in originalConditions)
            {
                var (convertedPart, wasConverted, notConvertedParts) = ConditionConverterUtility.HandleConditionPart(conditionPart, parsedAction, _conditionConverters);

                if (wasConverted)
                {
                    convertedConditions.Append(convertedPart);
                }
                else
                {
                    notConvertedConditions.AddRange(notConvertedParts);
                }
            }

            // Create a new ActionLine with the converted condition
            var updatedAction = new ActionLine(parsedAction.ListName, parsedAction.Action, parsedAction.SpecialHandling, convertedConditions.ToString());

            return (updatedAction, notConvertedConditions);
        }

    }
}
