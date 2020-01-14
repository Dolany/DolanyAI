namespace Dolany.Ai.Core.Base
{
    public interface IAITool
    {
        bool Enabled { get; set; }
        void Work();
    }
}
