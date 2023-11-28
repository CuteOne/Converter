using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter.LuaGenerators
{
    public class ModuleLuaGenerator : BaseLuaGenerator
    {
        public override bool CanGenerate(ConversionResult conversionResult)
        {
            return conversionResult.ActionLine.Type == ActionType.Module;
        }

        public override string GenerateActionLineCode(ConversionResult conversionResult, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag)
        {
            var output = new StringBuilder();

            // Remove spaces from the debug command
            debugCommand = debugCommand.Replace(" ", "");

            // Generate module code
            output.AppendLine($"    module.{debugCommand}()");

            return output.ToString();
        }
    }
}
