using SimcToBrConverter.ActionLines;
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
            switch (actionLine.Action)
            {
                case string s when s.Contains("use_items"):
                    actionLine.Action = "module";
                    actionLine.SpecialHandling = "name=basic_trinkets";
                    break;
                case string s when s.Contains("augmentation"):
                    actionLine.Action = "use_item";
                    actionLine.SpecialHandling = "name=augmentation";
                    break;
                case string s when s.Contains("flask"):
                    actionLine.Action = "use_item";
                    actionLine.SpecialHandling = "name=flask";
                    break;
                case string s when s.Contains("potion"):
                    actionLine.Action = "use_item";
                    actionLine.SpecialHandling = "name=potion";
                    break;
                case string s when s.Contains("food") || s.Contains("snapshot_stats"):
                    actionLine = new ActionLine();
                    break;
                default:
                    break;
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
                actionLine.Action = $"{actionLine.Action}.{nameValue}";
            //actionLine.Condition = opValue;

            return actionLine;
        }
    }
}
