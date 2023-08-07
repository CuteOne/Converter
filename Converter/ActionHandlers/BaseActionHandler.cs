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

            var (originalCondition, convertedCondition, wasConverted) = ConvertCondition(condition);

            return GenerateLuaCode(listName, command, convertedCondition, action, wasConverted, originalCondition);
        }

        protected abstract (string command, string condition) ParseAction(string action);

        protected (string OriginalCondition, string ConvertedCondition, bool WasConverted) ConvertCondition(string condition)
        {
            foreach (var converter in _conditionConverters)
            {
                if (converter.CanConvert(condition))
                {
                    return (condition, $" and ({converter.Convert(condition)})", true);
                }
            }
            if (condition == string.Empty)
            {
                return (condition, condition, true);
            }
            else
            {
                return (condition, condition, false);
            }
        }

        protected virtual string GenerateLuaCode(string listName, string command, string convertedCondition, string action, bool wasConverted, string originalCondition)
        {
            var formattedCommand = StringUtilities.ConvertToCamelCase(command);
            var debugCommand = StringUtilities.ConvertToTitleCase(command);

            var output = new StringBuilder();
            output.AppendLine($"    -- {debugCommand}");
            output.AppendLine($"    -- {action}");
            if (wasConverted)
            {
                output.AppendLine($"    if cast.able.{formattedCommand}(){convertedCondition} then");
                output.AppendLine($"        if cast.{formattedCommand}() then ui.debug(\"Casting {debugCommand} [{StringUtilities.ConvertToTitleCase(listName)}]\") return true end");
                output.AppendLine("    end");
            } 
            else
            {
                output.AppendLine($"    -- TODO: Condition '{originalCondition}' was not converted.");
            }

            return output.ToString();
        }
    }
}
