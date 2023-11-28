using SimcToBrConverter.ActionLines;

namespace SimcToBrConverter.Utilities
{
    public class ConversionResult
    {
        public ActionLine ActionLine { get; set; }
        public List<string> NotConvertedConditions { get; set; }
        public String Result { get; set; }

        public ConversionResult(ActionLine actionLine, List<string> notConvertedConditions, string result)
        {
            ActionLine = actionLine;
            NotConvertedConditions = notConvertedConditions;
            Result = result;
        }
    }
}
