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
        string command;
        string condition;
        string additionalCondition = "";
        Match match;

        // Check if the action contains ",if="
        if (action.Contains(",if="))
        {
            // If ",if=" is present, use the original pattern
            match = Regex.Match(action, @"(?<command>\w+),target_if=(?<targetIfCondition>.*),if=(?<condition>.*)");
            string targetIfCondition = match.Groups["targetIfCondition"].Value;
            if (targetIfCondition == "refreshable")
                additionalCondition = targetIfCondition;
        }
        else
        {
            // If ",if=" is not present, use a different pattern that treats everything after "target_if=" as the condition
            match = Regex.Match(action, @"(?<command>\w+),target_if=(?<condition>.*)");

        }
        command = match.Groups["command"].Value;
        if (additionalCondition != "")
            condition = additionalCondition + "&" + match.Groups["condition"].Value;
        else
            condition = match.Groups["condition"].Value;

        return (command, condition);
    }


    protected override bool UseLoopAction(string action)
    {
        return !(action.Contains("min:") || action.Contains("max:"));
    }
}
