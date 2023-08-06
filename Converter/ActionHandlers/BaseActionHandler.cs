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
        protected readonly List<IConditionConverter> conditionConverters;

        protected BaseActionHandler(List<IConditionConverter> conditionConverters)
        {
            this.conditionConverters = conditionConverters;
        }

        public abstract bool CanHandle(string action);

        public string Handle(string action, string listName)
        {
            var match = Regex.Match(action, @"(?<command>\w+)(,if=(?<condition>.*))?");
            var command = match.Groups["command"].Value;
            var condition = match.Groups["condition"].Value;

            var formattedCommand = StringUtilities.ConvertToCamelCase(command);

            return GenerateLuaCode(formattedCommand, condition, listName);
        }

        protected abstract string GenerateLuaCode(string command, string condition, string listName);
    }
}
