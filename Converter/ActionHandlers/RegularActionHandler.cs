using SimcToBrConverter.Conditions;
using SimcToBrConverter.Utilities;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SimcToBrConverter.ActionHandlers
{
    public class RegularActionHandler : IActionHandler
    {
        private readonly List<IConditionConverter> _conditionConverters;

        public RegularActionHandler(List<IConditionConverter> conditionConverters)
        {
            _conditionConverters = conditionConverters;
        }

        public bool CanHandle(string action)
        {
            return !action.StartsWith("use_item") && !action.Contains("target_if=");
        }

        public string Handle(string action, string listName)
        {
            var output = new StringBuilder();

            // Split the action into the command and condition parts
            var parts = action.Split(",if=");
            var command = parts[0];
            var condition = parts.Length > 1 ? parts[1] : "";

            var formattedCommand = StringUtilities.ConvertToCamelCase(command);
            var debugCommand = StringUtilities.ConvertToTitleCase(command);

            // Convert the condition
            var convertedCondition = "";
            foreach (var converter in _conditionConverters)
            {
                if (converter.CanConvert(condition))
                {
                    convertedCondition = converter.Convert(condition);
                    break;
                }
            }

            // Generate the Lua code
            output.AppendLine($"    -- {command}{(string.IsNullOrEmpty(condition) ? "" : ",if=" + condition)}");
            output.AppendLine($"    if cast.able.{formattedCommand}() and ({convertedCondition}) then");
            output.AppendLine($"        if cast.{formattedCommand}() then ui.debug(\"Casting {debugCommand} [{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(listName)}]\") return true end");
            output.AppendLine("    end");

            return output.ToString();
        }
    }
}