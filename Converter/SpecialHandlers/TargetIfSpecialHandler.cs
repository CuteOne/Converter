using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.SpecialHandlers
{
    public class TargetIfSpecialHandler : BaseSpecialHandler
    {
        public TargetIfSpecialHandler() : base() { }

        public override bool CanHandle()
        {
            return Program.currentActionLine.SpecialHandling.Contains("target_if=");
        }

        public override void Handle()
        {
            List<string> specialHandling = SplitSpecialHandling();

            foreach (var entry in specialHandling)
            {
                if (entry.Contains("target_if="))
                {
                    var targetIfValue = entry["target_if=".Length..].Trim();
                    if (!targetIfValue.Contains("max:") && !targetIfValue.Contains("min:"))
                    {
                        Program.currentActionLine.TypeSpecial = ActionType.Loop;
                        ModifyConditions.Add(Program.currentActionLine, targetIfValue);
                    }
                    else if (targetIfValue.Contains("max:") || targetIfValue.Contains("min:"))
                    {
                        string maxMin;
                        if (targetIfValue.Contains("max:"))
                        {
                            maxMin = targetIfValue["max:".Length..].Trim();
                            Program.currentActionLine.TypeSpecial = ActionType.Max;
                        }
                        else
                        {
                            maxMin = targetIfValue["min:".Length..].Trim();
                            Program.currentActionLine.TypeSpecial = ActionType.Min;
                        }
                        ConditionConversionService conditionConversionService = Program.conditionConversionService;
                        ActionLine specialActionLine = new("",Program.currentActionLine.Action,"", "", "", maxMin);
                        (_, _) = conditionConversionService.ConvertCondition(specialActionLine);
                        Program.currentActionLine.ConvertedSpecial = specialActionLine.Condition;
                    }
                    else
                    {
                        Program.currentActionLine.Comment = $"{Program.currentActionLine.Comment}\n    -- TODO: Handle {entry}";
                    }
                }
            }
        }
    }
}
