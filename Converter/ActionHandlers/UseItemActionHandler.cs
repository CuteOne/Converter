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
            var match = Regex.Match(action, @"use_item,name=(?<itemName>\w+),?(?<condition>.*)");
            var itemName = match.Groups["itemName"].Value;
            var condition = match.Groups["condition"].Value;

            string formattedItemName = StringUtilities.ConvertToCamelCase(itemName);
            string debugItemName = StringUtilities.ConvertToTitleCase(itemName);

            string luaCondition = "";

            if (!string.IsNullOrEmpty(condition))
            {
                foreach (var converter in _conditionConverters)
                {
                    if (converter.CanConvert(condition))
                    {
                        luaCondition = $" and ({converter.Convert(condition)})";
                        break;
                    }
                }
            }

            StringBuilder output = new StringBuilder();

            output.AppendLine($"    -- {action}");
            output.AppendLine($"    if cast.able.{formattedItemName}(PLACEHOLDER){luaCondition} then");
            output.AppendLine($"        if cast.{formattedItemName}(PLACEHOLDER) then ui.debug(\"Using {debugItemName} [{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(listName)}]\") return true end");
            output.AppendLine("    end");

            return output.ToString();
        }
    }
}
