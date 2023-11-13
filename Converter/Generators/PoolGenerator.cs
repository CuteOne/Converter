using SimcToBrConverter.ActionLines;
using System.Text;

namespace SimcToBrConverter.Generators
{
    public class PoolGenerator : BaseActionGenerator
    {
        public override bool CanGenerate(ActionType actionType)
        {
            return actionType == ActionType.Pool;
        }

        public override string GenerateActionLineCode(ActionLine actionLine, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag)
        {
            var output = new StringBuilder();

            output.AppendLine($"    -- TODO: Implement Pool Generator Code");

            return output.ToString();
        }
    }
}
