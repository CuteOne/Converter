using System.Text.RegularExpressions;
using System.Text;
using SimcToBrConverter.ActionHandlers;
using SimcToBrConverter.Conditions;
using SimcToBrConverter.Utilities;

namespace SimcToBrConverter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Define the URL of the SimulationCraft profile
            string url = "https://raw.githubusercontent.com/simulationcraft/simc/dragonflight/profiles/Tier30/T30_Druid_Feral.simc";

            // Download the SimulationCraft profile
            string profile = await DownloadProfile(url);

            // Split the profile into lines
            string[] profileLines = profile.Split('\n');

            // Parse the actions from the SimulationCraft profile
            var (actionLists, standaloneActions) = ParseActions(profileLines); // Modified to handle standalone actions

            // Define the condition converters
            List<IConditionConverter> conditionConverters = new List<IConditionConverter>
            {
                new ActionConditionConverter(),
                new BuffConditionConverter(),
                new CooldownConditionConverter(),
                new DotConditionConverter(),
                new GCDConditionConverter(),
                new SpecialCaseConditionConverter(),
                new SpellTargetsConditionConverter(),
                new TalentConditionConverter(),
                new UnitConditionConverter(),
                new VariableConditionConverter()
            };

            // Define the action handlers
            List<IActionHandler> actionHandlers = new List<IActionHandler>
            {
                new ActionListActionHandler(conditionConverters),
                new RegularActionHandler(conditionConverters),
                new TargetIfActionHandler(conditionConverters),
                new UseItemActionHandler(conditionConverters)
            };

            // Generate the Lua code
            GenerateLuaCode(actionLists, standaloneActions, actionHandlers);

            // Print the Lua code
            //Console.WriteLine(luaCode);
        }

        static async Task<string> DownloadProfile(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetStringAsync(url);
            }
        }

        private static (string ListName, string Action, string Condition) ParseActionLine(string line)
        {
            //var match = Regex.Match(line, @"actions\.(?<listName>\w+)(\+=\/|=)(?<action>([^,]|,(?!if=))*?)(,if=(?<condition>.*))?$");
            var match = Regex.Match(line, @"actions(\.(?<listName>\w+))?(\+=\/|=)(?<action>([^,]|,(?!if=))*?)(,if=(?<condition>.*))?$");

            var listName = match.Groups["listName"].Value;
            var action = match.Groups["action"].Value;
            var condition = match.Groups["condition"].Value;

            return (listName, action, condition);
        }

        private static (Dictionary<string, List<string>>, List<string>) ParseActions(string[] profileLines)
        {
            Dictionary<string, List<string>> actionLists = new Dictionary<string, List<string>>();
            List<string> standaloneActions = new List<string>(); // List to store standalone actions
            string currentList = "";

            foreach (var line in profileLines)
            {
                if (line.StartsWith("actions"))
                {
                    var (listName, action, condition) = ParseActionLine(line);

                    if (string.IsNullOrEmpty(listName)) // Standalone action
                    {
                        standaloneActions.Add(action + (string.IsNullOrEmpty(condition) ? "" : $",if={condition}"));
                    }
                    else
                    {
                        currentList = listName;
                        if (!actionLists.ContainsKey(currentList))
                        {
                            actionLists[currentList] = new List<string>();
                        }

                        if (!string.IsNullOrEmpty(action))
                        {
                            if (!string.IsNullOrEmpty(condition))
                            {
                                action += $",if={condition}";
                            }
                            actionLists[currentList].Add(action);
                        }
                    }
                }
            }

            return (actionLists, standaloneActions); // Return both regular and standalone actions
        }

        private static string GenerateActionListLuaCode(string listName, List<string> actions, List<IActionHandler> actionHandlers)
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine($"actionList.{StringUtilities.ConvertToTitleCaseNoSpace(listName)} = function()");

            foreach (var action in actions)
            {
                foreach (var handler in actionHandlers)
                {
                    if (handler.CanHandle(action))
                    {
                        string convertedAction = handler.Handle(listName, action);
                        if (!string.IsNullOrEmpty(convertedAction))
                        {
                            output.Append(convertedAction);
                            break;
                        }
                    }
                }
            }

            output.AppendLine("end");
            output.AppendLine();

            return output.ToString();
        }

        private static void GenerateLuaCode(Dictionary<string, List<string>> actionLists, List<string> standaloneActions, List<IActionHandler> actionHandlers)
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine("local ui");
            output.AppendLine("local cast");
            output.AppendLine("local buff");
            output.AppendLine("local debuff");
            output.AppendLine("local enemies");
            output.AppendLine("local unit");
            output.AppendLine("local charges");
            output.AppendLine("local cd");
            output.AppendLine("local actionList = {}");
            output.AppendLine();

            foreach (var actionList in actionLists)
            {
                output.Append(GenerateActionListLuaCode(actionList.Key, actionList.Value, actionHandlers));
            }

            output.AppendLine("function br.rotations.profile()");
            output.AppendLine();
            output.AppendLine("    ui = br.ui");
            output.AppendLine("    cast = br.player.cast");
            output.AppendLine("    buff = br.player.buff");
            output.AppendLine("    debuff = br.player.debuff");
            output.AppendLine("    enemies = br.player.enemies");
            output.AppendLine("    unit = br.player.unit");
            output.AppendLine("    charges = br.player.charges");
            output.AppendLine("    cd = br.player.cd");
            output.AppendLine();

            // Handle standalone actions
            foreach (var action in standaloneActions)
            {
                foreach (var handler in actionHandlers)
                {
                    if (handler.CanHandle(action))
                    {
                        string convertedAction = handler.Handle("", action); // No list name for standalone actions
                        if (!string.IsNullOrEmpty(convertedAction))
                        {
                            output.Append(convertedAction);
                            break;
                        }
                    }
                }
            }

            output.AppendLine("end");

            //File.WriteAllText("output.lua", output.ToString());
            Console.WriteLine(output.ToString());
        }
    }
}
