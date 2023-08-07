using Converter.ActionHandlers;
using SimcToBrConverter.Conditions;
using SimcToBrConverter.Utilities;
using System.Text.RegularExpressions;
using System.Text;

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

    protected override string GenerateLuaCode(string listName, string command, string convertedCondition, string action, List<string> notConvertedConditions, string originalCondition)
    {
        var formattedCommand = StringUtilities.ConvertToCamelCase(command);
        var debugCommand = StringUtilities.ConvertToTitleCase(command);

        var output = new StringBuilder();
        output.AppendLine($"    -- {debugCommand}");
        output.AppendLine($"    -- {action}");

        if (notConvertedConditions.Any())
        {
            output.AppendLine($"    -- TODO: The following conditions were not converted:");
            foreach (var notConvertedCondition in notConvertedConditions)
            {
                output.AppendLine($"    -- {notConvertedCondition}");
            }
        }

        if (action.Contains("min:") || action.Contains("max:"))
        {
            output.AppendLine($"    if cast.able.{formattedCommand}(PLACEHOLDER){convertedCondition} then");
            output.AppendLine($"        if cast.{formattedCommand}(PLACEHOLDER) then ui.debug(\"Casting {debugCommand} [{StringUtilities.ConvertToTitleCase(listName)}]\") return true end");
            output.AppendLine("    end");
        }
        else
        {
            output.AppendLine($"    for i = 1, #enemies.PLACEHOLDER_RANGE do");
            output.AppendLine($"        local thisUnit = enemies.PLACEHOLDER_RANGE[i]");
            output.AppendLine($"        if cast.able.{formattedCommand}(thisUnit){convertedCondition} then");
            output.AppendLine($"            if cast.{formattedCommand}(thisUnit) then ui.debug(\"Casting {debugCommand} [{StringUtilities.ConvertToTitleCase(listName)}]\") return true end");
            output.AppendLine("        end");
            output.AppendLine("    end");
        }

        return output.ToString();
    }

}
