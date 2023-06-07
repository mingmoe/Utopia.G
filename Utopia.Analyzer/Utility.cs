using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utopia.Analyzer.TranslateFinder;

namespace Utopia.Analyzer;

public class Utility
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static Microsoft.CodeAnalysis.Project[] OpenSlnToProject(string sln, string? projGuuid)
    {
        var msWorkspace = MSBuildWorkspace.Create();
        var t = msWorkspace.OpenSolutionAsync(sln!);
        t.Wait();
        var solution = t.Result;

        var projs = projGuuid == null ? solution.Projects.ToArray() :
            new Microsoft.CodeAnalysis.Project[1]
            { solution.GetProject(ProjectId.CreateFromSerialized(Guid.Parse(projGuuid)))
                ?? throw new ArgumentException($"the project guuid {projGuuid} not found") };

        return projs;
    }

    public static async Task<Compilation[]> GetCompilation(Microsoft.CodeAnalysis.Project[] projects)
    {
        List<Task<Compilation?>> compilations = new();

        foreach (var project in projects)
        {
            compilations.Add(project.GetCompilationAsync());
        }
        await Task.WhenAll(compilations.ToArray());

        List<Compilation> result = new();
        for (var index = 0; index != projects.Length; index++)
        {
            if (compilations[index].Result == null)
            {
                _logger.Error("failed to compile project {project}", projects[index]);
                continue;
            }
            result.Add(compilations[index].Result!);
        }

        return result.ToArray();
    }
}
