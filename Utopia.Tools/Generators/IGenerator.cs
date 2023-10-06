namespace Utopia.Tools.Generators;

/// <summary>
/// The same from microsoft is shit. So use this.
/// </summary>
internal interface IGenerator
{
    IFileSystem TargetProject { get; set; }

    string RootNamespace { get; set; }

    /// <summary>
    /// Note: this may be call many times,with different <see cref="TargetProject"/> and other properties.
    /// </summary>
    void Execute();
}
