﻿using SimcToBrConverter.ActionHandlers;
using SimcToBrConverter.ActionLines;
using SimcToBrConverter.Conditions;

public class ActionListActionHandler : BaseActionHandler
{
    public ActionListActionHandler(List<IConditionConverter> conditionConverters) : base(conditionConverters) { }

    public override bool CanHandle(ActionLine actionLine)
    {
        return actionLine.Action.Contains("run_action_list") || actionLine.Action.Contains("call_action_list");
    }

    protected override ActionLine CheckHandling(ActionLine actionLine)
    {
        if (actionLine.Action.Contains("run_action_list") || actionLine.Action.Contains("call_action_list"))
        {
            // Extract the name of the action list from the SpecialHandling property
            var actionListName = actionLine.SpecialHandling.Replace("name=", "").Trim();

            // Return a new ActionLine with the extracted action list name as the action
            return new ActionLine(actionLine.ListName, actionListName, actionLine.SpecialHandling, actionLine.Condition);
        }

        // If the action doesn't match the expected patterns, return the original ActionLine
        return actionLine;
    }

}