using SimcToBrConverter.ActionHandlers;
using SimcToBrConverter.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SimcToBrConverter.ActionLineParser;

namespace SimcToBrConverter
{
    public class LuaCodeGenerator
    {
        /// <summary>
        /// Generates Action Lists and places generated lua code for actions that match the Action List name within. 
        /// </summary>
        /// <param name="actionLine">The action line to convert.</param>
        /// <param name="actionHandlers">A list of action handlers to use for conversion.</param>
        /// <returns>The generated Lua code as a string.</returns>
        internal static string GenerateActionListLuaCode(ActionLine actionLine, List<IActionHandler> actionHandlers)
        {
            StringBuilder output = new StringBuilder();

            if (!string.IsNullOrEmpty(actionLine.ListName))
            {
                output.AppendLine($"actionList.{StringUtilities.ConvertToTitleCaseNoSpace(actionLine.ListName)} = function()");
            }

            // Convert the action using the appropriate handler
            var applicableHandlers = actionHandlers.Where(h => h.CanHandle(actionLine)).ToList();
            foreach (var handler in applicableHandlers)
            {
                string convertedAction = handler.Handle(actionLine);
                if (!string.IsNullOrEmpty(convertedAction))
                {
                    output.Append(convertedAction);
                    break;
                }
            }

            if (!string.IsNullOrEmpty(actionLine.ListName))
            {
                output.AppendLine("end");
                output.AppendLine();
            }

            return output.ToString();
        }

        private static bool UseLoopAction(string action)
        {
            return false; // Default behavior in the base class
        }

        private static bool ActionListAction(string action)
        {
            return false; // Default behavior in the base class
        }

        internal static string GenerateActionLineLuaCode(ActionLine actionParsed, string convertedCondition, List<string> notConvertedConditions)
        {
            var formattedCommand = StringUtilities.ConvertToCamelCase(actionParsed.Action);
            var debugCommand = StringUtilities.ConvertToTitleCase(actionParsed.Action);

            var output = new StringBuilder();
            output.AppendLine($"    -- {debugCommand}");
            output.AppendLine($"    -- {actionParsed.Action}");

            if (notConvertedConditions.Any(s => !string.IsNullOrEmpty(s)))
            {
                output.AppendLine($"    -- TODO: The following conditions were not converted:");
                foreach (var notConvertedCondition in notConvertedConditions)
                {
                    if (!string.IsNullOrEmpty(notConvertedCondition))
                        output.AppendLine($"    -- {notConvertedCondition}");
                }
            }

            if (ActionListAction(actionParsed.Action))
            {
                var actionListCommand = StringUtilities.ConvertToTitleCaseNoSpace(actionParsed.Action);
                // Check for the specific case
                if (convertedCondition.StartsWith(" and (") && convertedCondition.EndsWith(")"))
                {
                    // Remove the specific parts
                    convertedCondition = convertedCondition.Replace(" and (", "");
                    convertedCondition = convertedCondition.Remove(convertedCondition.Length - 1);
                }
                output.AppendLine($"    if {convertedCondition} then");
                output.AppendLine($"        if actionList.{actionListCommand}() then return true end");
                output.AppendLine("    end");
            }
            else if (UseLoopAction(actionParsed.Action))
            {
                output.AppendLine($"    for i = 1, #enemies.PLACEHOLDER_RANGE do");
                output.AppendLine($"        local thisUnit = enemies.PLACEHOLDER_RANGE[i]");
                output.AppendLine($"        if cast.able.{formattedCommand}(thisUnit){convertedCondition} then");
                output.AppendLine($"            if cast.{formattedCommand}(thisUnit) then ui.debug(\"Casting {debugCommand} [{StringUtilities.ConvertToTitleCase(actionParsed.ListName)}]\") return true end");
                output.AppendLine("        end");
                output.AppendLine("    end");
            }
            else
            {
                output.AppendLine($"    if cast.able.{formattedCommand}(){convertedCondition} then");
                output.AppendLine($"        if cast.{formattedCommand}() then ui.debug(\"Casting {debugCommand} [{StringUtilities.ConvertToTitleCase(actionParsed.ListName)}]\") return true end");
                output.AppendLine("    end");
            }

            return output.ToString();
        }

        /// <summary>
        /// Generates Lua code for a BadRotations profile.
        /// </summary>
        /// <param name="actionLines">The list of action lines to convert.</param>
        /// <param name="actionHandlers">A list of action handlers to use for conversion.</param>
        internal static void GenerateLuaCode(List<ActionLine> actionLines, List<IActionHandler> actionHandlers)
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
                output.Append(GenerateActionListLuaCode(actionLine, actionHandlers));
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

            // Handle standalone actions (those with an empty ListName)
            var standaloneActions = actionLines.Where(a => string.IsNullOrEmpty(a.ListName)).ToList();
            foreach (var actionLine in standaloneActions)
            {
                var applicableHandlers = actionHandlers.Where(h => h.CanHandle(actionLine)).ToList();
                foreach (var handler in applicableHandlers)
                {
                    string convertedAction = handler.Handle(actionLine);
                    if (!string.IsNullOrEmpty(convertedAction))
                    {
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
