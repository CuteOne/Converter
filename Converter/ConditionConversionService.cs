using SimcToBrConverter.ActionLines;
using SimcToBrConverter.ConditionConverters;
using SimcToBrConverter.Utilities;
using System.Text;

namespace SimcToBrConverter
{
    public class ConditionConversionService
    {
        private readonly List<IConditionConverter> _conditionConverters;

        public ConditionConversionService(List<IConditionConverter> conditionConverters)
        {
            _conditionConverters = conditionConverters;
        }

        private static void AddToLocalList(string local)
        {
            // Remove the "not " prefix
            if (local.StartsWith("not "))
            {
                local = local[4..];
            }

            // Remove any # characters
            local = local.Replace("#", "");

            // Trim whitespace
            local = local.Trim();

            if (!string.IsNullOrWhiteSpace(local) && !local.Equals("false"))
            {
                Program.Locals.Add(local);
            }
        }

        public void ConvertCondition()
        {
            ActionLine actionLine = Program.currentActionLine;
            List<string> notConvertedConditions = Program.notConverted;
            var convertedConditions = new StringBuilder();

            actionLine.Conditions = ConditionConverterUtility.SplitCondition(actionLine.Condition);

            for (int i = 0; i < actionLine.Conditions.Count; i++)
            {
                string conditionPart = actionLine.Conditions[i];

                // Check for special conditions that should be removed or replaced
                /*if (IsUnconvertableConditionPart(conditionPart))
                {
                    string fullCondition = actionLine.Condition;//convertedConditions.ToString() + string.Join("", actionLine.Conditions.GetRange(i, actionLine.Conditions.Count - i));
                    var (replacement, leftBoundary, rightBoundary) = DetermineReplacementForUnconvertable(fullCondition, i);

                    // Ensure boundaries are within the StringBuilder's length
                    leftBoundary = Math.Max(leftBoundary, 0);
                    rightBoundary = Math.Min(rightBoundary, fullCondition.Length - 1);

                    // Calculate the length to remove and ensure it's within bounds
                    int lengthToRemove = Math.Min(rightBoundary - leftBoundary + 1, fullCondition.Length - leftBoundary);

                    // Remove and replace
                    convertedConditions.Remove(leftBoundary, lengthToRemove);
                    convertedConditions.Insert(leftBoundary, replacement);

                    // Adjust the loop index - set it to the position after the replaced section
                    i = leftBoundary + replacement.Length - 1;
                    continue;
                }*/

                // Handling special operators >? and <?
                if (conditionPart == ">?" || conditionPart == "<?")
                {
                    string operatorFunction = conditionPart == ">?" ? "math.max" : "math.min";

                    // Gather all relevant parts of the expression on the left side of the operator
                    string leftSubExpression = GatherSubExpression(actionLine.Conditions, i - 1, true);

                    // Get the start and end indices of the right sub-expression
                    int rightStartIndex = i + 1;
                    int rightEndIndex = GetEndIndexForSubExpression(actionLine.Conditions, rightStartIndex, false);

                    // Gather all relevant parts of the expression on the right side of the operator
                    string rightSubExpression = GatherSubExpression(actionLine.Conditions, rightStartIndex, false);

                    // Replace the left sub-expression in convertedConditions with the new operator expression
                    string newExpression = $"{operatorFunction}({leftSubExpression},{rightSubExpression})";
                    string existingConditions = convertedConditions.ToString();
                    int leftSubExpressionStart = existingConditions.LastIndexOf(leftSubExpression);
                    if (leftSubExpressionStart != -1)
                    {
                        convertedConditions.Remove(leftSubExpressionStart, leftSubExpression.Length);
                    }
                    convertedConditions.Append(newExpression);

                    // Skip the parts of the right sub-expression as they have been handled
                    i = rightEndIndex;
                }
                else if (ConditionConverterUtility.IsOperatorOrNumber(conditionPart))
                {
                    convertedConditions.Append(conditionPart);
                }
                else
                {
                    string convertedPart = ConvertConditionPart(conditionPart, actionLine.Action, ref notConvertedConditions);
                    convertedConditions.Append(convertedPart);
                }
            }

            // Convert logical operators to their Lua equivalents
            string checkConditions = StringUtilities.CheckForOr(convertedConditions.ToString());
            var finalConvertedCondition = ConditionConverterUtility.ConvertOperatorsToLua(checkConditions);
            actionLine.Condition = finalConvertedCondition;

            Program.currentActionLine = actionLine;
            Program.notConverted = notConvertedConditions;
        }

        private static string DetermineLogicalReplacement(string condition, int leftBoundary, int rightBoundary)
        {
            // Check for '&' or '|' in the surrounding context
            string subCondition = condition.Substring(leftBoundary, rightBoundary - leftBoundary + 1);
            bool containsAndOperator = subCondition.Contains('&');
            bool containsOrOperator = subCondition.Contains('|');

            if (containsAndOperator) return "true";
            if (containsOrOperator) return "false";

            return "PLACEHOLDER_REMOVE"; // Default, though this scenario should ideally be handled more gracefully
        }

        private static int FindLogicalExpressionBoundary(string condition, int index, bool searchLeft)
        {
            int depth = 0;
            int i = index;

            while (searchLeft ? i >= 0 : i < condition.Length)
            {
                if (condition[i] == '(')
                {
                    if (!searchLeft) depth++;
                    else if (--depth < 0) break;
                }
                else if (condition[i] == ')')
                {
                    if (searchLeft) depth++;
                    else if (--depth < 0) break;
                }
                else if ((condition[i] == '&' || condition[i] == '|') && depth == 0)
                {
                    break;
                }

                i += searchLeft ? -1 : 1;
            }

            return i;
        }

        private static (string, int, int) DetermineReplacementForUnconvertable(string fullCondition, int currentIndex)
        {
            int leftBoundary = currentIndex, rightBoundary = currentIndex;
            bool foundOperator = false;
            string replacement = "PLACEHOLDER_REMOVE"; // Default

            // Search left for 'and' or 'or', stopping at '('
            for (int i = currentIndex - 1; i >= 0 && !foundOperator; i--)
            {
                if (fullCondition[i] == '(') break;
                if (fullCondition[i] == '&')
                {
                    replacement = "true";
                    leftBoundary = i;
                    foundOperator = true;
                }
                else if (fullCondition[i] == '|')
                {
                    replacement = "false";
                    leftBoundary = i;
                    foundOperator = true;
                }
            }

            // If no operator was found on the left, search right
            if (!foundOperator)
            {
                for (int i = currentIndex + 1; i < fullCondition.Length; i++)
                {
                    if (fullCondition[i] == ')') break;
                    if (fullCondition[i] == '&')
                    {
                        replacement = "true";
                        rightBoundary = i;
                        break;
                    }
                    else if (fullCondition[i] == '|')
                    {
                        replacement = "false";
                        rightBoundary = i;
                        break;
                    }
                }
            }

            return (replacement, leftBoundary, rightBoundary);

            /*int leftBoundary = currentIndex, rightBoundary = currentIndex;
            bool foundOperator = false;
            string replacement = "PLACEHOLDER_REMOVE"; // Default

            // Search left for 'and' or 'or', stopping at '('
            for (int i = currentIndex - 1; i >= 0 && !foundOperator; i--)
            {
                if (fullCondition[i] == '(') break;
                if (fullCondition[i] == '&')
                {
                    replacement = "true";
                    leftBoundary = i;
                    foundOperator = true;
                }
                else if (fullCondition[i] == '|')
                {
                    replacement = "false";
                    leftBoundary = i;
                    foundOperator = true;
                }
            }

            // If no operator was found on the left, search right
            if (!foundOperator)
            {
                for (int i = currentIndex + 1; i < fullCondition.Length; i++)
                {
                    if (fullCondition[i] == ')') break;
                    if (fullCondition[i] == '&')
                    {
                        replacement = "true";
                        rightBoundary = i;
                        break;
                    }
                    else if (fullCondition[i] == '|')
                    {
                        replacement = "false";
                        rightBoundary = i;
                        break;
                    }
                }
            }

            // Expand boundaries to include the entire expression within parentheses
            leftBoundary = ExpandBoundaryToLeft(fullCondition, leftBoundary);
            rightBoundary = ExpandBoundaryToRight(fullCondition, rightBoundary);

            return (replacement, leftBoundary, rightBoundary);*/
        }

        // Methods to expand boundaries
        private static int ExpandBoundaryToLeft(string condition, int startIndex)
        {
            int depth = 0;
            for (int i = startIndex; i >= 0; i--)
            {
                if (condition[i] == ')') depth++;
                else if (condition[i] == '(')
                {
                    depth--;
                    if (depth < 0) return i;
                }
            }
            return Math.Max(startIndex, 0); // Return the startIndex if no matching parenthesis is found
        }

        private static int ExpandBoundaryToRight(string condition, int startIndex)
        {
            int depth = 0;
            for (int i = startIndex; i < condition.Length; i++)
            {
                if (condition[i] == '(') depth++;
                else if (condition[i] == ')')
                {
                    depth--;
                    if (depth < 0) return i;
                }
            }
            return Math.Min(startIndex, condition.Length - 1); // Return the startIndex if no matching parenthesis is found
        }




        /*private static string DetermineReplacementForUnconvertable(string fullCondition, int currentIndex)
        {
            string leftReplacement = CheckForLogicalOperatorAndBoundaries(fullCondition, currentIndex - 1, true);
            if (leftReplacement != null)
            {
                return leftReplacement;
            }

            string rightReplacement = CheckForLogicalOperatorAndBoundaries(fullCondition, currentIndex + 1, false);
            if (rightReplacement != null)
            {
                return rightReplacement;
            }

            return "PLACEHOLDER_REMOVE"; // Default to false if no logical operator is found
        }*/

        private static string CheckForLogicalOperatorAndBoundaries(string condition, int index, bool isLeftSearch)
        {
            if (isLeftSearch)
            {
                // Search left for 'and' or 'or', stopping at '('
                for (int i = index - 1; i >= 0; i--)
                {
                    if (condition[i] == '(') break;
                    if (condition[i] == '&') return "true";
                    if (condition[i] == '|') return "false";
                }
            }
            else
            {
                // Search right for 'and' or 'or', stopping at ')'
                for (int i = index + 1; i < condition.Length; i++)
                {
                    if (condition[i] == '(') break;
                    if (condition[i] == '&') return "true";
                    if (condition[i] == '|') return "false";
                }
            }

            return null; // Return null if no logical operator is found within the boundaries
        }






        private static int GetEndIndexForSubExpression(List<string> conditions, int startIndex, bool reverse)
        {
            int index = startIndex;
            while (index >= 0 && index < conditions.Count)
            {
                string part = conditions[index];

                // Check for end of sub-expression
                if (part == "&" || part == "|" || part == "(" || part == ")" || part == ",")
                {
                    return reverse ? index - 1 : index;
                }

                index += reverse ? -1 : 1;
            }
            return reverse ? 0 : conditions.Count - 1;
        }


        private string GatherSubExpression(List<string> conditions, int startIndex, bool reverse)
        {
            var subExpression = new StringBuilder();
            int index = startIndex;
            while (index >= 0 && index < conditions.Count)
            {
                string part = conditions[index];

                // Check if the part is an operator or number, if not, convert it
                if (!ConditionConverterUtility.IsOperatorOrNumber(part))
                {
                    part = ConvertConditionPart(part, Program.currentActionLine.Action, ref Program.notConverted);
                }

                // Check for end of sub-expression
                if (part == "&" || part == "|" || (reverse && part == "(") || (!reverse && part == ")") || part == ",")
                {
                    // If reverse, we stop before adding this part; otherwise, we stop after adding it
                    if (reverse) break;
                    subExpression.Append(part);
                    break;
                }

                // Add the part to the sub-expression
                if (reverse)
                {
                    subExpression.Insert(0, part);
                    index--;
                }
                else
                {
                    subExpression.Append(part);
                    index++;
                }
            }

            return subExpression.ToString().Trim();
        }

        private static bool IsUnconvertableConditionPart(string conditionPart)
        {
            List<string> unconvertableConditions = new()
            {
                "raid_event.adds.in",
                "raid_event.movement.in",
            };
            foreach (var condition in unconvertableConditions)
            {
                if (conditionPart.Equals(condition))
                    return true;
            }
            return false;
        }

        public (ActionLine, List<string>) ConvertCondition(ActionLine actionLine)
        {
            var tempActionLine = Program.currentActionLine;
            Program.currentActionLine = actionLine;
            Program.notConverted = new List<string>();
            ConvertCondition();
            var convertedActionLine = Program.currentActionLine;
            var notConvertedConditions = Program.notConverted;
            Program.currentActionLine = tempActionLine;
            return (convertedActionLine, notConvertedConditions);
        }

        string ConvertConditionPart(string conditionPart, string action, ref List<string> notConvertedConditions)
        {
            var converter = _conditionConverters.FirstOrDefault(c => c.CanConvert(conditionPart));
            if (converter != null)
            {
                var (convertedPart, notConvertedParts) = converter.ConvertPart(conditionPart, action);
                var local = convertedPart.Split('.')[0];
                AddToLocalList(local);
                notConvertedConditions.AddRange(notConvertedParts);
                return convertedPart;
            }
            else
            {
                //notConvertedConditions.Add(conditionPart);
                return conditionPart;
            }
        }
    }
}
