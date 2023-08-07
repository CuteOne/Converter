using SimcToBrConverter.ActionHandlers;
using SimcToBrConverter.Conditions;
using SimcToBrConverter.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

            var convertedCondition = ConvertCondition(condition);

            return GenerateLuaCode(listName, command, convertedCondition, action);
        }

        protected abstract (string command, string condition) ParseAction(string action);

        private string ConvertCondition(string condition)
        {
            foreach (var converter in _conditionConverters)
            {
                if (converter.CanConvert(condition))
                {
                    return $" and ({converter.Convert(condition)})";
                }
            }

            return "";
        }

        protected virtual string GenerateLuaCode(string listName, string command, string convertedCondition, string action)
        {
            var formattedCommand = StringUtilities.ConvertToCamelCase(command);
            var debugCommand = StringUtilities.ConvertToTitleCase(command);

            var output = new StringBuilder();
            output.AppendLine($"    -- {debugCommand}");
            output.AppendLine($"    -- {action}");
            output.AppendLine($"    if cast.able.{formattedCommand}(){convertedCondition} then");
            output.AppendLine($"        if cast.{formattedCommand}() then ui.debug(\"Casting {debugCommand} [{StringUtilities.ConvertToTitleCase(listName)}]\") return true end");
            output.AppendLine("    end");

            return output.ToString();
        }
    }
}
