using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;
using Utopia.Core.Translate;
using Utopia.Core.Utilities;

namespace Utopia.ResourcePack;

public abstract class CorePluginBase : IPluginBase
{
    public readonly static Guuid ID = new("utopia", "core");

    public readonly static TranslatedString NAME = new("core");
    public readonly static TranslatedString DESC = new("The core plugin of utopia.");

    public ITranslatedString Name => NAME;

    public ITranslatedString Description => DESC;

    public string License => "AGPL v3";

    public Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    public Guuid Id => ID;

    public string Homepage => "https://github.com/mingmoe/utopia";

    public abstract void Active();
}
