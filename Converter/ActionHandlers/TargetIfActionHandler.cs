using SimcToBrConverter.Conditions;
using SimcToBrConverter.Utilities;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SimcToBrConverter.ActionHandlers
{
    public class TargetIfActionHandler : IActionHandler
    {
        private readonly List<IConditionConverter> _conditionConverters;

        public TargetIfActionHandler(List<IConditionConverter> conditionConverters)
        {
            _conditionConverters = conditionConverters;
        }

        public bool CanHandle(string action)
        {
            return action.Contains("target_if=");
        }

        public string Handle(string listName, string action)
        {
            // Split the action into the command and condition parts
            var match = Regex.Match(action, @"(?<command>\w+),target_if=(?<targetIf>\w+),?(?<condition>.*)");
            var command = match.Groups["command"].Value;
            var targetIf = match.Groups["targetIf"].Value;
            var condition = match.Groups["condition"].Value;

            var formattedCommand = StringUtilities.ConvertToCamelCase(command);
            var debugCommand = StringUtilities.ConvertToTitleCase(command);

            // Convert the condition
            var convertedCondition = "";
            foreach (var converter in _conditionConverters)
            {
                if (converter.CanConvert(condition))
                {
                    convertedCondition = $" and ({converter.Convert(condition)})";
                    break;
                }
            }

            // Generate the Lua code
            StringBuilder output = new StringBuilder();

            output.AppendLine($"    -- {debugCommand}");
            output.AppendLine($"    -- {action}");

            if (targetIf.Contains("min:") || targetIf.Contains("max:"))
            {
                output.AppendLine($"    if cast.able.{formattedCommand}(PLACEHOLDER){convertedCondition} then");
                output.AppendLine($"        if cast.{formattedCommand}(PLACEHOLDER) then ui.debug(\"Casting {debugCommand} [{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(listName)}]\") return true end");
                output.AppendLine("    end");
            }
            else
            {
                output.AppendLine($"    for i = 1, #enemies.PLACEHOLDER_RANGE do");
                output.AppendLine($"        local thisUnit = enemies.PLACEHOLDER_RANGE[i]");
                output.AppendLine($"        if cast.able.{formattedCommand}(thisUnit){convertedCondition} then");
                output.AppendLine($"            if cast.{formattedCommand}(thisUnit) then ui.debug(\"Casting {debugCommand} [{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(listName)}]\") return true end");
                output.AppendLine("        end");
                output.AppendLine("    end");
            }

            return output.ToString();
        }
    }
}