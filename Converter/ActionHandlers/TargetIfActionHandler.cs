using Converter.ActionHandlers;
using SimcToBrConverter.Conditions;
using System.Text.RegularExpressions;

public class TargetIfActionHandler : BaseActionHandler
{
    public TargetIfActionHandler(List<IConditionConverter> conditionConverters) : base(conditionConverters) { }

    public override bool CanHandle(string action)
    {
        return action.Contains("target_if=");
    }

    protected override (string command, string condition) ParseAction(string action)
    {
        var match = Regex.Match(action, @"(?<command>\w+),target_if=(?<targetIfCondition>.*),if=(?<condition>.*)");
        var command = match.Groups["command"].Value;
        var condition = match.Groups["condition"].Value;

        return (command, condition);
    }

    protected override bool UseLoopAction(string action)
    {
        return !(action.Contains("min:") || action.Contains("max:"));
    }
}
