namespace Dolany.Game.IncantationGame
{
    public interface IBuff
    {
        BuffKind Kind { get; set; }

        string Name { get; set; }

        int Value { get; set; }

        int DuringTurn { get; set; }
    }

    public enum BuffKind
    {
        DefenceAbs,
        DefenceRel,
        SuccessRateUp
    }
}
