using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter.LuaGenerators
{
    public class VariableLuaGenerator : BaseLuaGenerator
    {
        public override bool CanGenerate(ConversionResult conversionResult)
        {
            return conversionResult.ActionLine.Type == ActionType.Variable;
        }

        public override string GenerateActionLineCode(ConversionResult conversionResult, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag)
        {
            var output = new StringBuilder();

            if (!string.IsNullOrEmpty(conversionResult.ActionLine.Condition))
            {
                output.AppendLine($"    if {convertedCondition} then");
                output.AppendLine($"        {formattedCommand} = {conversionResult.ActionLine.Value}");
                output.AppendLine($"    end");
            }
            else
            {
                if (string.IsNullOrEmpty(conversionResult.ActionLine.Value))
                {
                    output.AppendLine($"    {formattedCommand} = false -- TODO: Optional replace with UI option, review SimC for detailed notes");
                }
                else
                    output.AppendLine($"    {formattedCommand} = {conversionResult.ActionLine.Value}");
            }

            return output.ToString();
        }
    }
}
