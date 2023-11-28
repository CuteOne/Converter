using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter.LuaGenerators
{
    public class PoolLuaGenerator : BaseLuaGenerator
    {
        public override bool CanGenerate(ConversionResult conversionResult)
        {
            return conversionResult.ActionLine.Type == ActionType.Pool;
        }

        public override string GenerateActionLineCode(ConversionResult conversionResult, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag)
        {
            var output = new StringBuilder();

            if (!conversionResult.ActionLine.SpecialHandling.Contains("for_next=1"))
            { 
                output.AppendLine($"    if {convertedCondition} then");
                output.AppendLine($"        return true");
                output.AppendLine($"    end");    
            }

            return output.ToString();
        }
    }
}
