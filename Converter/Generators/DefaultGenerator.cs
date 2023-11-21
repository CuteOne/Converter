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
            if (actionLine.PoolCondition.Contains("pool_if="))
            {
                var poolCondition = actionLine.PoolCondition.Replace("pool_if=", "").Trim();
                if (!string.IsNullOrEmpty(poolCondition))
                {
                    ConditionConversionService conditionConversionService = new(Program.GetConditionConverters());
                    ActionLine temp = new(actionLine.ListName, actionLine.Action, actionLine.SpecialHandling, poolCondition, actionLine.Comment, actionLine.Type);
                    (ActionLine poolActionLine, _) = conditionConversionService.ConvertCondition(temp);

                    output.AppendLine($"        if {poolActionLine.Condition} then");
                    output.AppendLine($"            if cast.pool.{formattedCommand}() then return true end");
                    output.AppendLine($"        end");
                }
                else
                    output.AppendLine($"            if cast.pool.{formattedCommand}() then return true end");
            }
            output.AppendLine($"        if cast.{formattedCommand}() then ui.debug(\"Casting {debugCommand}{listNameTag}\") return true end");
            output.AppendLine($"    end");

            SpellRepository.AddSpell(formattedCommand, "abilities");

            return output.ToString();
        }
    }
}
