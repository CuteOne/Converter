using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter.LuaGenerators
{
    public class ActionListLuaGenerator : BaseLuaGenerator
    {
        public override bool CanGenerate(ConversionResult conversionResult)
        {
            return conversionResult.ActionLine.Type == ActionType.ActionList;
        }
        public override string GenerateActionLineCode(ConversionResult conversionResult, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag)
        {
            var output = new StringBuilder();

            // Remove spaces from the debug command
            debugCommand = conversionResult.ActionLine.SpecialHandling.Replace("name=","");
            debugCommand = debugCommand.Replace(" ", "");
            debugCommand = StringUtilities.ConvertToTitleCaseNoSpace(debugCommand);

            if (string.IsNullOrEmpty(conversionResult.ActionLine.Condition))
                output.AppendLine($"    if actionList.{debugCommand}() then return true end");
            else
            {
                output.AppendLine($"    if {conversionResult.ActionLine.Condition} then");
                output.AppendLine($"        if actionList.{debugCommand}() then return true end");
                output.AppendLine($"    end");
            }

            return output.ToString();
        }
    }
}
