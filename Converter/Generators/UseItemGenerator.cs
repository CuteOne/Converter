using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Conditions;
using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter.Generators
{
    public class UseItemGenerator : BaseActionGenerator
    {
        public override bool CanGenerate(ActionType actionType)
        {
            return actionType == ActionType.UseItem;
        }

        public override string GenerateActionLineCode(ActionLine actionLine, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag)
        {
            convertedCondition = PrependConditions(convertedCondition);

            var output = new StringBuilder();

            output.AppendLine($"    if use.able.{formattedCommand}(){convertedCondition} then");
            output.AppendLine($"        if use.{formattedCommand}() then ui.debug(\"Using {debugCommand}{listNameTag}\") return true end");
            output.AppendLine($"    end");

            SpellRepository.AddSpell(formattedCommand, "items");

            return output.ToString();
        }
    }
}
