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
    public ITranslationManager Manager { get; init; }

    public LanguageID CurrentLanguage { get; set; }

    private System.Lazy<MessageFormatter> _messageFormatter;

    public TranslationGetter(ITranslationManager manager,LanguageID id)
    {
        Manager = manager;
        CurrentLanguage = id;
        _messageFormatter = new(() =>
        {
            return new(true, CurrentLanguage.Location);
        }, true);
    }

    public string I18n(string text, string comment)
    {
        if (Manager.TryGetTranslate(CurrentLanguage,
            text,
            out var msg
            ))
        {
            return msg;
        }

        return text;
    }

    public string I18nf(string text,
        Dictionary<string,object?> args,
        string comment/* useful */)
    {
        if(Manager.TryGetTranslate(CurrentLanguage,
            text,
            out string? msg
            ))
        {
            return _messageFormatter.Value.FormatMessage(msg, args);
        }

        return _messageFormatter.Value.FormatMessage(text, args);
    }
}
