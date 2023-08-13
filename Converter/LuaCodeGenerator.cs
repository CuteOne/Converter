using SimcToBrConverter.ActionHandlers;
using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter
{
    public class LuaCodeGenerator
    {
        private static string? lastListName = null; // Static variable to keep track of the last ListName

        /// <summary>
        /// Generates Action Lists and places generated lua code for actions that match the Action List name within. 
        /// </summary>
        /// <param name="actionLine">The action line to convert.</param>
        /// <param name="actionHandlers">A list of action handlers to use for conversion.</param>
        /// <returns>The generated Lua code as a string.</returns>
        internal static string GenerateActionListLuaCode(ActionLine actionLine, List<IActionHandler> actionHandlers, ConditionConversionService conditionConversionService)
        {
            StringBuilder output = new StringBuilder();

            // Convert the action using the appropriate handler
            var handledActionLine = actionHandlers.FirstOrDefault(h => h.CanHandle(actionLine))?.Handle(actionLine);

            if (handledActionLine == null)
            {
                return string.Empty;
            }
            
            // Convert the condition
            var (updatedActionLine, notConvertedConditions) = conditionConversionService.ConvertCondition((ActionLine)handledActionLine);


            if (updatedActionLine.ListName != lastListName && !string.IsNullOrEmpty(updatedActionLine.ListName))
            {
                output.AppendLine("end");
                output.AppendLine();
            }

            // Check if actionLine.ListName is not empty/null and has changed
            if (!string.IsNullOrEmpty(updatedActionLine.ListName) && updatedActionLine.ListName != lastListName)
            {
                output.AppendLine($"actionList.{StringUtilities.ConvertToTitleCaseNoSpace(updatedActionLine.ListName)} = function()");
                lastListName = updatedActionLine.ListName; // Update the lastListName
            }

            // Generate the Lua code for the action
            output.Append(GenerateActionLineLuaCode(updatedActionLine, notConvertedConditions));

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

            // Handle Action Lists
            foreach (var actionLine in actionLines)
            {
                output.Append(GenerateActionListLuaCode(actionLine, actionHandlers, conditionConversionService));
            }
            output.AppendLine("end");
            output.AppendLine();

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

            // Handle standalone actions (those with an empty ListName)
            var standaloneActions = actionLines.Where(a => string.IsNullOrEmpty(a.ListName)).ToList();
            foreach (var actionLine in standaloneActions)
            {
                var applicableHandlers = actionHandlers.Where(h => h.CanHandle(actionLine)).ToList();
                foreach (var handler in applicableHandlers)
                {
                    ActionLine handledAction = handler.Handle(actionLine);
                    if (handledAction.Action != null)
                    {
                        var (updatedActionLine, notConvertedConditions) = conditionConversionService.ConvertCondition(handledAction);
                        string convertedAction = GenerateActionLineLuaCode(updatedActionLine, notConvertedConditions);
                        output.Append(convertedAction);
                        break;
                    }
                }
            }

            output.AppendLine("end");

            //File.WriteAllText("output.lua", output.ToString());
            Console.WriteLine(output.ToString());
        }
    }

}
