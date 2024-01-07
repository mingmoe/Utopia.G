// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Security.Cryptography;
using Utopia.Core.Collections;
using Utopia.Core.Events;
using Utopia.Core.Utilities;

namespace Utopia.Core.Translation;

/// <summary>
/// 默认翻译管理器，线程安全。
/// </summary>
public class TranslationManager : SafeList<ITranslationProvider>, ITranslationManager
{
    private readonly object _lock = new();

    public long TranslateID { get; private set; }

    public IEventManager<EventWithParam<ITranslationManager>> TranslateUpdatedEvent { get; } = new
        EventManager<EventWithParam<ITranslationManager>>();

    private LanguageID _current = new();

    public LanguageID CurrentLanguage
    {
        get
        {
            lock (_lock)
            {
                return _current;
            }
        }
        set
        {
            lock (_lock)
            {
                _current = value;
            }
        }
    }

    public TranslationManager() =>
        // 使用加密安全随机数填充初始翻译ID，只填充一个int
        TranslateID =
            RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);

    public void UpdateTranslate()
    {
        lock (_lock)
        {
            TranslateID++;
            TranslateUpdatedEvent.Fire(new EventWithParam<ITranslationManager>(this));
        }
    }

    public bool Contains(LanguageID language, string item) => TryGetTranslate(language, item, out _);

    public bool TryGetTranslate(LanguageID language, string item, [NotNullWhen(true)] out string? result)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(item);

        using var @lock = this.EnterWriteLock();

        foreach(var provider in this)
        {
            if(provider.TryGetItem(language,item,out result))
            {
                return true;
            }
        }

        result = null;
        return false;
    }
}
