using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.SpecialHandlers
{
    internal class NameSpecialHandler : BaseSpecialHandler
    {
        public override bool CanHandle()
        {
            return Program.currentActionLine.SpecialHandling.Contains("name=");
        }

        public override void Handle()
        {

            List<string> specialHandling = SplitSpecialHandling();

            foreach (var entry in specialHandling)
            {
                if (entry.Contains("name="))
                {
                    var nameValue = entry.Replace("name=", "").Trim();
                    if (!string.IsNullOrEmpty(Program.currentActionLine.Action) && !string.IsNullOrEmpty(nameValue))
                    {
                        switch (Program.currentActionLine.Type)
                        {
                            case ActionType.Module:
                                Program.Locals.Add("module");
                                break;
                            case ActionType.UseItem:
                                Program.Locals.Add("use");
                                break;
                            case ActionType.Variable:
                                Program.Locals.Add("var");
                                break;
                            case ActionType.Default:
                                if (Program.currentActionLine.Action.Contains("variable"))
                                {
                                    Program.Locals.Add("var");
                                    Program.currentActionLine.Action = $"var.{nameValue}";
                                    Program.currentActionLine.Type = ActionType.Variable;
                                    int ifPos = Program.currentActionLine.Condition.IndexOf(",if=");
                                    string value;
                                    string condition;
                                    if (ifPos > 0)
                                    {
                                        value = Program.currentActionLine.Condition[..ifPos].Trim();
                                        condition = Program.currentActionLine.Condition[(ifPos + 4)..].Trim();
                                        Program.currentActionLine.Condition = condition;
                                        Program.currentActionLine.ConvertedSpecial = value;
                                    }
                                }
                                else if (Program.currentActionLine.Action.Contains("use_item"))
                                {
                                    Program.Locals.Add("use");
                                    Program.currentActionLine.Action = $"use.{nameValue}";
                                    Program.currentActionLine.Type = ActionType.UseItem;
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
