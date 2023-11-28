using SimcToBrConverter.Utilities;

namespace SimcToBrConverter.SpecialHandlers
{
    internal class MaxEnergySpecialHandler : BaseSpecialHandler
    {
        public override bool CanHandle()
        {
            return Program.currentActionLine.SpecialHandling.Contains("max_energy=");
        }

        public override void Handle()
        {
            List<string> specialHandling = SplitSpecialHandling();
            foreach (var entry in specialHandling)
            {
                var maxEnergyValue = entry.Replace("max_energy=", "").Trim();
                if (!string.IsNullOrEmpty(Program.currentActionLine.Action) && !string.IsNullOrEmpty(maxEnergyValue))
                {
                    if (maxEnergyValue == "1" && Program.currentActionLine.Action == "ferocious_bite")
                        ModifyConditions.Add(Program.currentActionLine, "energy>=50");
                }
            }
        }
    }
}
