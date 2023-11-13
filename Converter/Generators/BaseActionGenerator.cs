using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter.Generators
{
    public abstract class BaseActionGenerator : IActionCodeGenerator
    {
        public abstract bool CanGenerate(ActionType actionType);
        public abstract string GenerateActionLineCode(ActionLine actionLine, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag);
        public static string GenerateCommentsLineCode(ActionLine actionLine, List<string> notConvertedConditions)
        {
            // Determine the action type
            string actionTag = "";
            switch (actionLine.Type)
            {
                case ActionType.ActionList:
                    actionTag = "Action List - ";
                    actionLine.Action = actionLine.Action.Replace("actionList.", "");
                    break;
                case ActionType.Module:
                    actionTag = "Module - ";
                    actionLine.Action = actionLine.Action.Replace("module.", "");
                    break;
                case ActionType.UseItem:
                    actionTag = "Use Item - ";
                    actionLine.Action = actionLine.Action.Replace("use_item.", "");
                    break;
                case ActionType.Variable:
                    actionTag = "Variable - ";
                    actionLine.Action = actionLine.Action.Replace("var.", "");
                    break;
                default:
                    break;
            }

            var debugCommand = StringUtilities.ConvertToTitleCase(actionLine.Action);
            var output = new StringBuilder();

            output.AppendLine($"    -- {actionTag}{debugCommand}"); // Append the action type and debug command to the output
            output.AppendLine($"    -- {actionLine.Comment}");
            if (notConvertedConditions.Any())
            {
                output.AppendLine($"    -- TODO: The following conditions were not converted:");
                foreach (var condition in notConvertedConditions)
                {
                    if (!string.IsNullOrEmpty(condition))
                    {
                        output.AppendLine($"    -- {condition}");
                    }
                }
            }

            return output.ToString();
        }
        public static string PrependConditions(string convertedCondition)
        {
            return !string.IsNullOrEmpty(convertedCondition) ? " and " + convertedCondition : "";
        }
    }
}
