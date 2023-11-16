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

            if (!actionLine.SpecialHandling.Contains("for_next=1"))
            { 
                output.AppendLine($"    if {convertedCondition} then");
                output.AppendLine($"        return true");
                output.AppendLine($"    end");    
            }

            return output.ToString();
        }
    }
}
