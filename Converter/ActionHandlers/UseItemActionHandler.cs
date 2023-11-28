using SimcToBrConverter.Utilities;
using System.Text.RegularExpressions;

namespace SimcToBrConverter.ActionHandlers
{
    public class UseItemActionHandler : BaseActionHandler
    {
        public override bool CanHandle()
        {
            return Program.currentActionLine.Action.Contains("use_item") ||
                    Program.currentActionLine.Action.Contains("use_items") ||
                    Program.currentActionLine.Action.Contains("augmentation") ||
                    Program.currentActionLine.Action.Contains("flask") ||
                    Program.currentActionLine.Action.Contains("potion") ||
                    Program.currentActionLine.Action.Contains("food") ||
                    Program.currentActionLine.Action.Contains("snapshot_stats");
        }

        public override void Handle()
        {
            if (Program.currentActionLine.Action is string s)
            {
                if (s.Contains("use_items") && string.IsNullOrEmpty(Program.currentActionLine.Condition))
                {
                    Program.currentActionLine.Type = ActionType.Module;
                    Program.currentActionLine.Action = "module";
                    Program.Locals.Add("module");
                    Program.currentActionLine.SpecialHandling = "name=basic_trinkets";
                }
                else if (s.Contains("flask"))
                {
                    Program.currentActionLine.Type = ActionType.Module;
                    Program.currentActionLine.Action = "module";
                    Program.Locals.Add("module");
                    Program.currentActionLine.SpecialHandling = "name=flask_up";
                }
                else if (s.Contains("augmentation") || s.Contains("potion") || s.Contains("use_item"))
                {
                    Program.currentActionLine.Type = ActionType.UseItem;
                    Program.Locals.Add("use");
                }
                else if (s.Contains("food") || s.Contains("snapshot_stats"))
                {
                    Program.currentActionLine.Type = ActionType.Ignore;
                }
            }         
            var nameValue = Program.currentActionLine.SpecialHandling.Replace("name=", "").Trim().Split(",")[0];
            nameValue = nameValue.Replace("=trinket", "");
            if (Program.currentActionLine.SpecialHandling.Contains("slots="))
                Program.currentActionLine.Op = nameValue.Replace("slots","");

            // Handle trinket.integer.is case
            string patternIs = @"trinket\.(\d+)\.is\.(\w+)";
            string replacementIs = "equipped.$2.$1";
            Program.currentActionLine.Condition = Regex.Replace(Program.currentActionLine.Condition, patternIs, replacementIs);

            // Handle trinket.integer.cooldown case
            string patternCooldown = @"trinket\.(\d+)\.cooldown\.(\w+)";
            string replacementCooldown = $"cooldown.slot.$2.$1";
            Program.currentActionLine.Condition = Regex.Replace(Program.currentActionLine.Condition, patternCooldown, replacementCooldown);

            if (!string.IsNullOrEmpty(Program.currentActionLine.Action) && !string.IsNullOrEmpty(nameValue))
            {
                if (Program.currentActionLine.SpecialHandling.Contains("slots="))
                    Program.currentActionLine.Action = $"{nameValue.Replace(Program.currentActionLine.Op,"")}";
                else
                    Program.currentActionLine.Action = $"{nameValue}";
            }
            //actionLine.Condition = opValue;
        }
    }
}
