//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System.Security.Cryptography;
using Utopia.Core.Collections;
using Utopia.Core.Events;
using Utopia.Core.Utilities;

namespace Utopia.Core.Translate;

/// <summary>
/// 默认翻译管理器，线程安全。
/// </summary>
public class TranslateManager : SafeDictionary<Guuid, ITranslateProvider>, ITranslateManager
{
    private readonly object _lock = new();

    public long TranslateID { get; private set; }

    public IEventManager<EventWithParam<ITranslateManager>> TranslateUpdatedEvent { get; } = new
        EventManager<EventWithParam<ITranslateManager>>();

    public TranslateManager()
    {
        // 使用加密安全随机数填充初始翻译ID，只填充一个int
        this.TranslateID =
            RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
    }
    public void UpdateTranslate()
    {
        lock (this._lock)
        {
            this.TranslateID++;
            this.TranslateUpdatedEvent.Fire(new EventWithParam<ITranslateManager>(this, true));
        }
    }

    public bool Contains(TranslateIdentifence language, Guuid? translateProviderId, Guuid translateItemId)
    {
        return this.TryGetTranslate(language, translateProviderId, translateItemId, out _);
    }

    public bool TryGetTranslate(TranslateIdentifence language, Guuid? translateProviderId, Guuid translateItemId, out string? result)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(translateItemId);

        lock (this._lock)
        {
            if (translateProviderId is not null &&
            this.TryGetValue(translateProviderId, out ITranslateProvider? value))
            {
                if (value!.TryGetItem(language, translateItemId, out result))
                {
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }
            else if (translateProviderId is null)
            {
                var vs = this.ToArray();

                foreach (var provider in vs)
                {
                    if (provider.Value.TryGetItem(language, translateItemId, out result))
                    {
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }
    }
}
