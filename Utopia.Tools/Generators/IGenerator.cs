namespace Utopia.Tools.Generators;

/// <summary>
/// The same from microsoft is shit. So use this.
/// </summary>
public interface IGenerator
{
    /// <summary>
    /// Note: this may be call many times
    /// </summary>
    void Execute(GeneratorOption option);
}
