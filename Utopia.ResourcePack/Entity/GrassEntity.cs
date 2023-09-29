using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Translate;
using Utopia.Core.Utilities;

namespace Utopia.ResourcePack.Entity;

public static class GrassEntity
{
    public static readonly Guuid ID = new("utopia", "core", "entity", "grass");

    public static readonly TranslateKey TranslateKey = new(
        "utopia:core:translate:entity:grass", "the name of the entity `grass`");
}
