using SimcToBrConverter.ActionLines;

namespace SimcToBrConverter.Generators
{
    public interface IActionCodeGenerator
    {
        bool CanGenerate(ActionType actionType);
        string GenerateActionLineCode(ActionLine actionLine, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag);
    }
}
