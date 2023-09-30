#region copyright
// This file(may named Utility.cs) is a part of the project: Utopia.Analyzer.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Analyzer.
//
// Utopia.Analyzer is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Analyzer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Analyzer. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using NLog;

namespace Utopia.Tools;

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
