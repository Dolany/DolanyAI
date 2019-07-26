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

        private string KindString
        {
            get
            {
                switch (Kind)
                {
                    case "Shield":
                        return "防具";
                    case "Weapon":
                        return "武器";
                    default:
                        return "";
                }
            }
        }

        private string ValueString
        {
            get
            {
                var preString = string.Empty;
                switch (Kind)
                {
                    case "Shield":
                        preString = "防御力";
                        break;
                    case "Weapon":
                        preString = "攻击力";
                        break;
                }

                return $"{preString}：{Value}";
            }
        }

        public override string ToString()
        {
            return $"名称：{Name}\r类别：{KindString}\r{ValueString}\r价格：{Price}\r稀有度：{Rate}";
        }
    }
}
