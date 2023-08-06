using System.Text.RegularExpressions;
using System.Text;
using SimcToBrConverter.ActionHandlers;
using SimcToBrConverter.Conditions;
using System.Globalization;
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
            var actionLists = ParseActions(profileLines);

            // Define the condition converters
            List<IConditionConverter> conditionConverters = new List<IConditionConverter>
            {
                new BuffConditionConverter()
            };

            // Define the action handlers
            List<IActionHandler> actionHandlers = new List<IActionHandler>
            {
                new RegularActionHandler(conditionConverters),
                new TargetIfActionHandler(conditionConverters),
                new UseItemActionHandler(conditionConverters)
            };

            // Generate the Lua code
            string luaCode = GenerateLuaCode(actionLists, actionHandlers);

            // Print the Lua code
            Console.WriteLine(luaCode);
        }

        static async Task<string> DownloadProfile(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetStringAsync(url);
            }
        }

        private static Dictionary<string, List<string>> ParseActions(string[] profileLines)
        {
            Dictionary<string, List<string>> actionLists = new Dictionary<string, List<string>>();
            string currentList = "";

            foreach (var line in profileLines)
            {
                if (line.StartsWith("actions"))
                {
                    var match = Regex.Match(line, @"actions\.(?<listName>\w+)(\+=\/|=)(?<action>([^,]|,(?!if=))*?)(,if=(?<condition>.*))?$");
                    var listName = match.Groups["listName"].Value;
                    var action = match.Groups["action"].Value;
                    var condition = match.Groups["condition"].Value;

                    if (!string.IsNullOrEmpty(listName))
                    {
                        currentList = listName;
                        if (!actionLists.ContainsKey(currentList))
                        {
                            actionLists[currentList] = new List<string>();
                        }
                    }

                    if (!string.IsNullOrEmpty(action))
                    {
                        actionLists[currentList].Add(action);
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

            return actionLists;
        }

        public static string GenerateLuaCode(Dictionary<string, List<string>> actionLists, List<IActionHandler> actionHandlers)
        {
            StringBuilder output = new StringBuilder();

            foreach (var kvp in actionLists)
            {
                string listName = kvp.Key;
                output.AppendLine($"actionList.{StringUtilities.ConvertToTitleCase(listName)} = function()");
                output.AppendLine();

                foreach (var action in kvp.Value)
                {
                    foreach (var handler in actionHandlers)
                    {
                        if (handler.CanHandle(action))
                        {
                            output.AppendLine(handler.Handle(action, listName));
                            break;
                        }
                    }
                }

                output.AppendLine("end");
                output.AppendLine();
            }

            return output.ToString();
        }

    }
}
