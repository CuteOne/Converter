using SimcToBrConverter.ActionHandlers;
using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Conditions;

namespace SimcToBrConverter
{
    class Program
    {
        private static string PROFILE_URL = "https://github.com/simulationcraft/simc/blob/dragonflight/profiles/Tier31/T31_Druid_Feral.simc";

        static async Task Main()
        {
            // Always use the raw.githubusercontent.com URL
            if (PROFILE_URL.Contains("github.com"))
            {
                PROFILE_URL = PROFILE_URL.Replace("github.com", "raw.githubusercontent.com");
                PROFILE_URL = PROFILE_URL.Replace("/blob", "");
            }

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
                using HttpClient client = new();
                return await client.GetStringAsync(url);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error fetching profile: {e.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Containsa a list of local variables used by the converted code.
        /// </summary>
        /// <returns>A list of locals</returns>
        public static HashSet<string> Locals { get; set; } = new HashSet<string>();

        /// <summary>
        /// Retrieves a list of condition converters used for action conversion.
        /// </summary>
        /// <returns>A list of condition converters.</returns>
        public static List<IConditionConverter> GetConditionConverters()
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
                new PoolAndWaitActionHandler(),
                new TargetIfActionHandler(),
                new UseItemActionHandler(),
                new VariableActionHandler(),
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
            List<ActionLine> actionLines = new();
            string poolCondition = string.Empty;

            foreach (var line in profileLines)
            {
                if (line.StartsWith("actions"))
                {
                    var result = ActionLineParser.ParseActionLine(line);
                    if (result is ActionLine singleResult)
                    {
                        poolCondition = HandlePool(poolCondition, singleResult);
                        actionLines.Add(singleResult);
                    }
                    else if (result is MultipleActionLineResult multipleResult)
                    {
                        foreach (ActionLine singleLine in multipleResult.ActionLines)
                        {
                            poolCondition = HandlePool(poolCondition, singleLine);
                        }
                        actionLines.AddRange(multipleResult.ActionLines);
                    }
                }
            }

            return actionLines;
        }

        private static string HandlePool(string poolCondition, ActionLine line)
        {
            if (!string.IsNullOrEmpty(poolCondition))
            {
                line.PoolCondition = $"pool_if={poolCondition}";
                return string.Empty;
            }
            if (line.Action.Contains("pool_resource") && line.SpecialHandling.Contains("for_next=1"))
            {
                return line.Condition;
            }
            return string.Empty;
        }
    }
}
