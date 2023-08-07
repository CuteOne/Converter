using SimcToBrConverter.ActionHandlers;
using SimcToBrConverter.Conditions;
using SimcToBrConverter.Utilities;
using System.Text;

namespace Converter.ActionHandlers
{
    public abstract class BaseActionHandler : IActionHandler
    {
        protected readonly List<IConditionConverter> _conditionConverters;

        protected BaseActionHandler(List<IConditionConverter> conditionConverters)
        {
            _conditionConverters = conditionConverters;
        }

        public abstract bool CanHandle(string action);

        public string Handle(string listName, string action)
        {
            var (command, condition) = ParseAction(action);

            var (originalCondition, convertedCondition, notConvertedConditions) = ConvertCondition(condition);

            return GenerateLuaCode(listName, command, convertedCondition, action, notConvertedConditions, originalCondition);
        }


        protected abstract (string command, string condition) ParseAction(string action);

        protected (string OriginalCondition, string ConvertedCondition, List<string> NotConvertedConditions) ConvertCondition(string condition)
        {
            var notConvertedConditions = new List<string>();
            var convertedConditions = new StringBuilder();
            var originalConditions = condition.Split(new[] { "&", "|", "!" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var originalCondition in originalConditions)
            {
                var trimmedCondition = originalCondition.Trim();
                var wasConverted = false;

                foreach (var converter in _conditionConverters)
                {
                    if (converter.CanConvert(trimmedCondition))
                    {
                        convertedConditions.Append($" and ({converter.Convert(trimmedCondition)})");
                        wasConverted = true;
                        break;
                    }
                }

                if (!wasConverted)
                {
                    notConvertedConditions.Add(trimmedCondition);
                }
            }

            return (condition, convertedConditions.ToString(), notConvertedConditions);
        }

        protected virtual bool UseLoopAction(string action)
        {
            return false; // Default behavior in the base class
        }


        protected virtual string GenerateLuaCode(string listName, string command, string convertedCondition, string action, List<string> notConvertedConditions, string originalCondition)
        {
            var formattedCommand = StringUtilities.ConvertToCamelCase(command);
            var debugCommand = StringUtilities.ConvertToTitleCase(command);

            var output = new StringBuilder();
            output.AppendLine($"    -- {debugCommand}");
            output.AppendLine($"    -- {action}");

            if (notConvertedConditions.Any())
            {
                output.AppendLine($"    -- TODO: The following conditions were not converted:");
                foreach (var notConvertedCondition in notConvertedConditions)
                {
                    output.AppendLine($"    -- {notConvertedCondition}");
                }
            }

            if (UseLoopAction(action))
            {
                output.AppendLine($"    for i = 1, #enemies.PLACEHOLDER_RANGE do");
                output.AppendLine($"        local thisUnit = enemies.PLACEHOLDER_RANGE[i]");
                output.AppendLine($"        if cast.able.{formattedCommand}(thisUnit){convertedCondition} then");
                output.AppendLine($"            if cast.{formattedCommand}(thisUnit) then ui.debug(\"Casting {debugCommand} [{StringUtilities.ConvertToTitleCase(listName)}]\") return true end");
                output.AppendLine("        end");
                output.AppendLine("    end");
            }
            else
            {
                output.AppendLine($"    if cast.able.{formattedCommand}(){convertedCondition} then");
                output.AppendLine($"        if cast.{formattedCommand}() then ui.debug(\"Casting {debugCommand} [{StringUtilities.ConvertToTitleCase(listName)}]\") return true end");
                output.AppendLine("    end");
            }

            return output.ToString();
        }

    }
}
