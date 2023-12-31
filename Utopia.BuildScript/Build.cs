using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

namespace Utopia.BuildScript;

class Build : NukeBuild , INativeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.UnitTest);

    [GitVersion]
    readonly GitVersion GitVersion;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Restore => (_) =>
    {
        return _
        .Description("download and process dependencies")
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore((config) =>
            {
                return config;
            });
        });
    };

    Target GenerateCodes => (_) =>
    {
        return _
        .Description("call Utopia.Tool to generate codes")
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild((config) =>
            {
                return config
                .SetConfiguration(Configuration.Mode)
                .EnableNoRestore()
                .SetProjectFile(RootDirectory.GetProjectFilePath("Tool"));
            });
            DotNetTasks.DotNetRun((config) =>
            {
                return config
                .SetConfiguration(Configuration.Mode)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetProjectFile(RootDirectory.GetProjectFilePath("Tool"))
                .SetApplicationArguments($"generate --configuration {RootDirectory / "utopia.xml"} --project {RootDirectory}");
            });
        })
        .DependsOn(Restore);
    };

    Target CompileCSharp => (_) =>
    {
        return _
        .Description("compile C# projects")
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild((config) =>
            {
                return config
                .EnableNoRestore()
                .SetConfiguration(Configuration.Mode);
            });
        })
        .DependsOn(GenerateCodes)
        .DependsOn(Restore);
    };

    Target CompileAll => (_) =>
    {
        return _
        .Description("compile both native libraries and C# projects")
        .DependsOn(CompileCSharp)
        .DependsOn<INativeBuild>();
    };

    Target UnitTest => (_) =>
    {
        return _
        .Description("run unit tests")
        .Executes(() =>
        {
            DotNetTasks.DotNetTest((config) =>
            {
                return config
                .EnableNoRestore()
                .SetProjectFile(RootDirectory.GetProjectFilePath("Test"));
            });
        })
        .DependsOn(CompileAll);
    };

    Target Release => (_) =>
    {
        return _
        .Description("release game")
        .Requires(() => Configuration.Mode == nameof(Configuration.Release))
        .DependsOn(CompileAll);
    };
}
