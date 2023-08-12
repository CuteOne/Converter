namespace SimcToBrConverter.ActionLines
{    public class MultipleActionLineResult : IParseResult
    {
        public List<ActionLine> ActionLines { get; }

        internal MultipleActionLineResult(List<ActionLine> actionLines)
        {
            ActionLines = actionLines ?? new List<ActionLine>();
        }
    }
}
