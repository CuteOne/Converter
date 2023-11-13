using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter.Generators
{
    public class VariableGenerator : BaseActionGenerator
    {
        public override bool CanGenerate(ActionType actionType)
        {
            return actionType == ActionType.Variable;
        }

        public override string GenerateActionLineCode(ActionLine actionLine, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag)
        {
            var output = new StringBuilder();

            output.AppendLine($"    {StringUtilities.ConvertToCamelCase(actionLine.Action)} = {actionLine.Condition}");

            return output.ToString();
        }
    }
}
