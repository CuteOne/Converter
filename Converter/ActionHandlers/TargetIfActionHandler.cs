using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.ActionHandlers
{
    public class TargetIfActionHandler : BaseActionHandler
    {
        public TargetIfActionHandler() : base() { }

        public override bool CanHandle(ActionLine actionLine)
        {
            return actionLine.SpecialHandling.Contains("target_if=");
        }

        protected override ActionLine CheckHandling(ActionLine actionLine)
        {
            List<string> specialHandling = new();
            if (actionLine.SpecialHandling.Contains(','))
                specialHandling = actionLine.SpecialHandling.Split(',').ToList();
            else
                specialHandling.Add(actionLine.SpecialHandling);

            foreach (var entry in specialHandling)
            {
                if (entry.Contains("target_if="))
                {
                    var targetIfValue = entry["target_if=".Length..].Trim();
                    if (!targetIfValue.Contains("max:") && !targetIfValue.Contains("min:"))
                    {
                        actionLine.Type = ActionType.Loop;
                        AddToCondition(actionLine, targetIfValue);
                    }
                    else
                    {
                        actionLine.Comment = $"{actionLine.Comment}\n    -- TODO: Handle {entry}";
                    }
                }
                if (entry.Contains("max_energy=1"))
                {
                    if (actionLine.Action == "ferocious_bite")
                        AddToCondition(actionLine, "energy>=50");
                }
                if (entry.Contains("name="))
                {
                    var nameValue = entry.Replace("name=", "").Trim();
                    if (!string.IsNullOrEmpty(actionLine.Action) && !string.IsNullOrEmpty(nameValue))
                    {
                        actionLine.Action = $"{nameValue}";
                        actionLine.Type = ActionType.UseItem;
                        Program.Locals.Add("use");
                    }
                }
            }
            

            return actionLine;
        }

        private static void AddToCondition(ActionLine actionLine, string condition)
        {
            if (!string.IsNullOrEmpty(actionLine.Condition))
                actionLine.Condition = StringUtilities.CheckForOr(condition) + "&" + StringUtilities.CheckForOr(actionLine.Condition);
            else
                actionLine.Condition = StringUtilities.CheckForOr(condition);
        }
    }
}
