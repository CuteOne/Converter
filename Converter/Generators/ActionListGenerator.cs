using SimcToBrConverter.ActionLines;
using System.Text;

namespace SimcToBrConverter.Generators
{
    public class ActionListGenerator : BaseActionGenerator
    {
        public override bool CanGenerate(ActionType actionType)
        {
            return actionType == ActionType.ActionList;
        }
        public override string GenerateActionLineCode(ActionLine actionLine, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag)
        {
            var output = new StringBuilder();

            // Remove spaces from the debug command
            debugCommand = debugCommand.Replace("Actionlist.", "");
            debugCommand = debugCommand.Replace(" ", "");

            if (string.IsNullOrEmpty(actionLine.Condition))
                output.AppendLine($"    if actionList.{debugCommand}() then return true end");
            else
            {
                output.AppendLine($"    if {actionLine.Condition} then");
                output.AppendLine($"        if actionList.{debugCommand}() then return true end");
                output.AppendLine($"    end");
            }

            return output.ToString();
        }
    }
}
