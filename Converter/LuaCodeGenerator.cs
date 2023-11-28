using SimcToBrConverter.LuaGenerators;
using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter
{
    public class LuaCodeGenerator
    {
        private static string? lastListName = null; // Static variable to keep track of the last ListName

        /// <summary>
        /// Generates Lua code for a given action list. This method checks if the action list name has changed and if so, it ends the previous action list and starts a new one. It then generates the Lua code for the action line.
        /// </summary>
        /// <param name="actionLine">The action line to convert into Lua code.</param>
        /// <param name="notConvertedConditions">A list of conditions that were not converted.</param>
        /// <returns>
        /// A string containing the generated Lua code for the action list.
        /// </returns>
        internal static void GenerateActionListLuaCode(ConversionResult conversionResult)
        {
            StringBuilder output = new();

            if (conversionResult.ActionLine.ListName != lastListName && !string.IsNullOrEmpty(conversionResult.ActionLine.ListName))
            {
                if (!String.IsNullOrEmpty(lastListName))
                {
                    output.AppendLine($"end -- End Action List - {StringUtilities.ConvertToTitleCaseNoSpace(lastListName)}");
                }
                output.AppendLine();
            }

            // Check if actionLine.ListName is not empty/null and has changed
            if (!string.IsNullOrEmpty(conversionResult.ActionLine.ListName) && conversionResult.ActionLine.ListName != lastListName)
            {
                output.AppendLine($"-- Action List - {StringUtilities.ConvertToTitleCaseNoSpace(conversionResult.ActionLine.ListName)}");
                output.AppendLine($"actionList.{StringUtilities.ConvertToTitleCaseNoSpace(conversionResult.ActionLine.ListName)} = function()");
                lastListName = conversionResult.ActionLine.ListName; // Update the lastListName
            }

            // Generate the Lua code for the action
            GenerateActionLineLuaCode(conversionResult);
            output.Append(conversionResult.Result);

            conversionResult.Result = output.ToString();
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
        internal static void GenerateActionLineLuaCode(ConversionResult conversionResult)
        {
            // Check if the action is null or contains "invoke_external_buff"
            if (string.IsNullOrEmpty(conversionResult.ActionLine.Action) || conversionResult.ActionLine.Action.Contains("invoke_external_buff"))
                return;

            // Convert the action to camel case
            var formattedCommand = StringUtilities.ConvertToCamelCase(conversionResult.ActionLine.Action);

            // Convert the action to title case for debugging
            var debugCommand = StringUtilities.ConvertToTitleCase(conversionResult.ActionLine.Action);

            // Initialize a new string builder
            var output = new StringBuilder();

            // Check for "or" in the action line condition
            string convertedCondition = StringUtilities.CheckForOr(conversionResult.ActionLine.Condition);

            string listNameTag = "";

            // Check if the list name is not null or empty
            if (!string.IsNullOrEmpty(conversionResult.ActionLine.ListName))
                listNameTag = $" [{StringUtilities.ConvertToTitleCase(conversionResult.ActionLine.ListName)}]";

            // Generate Action Lines
            List<ILuaGenerator> luaGenerators = Program.luaGenerators;
            foreach (var generator in luaGenerators.Where(g => g.CanGenerate(conversionResult)))
            {
                // Generate comments for the current action line
                string comments = BaseLuaGenerator.GenerateCommentsLineCode(conversionResult);

                // Generate the specific action line code
                string actionCode = generator.GenerateActionLineCode(conversionResult, formattedCommand, debugCommand, convertedCondition, listNameTag);

                // Append both comments and action code to the output
                output.Append(comments);
                output.Append(actionCode);
            }

            // Return the generated Lua code
            conversionResult.Result = output.ToString();
        }

        /// <summary>
        /// Generates Lua code from a list of action lines and action handlers. 
        /// This method processes the action lines, generates locals, action lists, and main rotation. 
        /// The generated Lua code is then written to the console.
        /// </summary>
        /// <param name="actionLines">A list of action lines to be processed.</param>
        /// <param name="actionHandlers">A list of action handlers to be used in processing the action lines.</param>
        /// <param name="conditionConversionService">A service for converting conditions in the action lines.</param>
        internal static string GenerateLuaCode()
        {
            // Generate Action Lists
            string lastListName = "";
            StringBuilder processedActionListLines = new();
            foreach (var conversionResult in Program.conversionResults.Where(a => !string.IsNullOrEmpty(a.ActionLine.ListName)))
            {
                GenerateActionListLuaCode(conversionResult);
                processedActionListLines.AppendLine(conversionResult.Result);
                lastListName = conversionResult.ActionLine.ListName;
                Program.previousActionLine = conversionResult.ActionLine;
            }

            // Generate Locals
            StringBuilder processedLocals = new();
            Program.Locals.Add("actionList"); // Add actionList to the list of locals (it's always used)
            Program.Locals.Add("ui"); // Add ui to the list of locals (it's always used)

            var locals = Program.Locals
                .Select(s => s.Replace("()", ""))
                .Distinct()
                .OrderBy(s => s)
                .ToList();
            foreach (var local in locals)
            {
                if (local.Equals("actionList"))
                    processedLocals.AppendLine($"local {local} = {{}}");
                else
                    processedLocals.AppendLine($"local {local}");
            }
            processedLocals.AppendLine();

            // Generate Extra Conditions
            StringBuilder processedExtraVars = new();
            foreach (var extraVar in Program.extraVars)
            {
                processedExtraVars.AppendLine(extraVar);
            }

            // Generate Local Defines
            StringBuilder processedLocalDefines = new();
            foreach (var local in locals)
            {
                // Check if the local is a power type
                if (PowerType.IsPowerType(local))
                    processedLocalDefines.AppendLine($"    {local} = br.player.power.{local}");
                // Check if the local is a variable
                else if (local.Equals("var", StringComparison.OrdinalIgnoreCase))
                    processedLocalDefines.AppendLine($"    {local} = br.player.variable");
                // Check if the local is actionList
                else if (!local.Equals("actionList", StringComparison.OrdinalIgnoreCase))
                    processedLocalDefines.AppendLine($"    {local} = br.player.{local}");
            }

            // Generate Rotation
            string rotation = @"
    -------------------------
    ----- Begin Profile -----
    -------------------------
    -- Profile Stop | Pause
    if not unit.inCombat() and not unit.exists(""target"") and var.profileStop then
        var.profileStop = false
    elseif (unit.inCombat() and var.profileStop) or ui.pause() or ui.mode.rotation==4 then
        return true
    else
        -----------------------
        ------- Rotation ------
        -----------------------
        if actionList.Extras() then return true end
        if actionList.Defensive() then return true end
        ----------------------------------
        ----- Out of Combat Rotation -----
        ----------------------------------
        if not unit.inCombat() and not (unit.flying() or unit.mounted()) then
            if actionList.Precombat() then return true end
        end
        ------------------------------
        ----- In Combat Rotation -----
        ------------------------------
        if (unit.inCombat() or (not unit.inCombat() and unit.valid(""target""))) and not var.profileStop
            and unit.exists(""target"") and cd.global.remain() == 0
        then
            if actionList.Combat() then return true end
        end
    end
            ";

            // Construct the final output
            StringBuilder output = new();

            output.AppendLine(processedLocals.ToString());
            output.AppendLine(processedActionListLines.ToString());
            output.Remove(output.Length - 1, 1);
            output.AppendLine($"end -- End Action List - {StringUtilities.ConvertToTitleCaseNoSpace(lastListName)}");
            output.AppendLine();
            output.AppendLine("function br.rotations.profile()");
            output.AppendLine(processedLocalDefines.ToString());
            output.AppendLine(processedExtraVars.ToString());
            output.AppendLine(rotation);
            output.Remove(output.Length - 1, 1);
            output.AppendLine("end");

            return output.ToString();
        }
    }
}
