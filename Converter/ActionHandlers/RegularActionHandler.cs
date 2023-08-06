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
            var match = Regex.Match(action, @"(?<command>\w+)(,if=(?<condition>.*))?");
            var command = match.Groups["command"].Value;
            var condition = match.Groups["condition"].Value;

            string formattedCommand = StringUtilities.ConvertToCamelCase(command);
            string debugCommand = StringUtilities.ConvertToTitleCase(command);

            string luaCondition = "";

            if (!string.IsNullOrEmpty(condition))
            {
                foreach (var converter in _conditionConverters)
                {
                    if (converter.CanConvert(condition))
                    {
                        luaCondition = converter.Convert(condition);
                        break;
                    }
                }
            }

            StringBuilder output = new StringBuilder();

            output.AppendLine($"    -- {action}");
            if (!string.IsNullOrEmpty(luaCondition))
            {
                output.AppendLine($"    if cast.able.{formattedCommand}() and ({luaCondition}) then");
            }
            else
            {
                output.AppendLine($"    if cast.able.{formattedCommand}() then");
            }
            output.AppendLine($"        if cast.{formattedCommand}() then ui.debug(\"Casting {debugCommand} [{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(listName)}]\") return true end");
            output.AppendLine("    end");

            return output.ToString();
        }
    }
}