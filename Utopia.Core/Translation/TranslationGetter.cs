// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jeffijoe.MessageFormat;
using SharpCompress;
using Utopia.Core.Utilities;

namespace Utopia.Core.Translation;
public class TranslationGetter : ITranslationGetter
{
    public required ITranslateManager TranslateManager { private get; init; }

    private readonly object _lock = new();

    private MessageFormatter? _messageFormatter = null;

    private MessageFormatter _GetFormatter()
    {
        lock (_lock)
        {
            if (_messageFormatter == null)
            {
                _messageFormatter = new(true, TranslateManager.CurrentLanguage.Language);
            }

            return _messageFormatter;
        }
    }

    public Guuid? DefaultProvider { get; init; }

    public string I18n(string text, string? comment, Guuid? provider)
    {
        return I18nf(text, [],comment, provider);
    }

    public string I18nf(string text,
        Dictionary<string,object?> args,
        string? comment/* useful */ ,
        Guuid? provider)
    {
        if(!TranslateManager.TryGetTranslate(TranslateManager.CurrentLanguage,
            provider,
            text,
            out string? msg
            ))
        {
            return _GetFormatter().FormatMessage(text, args);
        }

        return _GetFormatter().FormatMessage(msg!, args);
    }
}
