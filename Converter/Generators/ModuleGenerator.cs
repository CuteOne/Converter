using SimcToBrConverter.ActionLines;
using System.Text;

namespace SimcToBrConverter.Generators
{
    public class ModuleGenerator : BaseActionGenerator
    {
        public override bool CanGenerate(ActionType actionType)
        {
            return actionType == ActionType.Module;
        }

        public override string GenerateActionLineCode(ActionLine actionLine, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag)
        {
            var output = new StringBuilder();

            // Remove spaces from the debug command
            debugCommand = debugCommand.Replace(" ", "");

            // Generate module code
            output.AppendLine($" module.{debugCommand}()");

            return output.ToString();
        }
    }
}
