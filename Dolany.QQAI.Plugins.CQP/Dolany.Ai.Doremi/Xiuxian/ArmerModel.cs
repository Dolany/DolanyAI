using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Xiuxian
{
    public class ArmerModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string Kind { get; set; }

        public int Value { get; set; }

        public int Price { get; set; }

        public int Rate { get; set; }

        public string ArmerTag { get; set; }

        private string KindString
        {
            get
            {
                return Kind switch
                {
                    "Shield" => "防具",
                    "Weapon" => "武器",
                    _ => ""
                };
            }
        }

        private string ValueString
        {
            get
            {
                var preString = Kind switch
                {
                    "Shield" => "防御力",
                    "Weapon" => "攻击力",
                    _ => string.Empty
                };

                return $"{preString}：{Value}";
            }
        }

        public override string ToString()
        {
            return $"名称：{Name}\r类别：{KindString}\r{ValueString}\r价格：{Price}\r稀有度：{Rate}";
        }
    }
}
