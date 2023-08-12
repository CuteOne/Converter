using SimcToBrConverter.Conditions;
using static SimcToBrConverter.ActionLineParser;

namespace SimcToBrConverter.ActionHandlers
{
    public class RegularActionHandler : BaseActionHandler
    {
        public RegularActionHandler(List<IConditionConverter> conditionConverters) : base(conditionConverters) { }

        public override bool CanHandle(ActionLine actionLine)
        {
            return !(actionLine.Action.Contains("target_if=") || actionLine.Action.Contains("use_item"));
        }

        protected override ActionLine ParseAction(string action)
        {
            throw new NotImplementedException();
        }
    }
}