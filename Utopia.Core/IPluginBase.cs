namespace Utopia.Core;
public interface IPluginBase : IPluginInformation, IDisposable
{

    /// <summary>
    /// 在这个函数中,而不是在构造函数中进行初始化
    /// </summary>
    void Active();

}
