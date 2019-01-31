namespace Dolany.Game.FreedomMagic
{
    public class FMEffect
    {
        public string Type { get; set; }

        public int Level { get; set; }

        public int Cost { get; set; }

        public int Value { get; set; }

        public string Description { get; set; }
    }

    public enum EffectType
    {
        Heal,
        MPRestore,

        Harm_Fire,
        Harm_Ice,
        Harm_Wind,
        Harm_Thurnder,
        Harm_Physics,
        Harm_Real,

        Buff_HarmIncrease_Physics,
        Buff_HarmIncrease_Element,

        Debuff_HarmDecrease_Physics,
        Debuff_HarmDecrease_Magic,
        Debuff_HarmDecrease_Relational_Physics,
        Debuff_HarmDecrease_Relational_Magic,
        Debuff_Poison,
        Debuff_Blame,
        Debuff_Silence,

        CleanUp,
        CleanUp_All,

        Shell,
        Shell_Temp,

        HPSteal
    }
}
