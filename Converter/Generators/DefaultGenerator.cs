using Microsoft.VisualBasic;
using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter.Generators
{
    public class DefaultGenerator : BaseActionGenerator
    {
        public override bool CanGenerate(ActionType actionType)
        {
            return actionType == ActionType.Default;
        }

        public override string GenerateActionLineCode(ActionLine actionLine, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag)
        {
            convertedCondition = PrependConditions(convertedCondition);

            var output = new StringBuilder();

            output.AppendLine($"    if cast.able.{formattedCommand}(){convertedCondition} then");
            output.AppendLine($"        if cast.{formattedCommand}() then ui.debug(\"Casting {debugCommand}{listNameTag}\") return true end");
            output.AppendLine($"    end");

            return output.ToString();
        }
    }
}
