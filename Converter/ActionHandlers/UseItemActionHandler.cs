using Converter.ActionHandlers;
using SimcToBrConverter.Conditions;
using System.Text.RegularExpressions;

namespace SimcToBrConverter.ActionHandlers
{
    public class UseItemActionHandler : BaseActionHandler
    {
        public UseItemActionHandler(List<IConditionConverter> conditionConverters) : base(conditionConverters) { }

        public override bool CanHandle(string action)
        {
            return action.Contains("use_item");
        }

        protected override (string command, string condition) ParseAction(string action)
        {
            var match = Regex.Match(action, @"use_item,name=(?<command>\w+),?(?<condition>.*)");
            var command = match.Groups["command"].Value;
            var condition = match.Groups["condition"].Value;

            return (command, condition);
        }
    }
}
