namespace Dolany.Game.TouhouCardWar.Cards.HeroCards
{
    using Dolany.Game.TouhouCardWar.Effect;

    public interface IHeroCard : ICard
    {
        IEffect Skill { get; set; }
    }
}
