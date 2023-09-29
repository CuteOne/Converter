using SimcToBrConverter.ActionHandlers;
using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter
{
    public class LuaCodeGenerator
    {
        private static string? lastListName = null; // Static variable to keep track of the last ListName

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
        /// Generates Action Lists and places generated lua code for actions that match the Action List name within. 
        /// </summary>
        /// <param name="actionLine">The action line to convert.</param>
        /// <param name="actionHandlers">A list of action handlers to use for conversion.</param>
        /// <returns>The generated Lua code as a string.</returns>
        internal static string GenerateActionListLuaCode(ActionLine actionLine, List<string> notConvertedConditions)
        {
            StringBuilder output = new StringBuilder();

            if (actionLine.ListName != lastListName && !string.IsNullOrEmpty(actionLine.ListName))
            {
                output.AppendLine("end");
                output.AppendLine();
            }

            // Check if actionLine.ListName is not empty/null and has changed
            if (!string.IsNullOrEmpty(actionLine.ListName) && actionLine.ListName != lastListName)
            {
                output.AppendLine($"actionList.{StringUtilities.ConvertToTitleCaseNoSpace(actionLine.ListName)} = function()");
                lastListName = actionLine.ListName; // Update the lastListName
            }

            // Generate the Lua code for the action
            output.Append(GenerateActionLineLuaCode(actionLine, notConvertedConditions));

            return output.ToString();
        }

        internal static string GenerateActionLineLuaCode(ActionLine actionLine, List<string> notConvertedConditions)
        {
            var formattedCommand = StringUtilities.ConvertToCamelCase(actionLine.Action);
            var debugCommand = StringUtilities.ConvertToTitleCase(actionLine.Action);

            var output = new StringBuilder();
            output.AppendLine($"    -- {debugCommand}");
            output.AppendLine($"    -- {actionLine.Comment}");

            if (notConvertedConditions.Any(s => !string.IsNullOrEmpty(s)))
            {
                output.AppendLine($"    -- TODO: The following conditions were not converted:");
                foreach (var notConvertedCondition in notConvertedConditions)
                {
                    if (!string.IsNullOrEmpty(notConvertedCondition))
                        output.AppendLine($"    -- {notConvertedCondition}");
                }
            }

            bool generateActionListAction = actionLine.Action.Contains("action_list");
            bool generateLoopAction = actionLine.SpecialHandling.Contains("target_if") && !(actionLine.SpecialHandling.Contains("min:") || actionLine.SpecialHandling.Contains("max:"));
            string convertedCondition = StringUtilities.CheckForOr(actionLine.Condition);
            if (!string.IsNullOrEmpty(convertedCondition))
                convertedCondition = " and " + convertedCondition;
            string listNameTag = "";
            if (!string.IsNullOrEmpty(actionLine.ListName))
                listNameTag = $" [{StringUtilities.ConvertToTitleCase(actionLine.ListName)}]";

            if (generateActionListAction)
            {
                output.AppendLine($"    if {actionLine.Condition} then");
                output.AppendLine($"        if actionList.{StringUtilities.ConvertToTitleCaseNoSpace(actionLine.Action)}() then return true end");
                output.AppendLine("    end");
            }
            else if (generateLoopAction)
            {
                output.AppendLine($"    for i = 1, #enemies.PLACEHOLDER_RANGE do");
                output.AppendLine($"        local thisUnit = enemies.PLACEHOLDER_RANGE[i]");
                output.AppendLine($"        if cast.able.{formattedCommand}(thisUnit){convertedCondition} then");
                output.AppendLine($"            if cast.{formattedCommand}(thisUnit) then ui.debug(\"Casting {debugCommand}{listNameTag}\") return true end");
                output.AppendLine("        end");
                output.AppendLine("    end");
            }
            else
            {
                output.AppendLine($"    if cast.able.{formattedCommand}(){convertedCondition} then");
                output.AppendLine($"        if cast.{formattedCommand}() then ui.debug(\"Casting {debugCommand}{listNameTag}\") return true end");
                output.AppendLine("    end");
            }

            return output.ToString();
        }

        /// <summary>
        /// Generates Lua code for a BadRotations profile.
        /// </summary>
        /// <param name="actionLines">The list of action lines to convert.</param>
        /// <param name="actionHandlers">A list of action handlers to use for conversion.</param>
        internal static void GenerateLuaCode(List<ActionLine> actionLines, List<IActionHandler> actionHandlers, ConditionConversionService conditionConversionService)
        {
            var processedActionLines = ProcessActionLines(actionLines, actionHandlers, conditionConversionService);
            StringBuilder output = new StringBuilder();

            var locals = conditionConversionService.Locals.ToList();
            locals.Sort();
            foreach (var local in conditionConversionService.Locals)
            {
                output.AppendLine($"local {local}");
            }
            output.AppendLine("local actionList = {}");
            output.AppendLine();

            // Handle Action Lists first
            foreach (var (actionLine, notConvertedConditions) in processedActionLines.Where(a => !string.IsNullOrEmpty(a.actionLine.ListName)))
            {
                output.Append(GenerateActionListLuaCode(actionLine, notConvertedConditions));
            }
            output.AppendLine("end");
            output.AppendLine();

            output.AppendLine("function br.rotations.profile()");
            output.AppendLine();
            foreach (var local in conditionConversionService.Locals)
            {
                output.AppendLine($"    {local} = br.player.{local}");
            }
            output.AppendLine();

            // Handle standalone actions (those with an empty ListName)
            foreach (var (actionLine, notConvertedConditions) in processedActionLines.Where(a => string.IsNullOrEmpty(a.actionLine.ListName)))
            {
                output.Append(GenerateActionLineLuaCode(actionLine, notConvertedConditions));
            }

            output.AppendLine("end");

            //File.WriteAllText("output.lua", output.ToString());
            Console.WriteLine(output.ToString());
        }
    }
}
