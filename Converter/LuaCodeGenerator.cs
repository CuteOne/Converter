using SimcToBrConverter.ActionHandlers;
using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Generators;
using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter
{
    public class LuaCodeGenerator
    {
        private static string? lastListName = null; // Static variable to keep track of the last ListName

        private static readonly List<IActionCodeGenerator> actionGenerators = new()
        {
            new ActionListGenerator(),
            new DefaultGenerator(),
            new LoopGenerator(),
            new ModuleGenerator(),
            new PoolGenerator(),
            new UseItemGenerator(),
            new VariableGenerator(),
            new WaitGenerator()
        };

        /// <summary>
        /// Processes a list of ActionLines using a list of ActionHandlers and a ConditionConversionService. 
        /// For each ActionLine, it finds the first ActionHandler that can handle it, handles it, and then converts its conditions using the ConditionConversionService. 
        /// The result is a list of tuples, each containing an updated ActionLine and a list of conditions that could not be converted.
        /// </summary>
        /// <param name="actionLines">A list of ActionLines to be processed.</param>
        /// <param name="actionHandlers">A list of ActionHandlers to handle the ActionLines.</param>
        /// <param name="conditionConversionService">A ConditionConversionService to convert the conditions of the ActionLines.</param>
        /// <returns>
        /// A list of tuples. Each tuple contains an updated ActionLine and a list of conditions that could not be converted.
        /// </returns>
        private static List<(ActionLine actionLine, List<string> notConvertedConditions)> ProcessActionLines(List<ActionLine> actionLines, List<IActionHandler> actionHandlers, ConditionConversionService conditionConversionService)
        {
            var processedActionLines = new List<(ActionLine, List<string>)>();

            foreach (var actionLine in actionLines)
            {
                var handledActionLine = actionHandlers.FirstOrDefault(h => h.CanHandle(actionLine))?.Handle(actionLine);
                if (handledActionLine != null)
                {
                    var (updatedActionLine, notConvertedConditions) = conditionConversionService.ConvertCondition((ActionLine)handledActionLine);
                    processedActionLines.Add((updatedActionLine, notConvertedConditions));
                }
            }

            return processedActionLines;
        }

        /// <summary>
        /// Generates Lua code for a given action list. This method checks if the action list name has changed and if so, it ends the previous action list and starts a new one. It then generates the Lua code for the action line.
        /// </summary>
        /// <param name="actionLine">The action line to convert into Lua code.</param>
        /// <param name="notConvertedConditions">A list of conditions that were not converted.</param>
        /// <returns>
        /// A string containing the generated Lua code for the action list.
        /// </returns>
        internal static string GenerateActionListLuaCode(ActionLine actionLine, List<string> notConvertedConditions)
        {
            StringBuilder output = new();

            if (actionLine.ListName != lastListName && !string.IsNullOrEmpty(actionLine.ListName))
            {
                if (!String.IsNullOrEmpty(lastListName))
                {
                    output.AppendLine($"end -- End Action List - {StringUtilities.ConvertToTitleCaseNoSpace(lastListName)}");
                }
                output.AppendLine();
            }

            // Check if actionLine.ListName is not empty/null and has changed
            if (!string.IsNullOrEmpty(actionLine.ListName) && actionLine.ListName != lastListName)
            {
                output.AppendLine($"-- Action List - {StringUtilities.ConvertToTitleCaseNoSpace(actionLine.ListName)}");
                output.AppendLine($"actionList.{StringUtilities.ConvertToTitleCaseNoSpace(actionLine.ListName)} = function()");
                lastListName = actionLine.ListName; // Update the lastListName
            }

            // Generate the Lua code for the action
            output.Append(GenerateActionLineLuaCode(actionLine, notConvertedConditions));

            return output.ToString();
        }

        /// <summary>
        /// Generates Lua code for a given action line. This method takes an action line and a list of conditions that were 
        /// not converted, and generates the corresponding Lua code. It handles different types of actions such as loops, 
        /// variables, modules, and checks for conditions that were not converted. It also formats the action line for debugging purposes.
        /// </summary>
        /// <param name="actionLine">The action line to generate Lua code for.</param>
        /// <param name="notConvertedConditions">A list of conditions that were not converted.</param>
        /// <returns>
        /// A string containing the generated Lua code.
        /// </returns>
        internal static string GenerateActionLineLuaCode(ActionLine actionLine, List<string> notConvertedConditions)
        {
            // Check if the action is null or contains "invoke_external_buff"
            if (string.IsNullOrEmpty(actionLine.Action) || actionLine.Action.Contains("invoke_external_buff"))
                return "";

            // Convert the action to camel case
            var formattedCommand = StringUtilities.ConvertToCamelCase(actionLine.Action);

            // Convert the action to title case for debugging
            var debugCommand = StringUtilities.ConvertToTitleCase(actionLine.Action);

            // Initialize a new string builder
            var output = new StringBuilder();

            // Check for "or" in the action line condition
            string convertedCondition = StringUtilities.CheckForOr(actionLine.Condition);

            string listNameTag = "";

            // Check if the list name is not null or empty
            if (!string.IsNullOrEmpty(actionLine.ListName))
                listNameTag = $" [{StringUtilities.ConvertToTitleCase(actionLine.ListName)}]";

            // Generate Action Lines
            foreach (var generator in actionGenerators)
            {
                if (generator.CanGenerate(actionLine.Type))
                {
                    // Generate comments for the current action line
                    string comments = BaseActionGenerator.GenerateCommentsLineCode(actionLine, notConvertedConditions);

                    // Generate the specific action line code
                    string actionCode = generator.GenerateActionLineCode(actionLine, formattedCommand, debugCommand, convertedCondition, listNameTag);

                    // Append both comments and action code to the output
                    output.Append(comments);
                    output.Append(actionCode);
                }
            }

            // Return the generated Lua code
            return output.ToString();
        }

        /// <summary>
        /// Generates Lua code from a list of action lines and action handlers. 
        /// This method processes the action lines, generates locals, action lists, and main rotation. 
        /// The generated Lua code is then written to the console.
        /// </summary>
        /// <param name="actionLines">A list of action lines to be processed.</param>
        /// <param name="actionHandlers">A list of action handlers to be used in processing the action lines.</param>
        /// <param name="conditionConversionService">A service for converting conditions in the action lines.</param>
        internal static void GenerateLuaCode(List<ActionLine> actionLines, List<IActionHandler> actionHandlers, ConditionConversionService conditionConversionService)
        {
            var processedActionLines = ProcessActionLines(actionLines, actionHandlers, conditionConversionService);
            StringBuilder output = new();

            // Generate Locals
            Program.Locals.Add("actionList"); // Add actionList to the list of locals (it's always used)
            Program.Locals.Add("ui"); // Add ui to the list of locals (it's always used)

            var locals = Program.Locals
                .Select(s => s.Replace("()", ""))
                .Distinct()
                .OrderBy(s => s)
                .ToList();
            foreach (var local in locals)
            {
                output.AppendLine($"local {local}");
            }
            output.AppendLine();

            // Generate Action Lists
            string lastListName = "";
            foreach (var (actionLine, notConvertedConditions) in processedActionLines.Where(a => !string.IsNullOrEmpty(a.actionLine.ListName)))
            {
                output.Append(GenerateActionListLuaCode(actionLine, notConvertedConditions));
                lastListName = actionLine.ListName;
            }
            output.AppendLine($"end -- End Action List - {StringUtilities.ConvertToTitleCaseNoSpace(lastListName)}");
            output.AppendLine();

            // Generate Main Rotation
            output.AppendLine("function br.rotations.profile()");
            output.AppendLine();
            // Main Rotation - Generate Local Defines
            foreach (var local in locals)
            {
                // Check if the local is a power type
                if (PowerType.IsPowerType(local))
                    output.AppendLine($"    {local} = br.player.power.{local}");
                else if (local.Equals("var", StringComparison.OrdinalIgnoreCase))
                    output.AppendLine($"    {local} = br.player.variable");
                else
                    output.AppendLine($"    {local} = br.player.{local}");
            }
            output.AppendLine();

            // Main Rotation - Generate Non-Listed Actions
            foreach (var (actionLine, notConvertedConditions) in processedActionLines.Where(a => string.IsNullOrEmpty(a.actionLine.ListName)))
            {
                output.Append(GenerateActionLineLuaCode(actionLine, notConvertedConditions));
            }

            output.AppendLine("end");
            output.AppendLine();

            //File.WriteAllText("output.lua", output.ToString());
            Console.WriteLine(output.ToString());
            SpellRepository.PrintSpellsByType();
        }
    }
}
