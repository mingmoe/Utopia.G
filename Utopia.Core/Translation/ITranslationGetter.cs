// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Utilities;

namespace Utopia.Core.Translation;

public interface ITranslationGetter
{
    Guuid? DefaultProvider { get; }

    string I18n(string text,string? comment = null,Guuid? provider = null);

    string I18nf(string text, Dictionary<string,object?> args, string? comment = null, Guuid? provider = null);
}