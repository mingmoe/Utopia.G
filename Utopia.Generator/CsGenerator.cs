using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Generator;

public class CsGenerator
{

    public List<string> Usings = new();

    public string? Namespace = null;

    public List<string> Lines = new();


    public string Generate()
    {
        StringBuilder sb = new();

        sb.AppendLine("// This file was generated by source generator.");

        foreach (var s in Usings)
        {
            sb.Append("using ").Append(s).AppendLine(";");
        }

        if(Namespace != null)
        {
            sb.AppendLine($"namespace {Namespace};");
        }

        foreach(var s in Lines)
        {
            sb.AppendLine(s);
        }

        return sb.ToString();
    }    

}
