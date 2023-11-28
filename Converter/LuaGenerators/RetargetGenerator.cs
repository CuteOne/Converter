using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter.LuaGenerators
{
    public class RetargetLuaGenerator : BaseLuaGenerator
    {
        public override bool CanGenerate(ConversionResult conversionResult)
        {
            return conversionResult.ActionLine.Type == ActionType.Retarget;
        }

        public override string GenerateActionLineCode(ConversionResult conversionResult, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag)
        {
            var output = new StringBuilder();

            convertedCondition = PrependConditions(conversionResult.ActionLine.Condition);

            if (!string.IsNullOrEmpty(conversionResult.ActionLine.Condition))
            {
                output.AppendLine($"    if cast.able.{formattedCommand}(){convertedCondition} then");
            }
            else
                output.AppendLine($"    if {convertedCondition} then");
            output.AppendLine($"        unit.target(PLACEHOLDER)");
            output.AppendLine($"        if cast.{formattedCommand}() then ui.debug(\"Casting {debugCommand} with retarget{listNameTag}\") return true end");
            output.AppendLine($"    end");

            return output.ToString();
        }
    }
}
