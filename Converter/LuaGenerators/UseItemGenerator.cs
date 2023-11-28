using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter.LuaGenerators
{
    public class UseItemLuaGenerator : BaseLuaGenerator
    {
        public override bool CanGenerate(ConversionResult conversionResult)
        {
            return conversionResult.ActionLine.Type == ActionType.UseItem;
        }

        public override string GenerateActionLineCode(ConversionResult conversionResult, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag)
        {
            convertedCondition = PrependConditions(convertedCondition);

            var output = new StringBuilder();
            var value = "";
            if (conversionResult.ActionLine.SpecialHandling.Contains("slots="))
                value = conversionResult.ActionLine.Op;

            output.AppendLine($"    if use.able.{formattedCommand}({value}){convertedCondition} then");
            output.AppendLine($"        if use.{formattedCommand}({value}) then ui.debug(\"Using {debugCommand}{listNameTag}\") return true end");
            output.AppendLine($"    end");

            SpellRepository.AddSpell(formattedCommand, "items");

            return output.ToString();
        }
    }
}
