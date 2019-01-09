namespace Dolany.Game.IncantationGame
{
    using System.Collections.Generic;

    using Dolany.Database.Incantation;

    public class Player
    {
        public long QQNum { get; set; }

        public int MaxHP { get; set; }

        public int CurrentHP { get; set; }

        public List<IncaMagic> MagicList { get; set; } = new List<IncaMagic>();

        public List<IBuff> BuffList { get; set; } = new List<IBuff>();
    }
}
