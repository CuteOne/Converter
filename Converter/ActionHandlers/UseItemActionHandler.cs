using SimcToBrConverter.Conditions;
using SimcToBrConverter.Utilities;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SimcToBrConverter.ActionHandlers
{
    public class UseItemActionHandler : IActionHandler
    {
        private readonly List<IConditionConverter> _conditionConverters;

        public UseItemActionHandler(List<IConditionConverter> conditionConverters)
        {
            _conditionConverters = conditionConverters;
        }

        public bool CanHandle(string action)
        {
            return action.StartsWith("use_item");
        }

        public string Handle(string listName, string action)
        {
            // Split the action into the command and condition parts
            var match = Regex.Match(action, @"use_item,name=(?<itemName>\w+),?(?<condition>.*)");
            var command = match.Groups["itemName"].Value;
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
            output.AppendLine($"    if cast.able.{formattedCommand}(PLACEHOLDER){convertedCondition} then");
            output.AppendLine($"        if cast.{formattedCommand}(PLACEHOLDER) then ui.debug(\"Using {convertedCondition} [{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(listName)}]\") return true end");
            output.AppendLine("    end");

            return output.ToString();
        }
    }
}
