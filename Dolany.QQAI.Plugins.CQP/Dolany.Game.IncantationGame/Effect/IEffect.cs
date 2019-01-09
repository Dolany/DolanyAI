namespace Dolany.Game.IncantationGame.Effect
{
    using Dolany.Database.Incantation;

    public interface IEffect
    {
        EffectKindEnum Kind { get; set; }

        string Name { get; set; }

        int Value { get; set; }

        IEffect CreateNew(int effectLevel);

        string GetDescrption();

        string DeEffect(IncaGameMgr mgr, IncaCharactor aimCharactor);
    }

    public enum EffectKindEnum
    {
        Damage
    }
}
