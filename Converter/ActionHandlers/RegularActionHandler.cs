using Converter.ActionHandlers;
using SimcToBrConverter.Conditions;

public class RegularActionHandler : BaseActionHandler
{
    public RegularActionHandler(List<IConditionConverter> conditionConverters) : base(conditionConverters) { }

    public override bool CanHandle(string action)
    {
        return !(action.Contains("target_if=") || action.Contains("use_item"));
    }

    protected override (string command, string condition) ParseAction(string action)
    {
        var parts = action.Split(",if=");
        var command = parts[0];
        var condition = parts.Length > 1 ? parts[1] : string.Empty;

        return (command, condition);
    }
}