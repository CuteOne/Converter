using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter.LuaGenerators
{
    public abstract class BaseLuaGenerator : ILuaGenerator
    {
        public abstract bool CanGenerate(ConversionResult conversionResult);
        public abstract string GenerateActionLineCode(ConversionResult conversionResult, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag);
        public static string GenerateCommentsLineCode(ConversionResult conversionResult)
        {
            // Determine the action type
            string actionTag = "";
            switch (conversionResult.ActionLine.Type)
            {
                case ActionType.ActionList:
                    actionTag = "Call Action List - ";
                    break;
                case ActionType.Module:
                    actionTag = "Module - ";
                    break;
                case ActionType.UseItem:
                    actionTag = "Use Item - ";
                    break;
                case ActionType.Variable:
                    actionTag = "Variable - ";
                    break;
                default:
                    break;
            }

            string debugCommand = "";
            if (conversionResult.ActionLine.SpecialHandling.Contains("name="))
                debugCommand = StringUtilities.ConvertToTitleCase(conversionResult.ActionLine.SpecialHandling.Replace("name=",""));
            if (string.IsNullOrEmpty(debugCommand))
                debugCommand = StringUtilities.ConvertToTitleCase(conversionResult.ActionLine.Action);
            var output = new StringBuilder();
            if (!conversionResult.ActionLine.SpecialHandling.Contains("for_next=1"))
            {
                output.AppendLine($"    -- {actionTag}{debugCommand}"); // Append the action type and debug command to the output
                output.AppendLine($"    -- {conversionResult.ActionLine.Comment}");
                if (conversionResult.NotConvertedConditions.Any())
                {
                    output.AppendLine($"    -- TODO: The following conditions were not converted:");
                    foreach (var condition in conversionResult.NotConvertedConditions)
                    {
                        if (!string.IsNullOrEmpty(condition))
                        {
                            output.AppendLine($"    -- {condition}");
                        }
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
