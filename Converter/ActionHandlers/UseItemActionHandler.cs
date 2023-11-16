using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Conditions;
using System.Text.RegularExpressions;

namespace SimcToBrConverter.ActionHandlers
{
    public class UseItemActionHandler : BaseActionHandler
    {
        public UseItemActionHandler() : base() { }

        public override bool CanHandle(ActionLine actionLine)
        {
            return actionLine.Action.Contains("use_item") || 
                    actionLine.Action.Contains("use_items") || 
                    actionLine.Action.Contains("augmentation") ||
                    actionLine.Action.Contains("flask") ||
                    actionLine.Action.Contains("potion") ||
                    actionLine.Action.Contains("food") ||
                    actionLine.Action.Contains("snapshot_stats");
        }

        protected override ActionLine CheckHandling(ActionLine actionLine)
        {
            if (actionLine.Action is string s)
            {
                if (s.Contains("use_items"))
                {
                    actionLine.Type = ActionType.Module;
                    actionLine.Action = "module";
                    Program.Locals.Add("module");
                    actionLine.SpecialHandling = "name=basic_trinkets";
                }
                else if (s.Contains("augmentation") || s.Contains("flask") || s.Contains("potion") || s.Contains("use_item"))
                {
                    actionLine.Type = ActionType.UseItem;
                    Program.Locals.Add("use");
                }
                else if (s.Contains("food") || s.Contains("snapshot_stats"))
                {
                    actionLine = new ActionLine();
                }
            }         
            var nameValue = actionLine.SpecialHandling.Replace("name=", "").Trim();

            // Handle trinket.integer.is case
            string patternIs = @"trinket\.(\d+)\.is\.(\w+)";
            string replacementIs = "equipped.$2.$1";
            actionLine.Condition = Regex.Replace(actionLine.Condition, patternIs, replacementIs);

            // Handle trinket.integer.cooldown case
            string patternCooldown = @"trinket\.(\d+)\.cooldown\.(\w+)";
            string replacementCooldown = $"cooldown.slot.$2.$1";
            actionLine.Condition = Regex.Replace(actionLine.Condition, patternCooldown, replacementCooldown);

            if (!string.IsNullOrEmpty(actionLine.Action) && !string.IsNullOrEmpty(nameValue))
                actionLine.Action = $"{nameValue}";
            //actionLine.Condition = opValue;

            return actionLine;
        }
    }
}
