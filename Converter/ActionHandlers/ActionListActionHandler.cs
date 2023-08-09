using Converter.ActionHandlers;
using SimcToBrConverter.Conditions;
using System.Text.RegularExpressions;

public class ActionListActionHandler : BaseActionHandler
{
    public ActionListActionHandler(List<IConditionConverter> conditionConverters) : base(conditionConverters) { }

    public override bool CanHandle(string action)
    {
        return action.Contains("run_action_list") || action.Contains("call_action_list");
    }

    protected override (string command, string condition) ParseAction(string action)
    {
        /*var parts = action.Split(",if=");
        var command = parts[0];
        var condition = parts.Length > 1 ? parts[1] : string.Empty;*/
        var match = Regex.Match(action, @"(call|run)_action_list,name=(?<command>\w+),if=?(?<condition>.*)");
        var command = match.Groups["command"].Value;
        var condition = match.Groups["condition"].Value;

        return (command, condition);
    }

    protected override bool ActionListAction(string action)
    {
        return action.Contains("run_action_list") || action.Contains("call_action_list");
    }
}