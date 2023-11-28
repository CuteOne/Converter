using SimcToBrConverter.ActionHandlers;
using SimcToBrConverter.ActionLines;
using SimcToBrConverter.ConditionConverters;
using SimcToBrConverter.LuaGenerators;
using SimcToBrConverter.SpecialHandlers;
using SimcToBrConverter.Utilities;

namespace SimcToBrConverter
{
    class Program
    {
        private static string PROFILE_URL = "https://github.com/simulationcraft/simc/blob/dragonflight/profiles/Tier31/T31_Demon_Hunter_Havoc.simc";

        // Define Components
        public static readonly List<IActionHandler> actionHandlers = GetActionHandlers();
        public static readonly List<ISpecialHandler> specialHandlers = GetSpecialHandlers();
        public static readonly List<IConditionConverter> conditionConverters = GetConditionConverters();
        public static readonly List<ILuaGenerator> luaGenerators = GetLuaGenerators();
        public static readonly ConditionConversionService conditionConversionService = new(conditionConverters);
        public static ActionLine currentActionLine = new();
        public static ActionLine previousActionLine = new();
        public static List<string> notConverted = new();
        public static List<ConversionResult> conversionResults = new();
        public static List<string> extraVars = new();

        /// <summary>
        /// Asynchronously downloads a SimulationCraft profile from the specified URL, processes the profile to generate Lua code, and prints the Lua code along with the spell list.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
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

            // Parse the SimulationCraft profile
            var actionLines = ParseActions(profile.Split('\n'));
            ProcessActionLines(actionLines);

            // Generate and print the Lua code
            string luaCode = LuaCodeGenerator.GenerateLuaCode();

            // Print the Lua code
            Console.WriteLine(luaCode);

            // Print spell list
            SpellRepository.PrintSpellsByType();
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
        /// Returns a list of ILuaGenerator implementations.
        /// </summary>
        /// <returns>
        /// List of ILuaGenerator implementations.
        /// </returns>
        public static List<ILuaGenerator> GetLuaGenerators()
        {
            return new List<ILuaGenerator>
            {
                new ActionListLuaGenerator(),
                new LoopLuaGenerator(),
                new MaxMinLuaGenerator(),
                new ModuleLuaGenerator(),
                new PoolLuaGenerator(),
                new RetargetLuaGenerator(),
                new UseItemLuaGenerator(),
                new VariableLuaGenerator(),
                new WaitLuaGenerator(),
                new DefaultLuaGenerator()
            };
        }


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
        /// Returns a list of ISpecialHandler objects.
        /// </summary>
        /// <returns>
        /// List of ISpecialHandler objects.
        /// </returns>
        public static List<ISpecialHandler> GetSpecialHandlers()
        {
            return new List<ISpecialHandler>
            {
                new LineCdSpecialHandler(),
                new MaxEnergySpecialHandler(),
                new NameSpecialHandler(),
                new TargetIfSpecialHandler(),
            };
        }

        /// <summary>
        /// Returns a list of action handlers including default action handler as the last item.
        /// </summary>
        /// <returns>
        /// List of IActionHandler
        /// </returns>
        private static List<IActionHandler> GetActionHandlers()
        {
            return new List<IActionHandler>
            {
                new ActionListActionHandler(),
                new PoolActionHandler(),
                new RetargetActionHandler(),
                new UseItemActionHandler(),
                new VariableActionHandler(),
                new WaitActionHandler(),
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
            //string poolCondition = string.Empty;

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


        /// <summary>
        /// Processes the list of action lines, handles special and non-special actions, and converts conditions using the condition conversion service.
        /// </summary>
        /// <param name="actionLines">The list of action lines to be processed.</param>
        private static void ProcessActionLines(List<ActionLine> actionLines)
        {
            foreach (var actionLine in actionLines)
            {
                currentActionLine = actionLine;
                notConverted = new();
                // Process Actions
                foreach (var actionHandler in actionHandlers)
                {
                    if (actionHandler.CanHandle())
                    {
                        actionHandler.Handle();
                        break; // Only one action handler should handle an action line
                    }
                }
                // Process Special Handling
                foreach (var specialHandler in specialHandlers)
                {
                    if (specialHandler.CanHandle())
                    {
                        specialHandler.Handle(); // There could be multiple special handlers for a single action line
                    }
                }
                if (currentActionLine.Type == ActionType.Ignore) continue;
                // Process Conditions
                if (string.IsNullOrEmpty(currentActionLine.Condition) && !string.IsNullOrEmpty(currentActionLine.Value))
                {
                    // If there is no condition, but there is a value, then treat the value as the condition for conversion purposes and restore the value
                    currentActionLine.Condition = currentActionLine.Value;
                    conditionConversionService.ConvertCondition();
                    currentActionLine.Value = currentActionLine.Condition;
                    currentActionLine.Condition = string.Empty;
                }
                else
                    conditionConversionService.ConvertCondition();
                // Add to Results
                ConversionResult conversionResult = new(currentActionLine, notConverted, "");
                conversionResults.Add(conversionResult);
            }
        }
    }
}
