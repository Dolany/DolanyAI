using System.Collections.Generic;
using System.Linq;

namespace Dolany.Game.FreedomMagic
{
    public class FMMagic
    {
        public string Name { get; set; }

        public string SingWords { get; set; }

        public List<FMEffect> Effects { get; set; }

        public int MagicLevel => Effects.Sum(e => e.Level);

        public int MagicCost => Effects.Sum(e => e.Cost);

        public override string ToString()
        {
            return $"名称：{Name}\r" +
                   $"效果：{string.Join(",", Effects.Select(e => e.Description))}\r" +
                   $"等级：{MagicLevel}\r" +
                   $"MP：{MagicCost}\r" +
                   $"咒文：{SingWords}";
        }
    }
}
