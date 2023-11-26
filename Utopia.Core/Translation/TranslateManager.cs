// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Security.Cryptography;
using Utopia.Core.Collections;
using Utopia.Core.Events;
using Utopia.Core.Utilities;

namespace Utopia.Core.Translation;

/// <summary>
/// 默认翻译管理器，线程安全。
/// </summary>
public class TranslateManager : SafeDictionary<Guuid, ITranslateProvider>, ITranslateManager
{
    private readonly object _lock = new();

    public long TranslateID { get; private set; }

    public IEventManager<EventWithParam<ITranslateManager>> TranslateUpdatedEvent { get; } = new
        EventManager<EventWithParam<ITranslateManager>>();

    public TranslateManager() =>
        // 使用加密安全随机数填充初始翻译ID，只填充一个int
        TranslateID =
            RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
    public void UpdateTranslate()
    {
        lock (_lock)
        {
            TranslateID++;
            TranslateUpdatedEvent.Fire(new EventWithParam<ITranslateManager>(this));
        }
    }

    public bool Contains(TranslateIdentifence language, Guuid? translateProviderId, Guuid translateItemId) => TryGetTranslate(language, translateProviderId, translateItemId, out _);

    public bool TryGetTranslate(TranslateIdentifence language, Guuid? translateProviderId, Guuid translateItemId, out string? result)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(translateItemId);

        lock (_lock)
        {
            if (translateProviderId.HasValue is not false &&
            TryGetValue(translateProviderId.Value, out ITranslateProvider? value))
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
                KeyValuePair<Guuid, ITranslateProvider>[] vs = ToArray();

                foreach (KeyValuePair<Guuid, ITranslateProvider> provider in vs)
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
