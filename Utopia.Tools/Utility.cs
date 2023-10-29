// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using NLog;

namespace Utopia.Tools;

public class Utility
{
    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    public static Microsoft.CodeAnalysis.Project[] OpenSlnToProject(string sln, string? projGuuid)
    {
        var msWorkspace = MSBuildWorkspace.Create();
        Task<Solution> t = msWorkspace.OpenSolutionAsync(sln!);
        t.Wait();
        Solution solution = t.Result;

        Project[] projs = projGuuid == null ? solution.Projects.ToArray() :
            new Microsoft.CodeAnalysis.Project[1]
            { solution.GetProject(ProjectId.CreateFromSerialized(Guid.Parse(projGuuid)))
                ?? throw new ArgumentException($"the project guuid {projGuuid} not found") };

        return projs;
    }

    public static async Task<Compilation[]> GetCompilation(Microsoft.CodeAnalysis.Project[] projects)
    {
        List<Task<Compilation?>> compilations = new();

        foreach (Project project in projects)
        {
            compilations.Add(project.GetCompilationAsync());
        }
        _ = await Task.WhenAll(compilations.ToArray());

        List<Compilation> result = new();
        for (int index = 0; index != projects.Length; index++)
        {
            if (compilations[index].Result == null)
            {
                s_logger.Error("failed to compile project {project}", projects[index]);
                continue;
            }
            result.Add(compilations[index].Result!);
        }

        return result.ToArray();
    }
}
