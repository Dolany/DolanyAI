namespace Dolany.Ai.Core.AITools
{
    public interface IAITool
    {
        bool Enabled { get; set; }
        void Work();
    }
}
