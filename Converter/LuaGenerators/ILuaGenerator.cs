using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.LuaGenerators
{
    public interface ILuaGenerator
    {
        bool CanGenerate(ConversionResult conversionResult);
        string GenerateActionLineCode(ConversionResult conversionResult, string formattedCommand, string debugCommand, string convertedCondition, string listNameTag);
    }
}
