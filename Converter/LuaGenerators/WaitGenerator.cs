using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter.LuaGenerators
{
    public class WaitLuaGenerator : BaseLuaGenerator
    {
        public override bool CanGenerate(ConversionResult conversionResult)
        {
            return conversionResult.ActionLine.Type == ActionType.Wait;
        }

        public override string GenerateActionLineCode(ConversionResult conversionResult, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag)
        {
            var output = new StringBuilder();

            string[] splitConditions = convertedCondition.Split("+== and ");
            string waitFor = splitConditions[0];
            string waitForDebug = StringUtilities.ConvertToTitleCase(conversionResult.ActionLine.SpecialHandling);
            convertedCondition = splitConditions[1];
            output.AppendLine($"    if {convertedCondition} then");
            output.AppendLine($"        local waitFor = {waitFor}");
            output.AppendLine($"        if cast.wait(waitFor, function() return true end) then ui.debug(\"Waiting for {waitForDebug}\") return false end");
            output.AppendLine($"    end");

            return output.ToString();
        }
    }
}
