using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter.LuaGenerators
{
    internal class MaxMinLuaGenerator : BaseLuaGenerator
    {
        public override bool CanGenerate(ConversionResult conversionResult)
        {
            return conversionResult.ActionLine.TypeSpecial == ActionType.Max || conversionResult.ActionLine.TypeSpecial == ActionType.Min;
        }

        private readonly List<string> minMaxGenerated = new();

        public override string GenerateActionLineCode(ConversionResult conversionResult, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag)
        {
            StringBuilder output;
            string maxMin = conversionResult.ActionLine.TypeSpecial == ActionType.Max ? "max" : "min";
            string greaterLesser = conversionResult.ActionLine.TypeSpecial == ActionType.Max ? ">" : "<";
            string greaterLesserValue = conversionResult.ActionLine.TypeSpecial == ActionType.Max ? "0" : "99999";
            string action = "PLACEHOLDER";
            conversionResult.ActionLine.ConvertedSpecial = conversionResult.ActionLine.ConvertedSpecial.Replace("PLACEHOLDER", "thisUnit");
            string convertedSpecial = conversionResult.ActionLine.ConvertedSpecial.Replace("(","").Replace(")","");

            if (!minMaxGenerated.Any(s => s.Contains(convertedSpecial, StringComparison.OrdinalIgnoreCase)))
            { 
                output = new StringBuilder();

                output.AppendLine($"    -- This is a very rough implementation, review needed and be sure to rename the var used where implemented");
                output.AppendLine($"    -- {conversionResult.ActionLine.SpecialHandling}");
                output.AppendLine($"    var.{maxMin}{action}={greaterLesserValue}");
                output.AppendLine($"    var.{maxMin}{action}Unit=\"target\"");
                output.AppendLine($"    for i=1,#enemies.yards0 do");
                output.AppendLine($"        local thisUnit=enemies.yards0[i]");
                output.AppendLine($"        local thisCondition={conversionResult.ActionLine.ConvertedSpecial}");
                output.AppendLine($"        if thisCondition{greaterLesser}var.{maxMin}{action} then");
                output.AppendLine($"            var.{maxMin}{action}=thisCondition");
                output.AppendLine($"            var.{maxMin}{action}Unit=thisUnit");
                output.AppendLine($"        end");
                output.AppendLine($"    end");
                output.AppendLine();

                Program.extraVars.Add(output.ToString());
                minMaxGenerated.Add(convertedSpecial);
            }

            //output = new StringBuilder();

            //convertedCondition = PrependConditions(convertedCondition);

            conversionResult.ActionLine.Condition = convertedCondition.Replace("PLACEHOLDER", $"var.{maxMin}{action}Unit");
            /*string castUse = conversionResult.ActionLine.Comment.Contains("use_item") ? "use" : "cast";

            output.AppendLine($"    if {castUse}.able.{formattedCommand}(var.{maxMin}{action}Unit){convertedCondition} then");
            output.AppendLine($"        if {castUse}.{formattedCommand}() then ui.debug(\"{char.ToUpper(castUse[0])+castUse[1..]}ing {debugCommand}{listNameTag}\") return true end");
            output.AppendLine($"    end");

            if (castUse.Equals("cast"))
                SpellRepository.AddSpell(formattedCommand, "abilities");
            else
                SpellRepository.AddSpell(formattedCommand, "items");*/

            return ""; //output.ToString();
        }
    }
}
