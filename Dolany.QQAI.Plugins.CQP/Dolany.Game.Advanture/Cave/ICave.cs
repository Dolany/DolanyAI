namespace Dolany.Game.Advanture.Cave
{
    public abstract class ICave
    {
        public virtual string Description => "未知";

        public virtual bool IsNeedRefresh => true;

        public abstract CaveType Type { get; set; }

        public bool Visible { get; set; } = false;
    }

    public enum CaveType
    {
        怪兽,
        陷阱,
        宝箱
    }
}
