namespace Dolany.Ai.Core.Base
{
    /// <summary>
    /// 异步辅助工具接口
    /// </summary>
    public interface IAITool
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        bool Enabled { get; set; }
        /// <summary>
        /// 工作函数
        /// </summary>
        void Work();
    }
}
