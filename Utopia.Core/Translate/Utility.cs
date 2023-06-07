using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core.Translate;

public static class Utility
{
    public static ITranslatedString Activate(this TranslateManager manager,
        TranslateKey key,
        TranslateIdentifence id,
        object data)
    {
        var msg = manager.Activate(key, id);

        return new ICUTranslatedString(msg, data);
    }
}
