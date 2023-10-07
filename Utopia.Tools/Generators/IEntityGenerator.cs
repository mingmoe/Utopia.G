using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Tools.Generators;

public interface IEntityGenerator
{

    void Generate(string path,EntityInfo info,GeneratorOption option);
}
