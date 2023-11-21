namespace SimcToBrConverter.Utilities
{
    public readonly struct PowerType
    {
        public string SimCText { get; }
        public string BrText { get; }

        private PowerType(string simCText, string brText)
        {
            SimCText = simCText;
            BrText = brText;
        }

        // Static array of all power types
        public static readonly PowerType[] PowerTypes =
        {
            new("mana", "mana"),
            new("rage", "rage"),
            new("focus", "focus"),
            new("energy", "energy"),
            new("combo_points", "comboPoints"),
            new("runes", "runes"),
            new("runic_power", "runicPower"),
            new("soul_shards", "soulShards"),
            new("lunar_power", "lunarPower"),
            new("holy_power", "holyPower"),
            new("alternate", "alternate"),
            new("maelstrom", "maelstrom"),
            new("chi", "chi"),
            new("insanity", "insanity"),
            new("obsolete", "obsolete"),
            new("obsolete2", "obsolete2"),
            new("arcane_charges", "arcaneCharges"),
            new("fury", "fury"),
            new("pain", "pain"),
            new("essence", "essence"),
            new("runeblood", "runeBlood"),
            new("runefrost", "runeFrost"),
            new("runeunholy", "runeUnholy"),
        };

        public string ToSimcString()
        {
            return SimCText;
        }

        public string ToBrString()
        {
            return BrText;
        }

        // Static method to identify power type from a condition string
        public static bool IsPowerType(string checkString)
        {
            foreach (var powerType in PowerTypes)
                if (checkString.StartsWith(powerType.SimCText) || checkString.StartsWith(powerType.BrText))
                    return true;

            return false; // No matching power type found
        }
    }

}
