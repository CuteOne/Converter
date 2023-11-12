using SimcToBrConverter.ActionHandlers;
using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Conditions;

namespace SimcToBrConverter
{
    class Program
    {
        private const string PROFILE_URL = "https://raw.githubusercontent.com/simulationcraft/simc/dragonflight/profiles/Tier30/T30_Druid_Feral.simc";

        static async Task Main(string[] args)
        {
            // Download the SimulationCraft profile
            string profile = await DownloadProfile(PROFILE_URL);

            var actionLines = ParseActions(profile.Split('\n'));

            // Define the condition converters and action handlers
            var conditionConverters = GetConditionConverters();
            var actionHandlers = GetActionHandlers();
            var conditionConversionService = new ConditionConversionService(conditionConverters);

            // Generate and print the Lua code
            LuaCodeGenerator.GenerateLuaCode(actionLines, actionHandlers, conditionConversionService);

            // Print the Lua code
            //Console.WriteLine(luaCode);
        }

        /// <summary>
        /// Downloads the SimulationCraft profile from the provided URL.
        /// </summary>
        /// <param name="url">The URL of the SimulationCraft profile.</param>
        /// <returns>The content of the SimulationCraft profile as a string.</returns>
        static async Task<string> DownloadProfile(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    return await client.GetStringAsync(url);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error fetching profile: {e.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Retrieves a list of condition converters used for action conversion.
        /// </summary>
        /// <returns>A list of condition converters.</returns>
        private static List<IConditionConverter> GetConditionConverters()
        {
            return new List<IConditionConverter>
            {
                new ActionConditionConverter(),
                new BuffConditionConverter(),
                new CooldownConditionConverter(),
                new DebuffConditionConverter(),
                new DruidConditionConverter(),
                new GCDConditionConverter(),
                new ItemConditionConverter(),
                new PowerConditionConverter(),
                new SpellTargetsConditionConverter(),
                new TalentConditionConverter(),
                new UnitConditionConverter(),
                new VariableConditionConverter()
            };
        }

        /// <summary>
        /// Retrieves a list of action handlers used for action conversion.
        /// </summary>
        /// <param name="conditionConverters">A list of condition converters to be used by the action handlers.</param>
        /// <returns>A list of action handlers.</returns>
        private static List<IActionHandler> GetActionHandlers()
        {
            return new List<IActionHandler>
            {
                new ActionListActionHandler(),
                new TargetIfActionHandler(),
                // DefaultActionHandler should always be the last in the list to ensure it acts as a fallback.
                new DefaultActionHandler()
            };
        }

        /// <summary>
        /// Parses the provided profile lines to extract action lines.
        /// </summary>
        /// <param name="profileLines">The lines of the SimulationCraft profile.</param>
        /// <returns>A list of parsed action lines.</returns>
        private static List<ActionLine> ParseActions(string[] profileLines)
        {
            List<ActionLine> actionLines = new List<ActionLine>();

            foreach (var line in profileLines)
            {
                if (line.StartsWith("actions"))
                {
                    var result = ActionLineParser.ParseActionLine(line);
                    if (result is ActionLine singleResult)
                    {
                        actionLines.Add(singleResult);
                    }
                    else if (result is MultipleActionLineResult multipleResult)
                    {
                        actionLines.AddRange(multipleResult.ActionLines);
                    }
                }
            }

            return actionLines;
        }
    }
}
