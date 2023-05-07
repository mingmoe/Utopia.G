//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System.Security.Cryptography;

namespace Utopia.Core.Translate;

/// <summary>
/// 默认翻译管理器，非线程安全。
/// </summary>
public class TranslateManager : SafeDictionary<Guuid, ITranslateProvider>, ITranslateManager
{

    public long TranslateID { get; private set; }

    public TranslateManager()
    {
        // 使用加密安全随机数填充初始翻译ID，只填充一个int
        this.TranslateID =
            RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
    }

    public string Activate(TranslateKey key, TranslateIdentifence id)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(id);

        if (key.Id?.Cached != null && key.Id.Id == this.TranslateID)
        {
            return key.Id.Cached;
        }

        else if (this.TryGetTranslate(id, key.TranslateProviderId == null ? null : Guuid.ParseString(key.TranslateProviderId),
            Guuid.ParseString(key.TranslateItemId), out string? result))
        {
            key.Id = new(result, this.TranslateID);
            return result!;
        }
        else
        {
            key.Id = new(string.Format("{0}::{1}", key.TranslateProviderId?.ToString() ?? "any provider",
                key.TranslateItemId.ToString()), this.TranslateID);
        }

        return key.Id.Cached!;
    }

    public void UpdateCache()
    {
        this.TranslateID++;
    }

    public bool Contains(TranslateIdentifence language, Guuid? translateProviderId, Guuid translateItemId)
    {
        return this.TryGetTranslate(language, translateProviderId, translateItemId, out _);
    }

    public bool TryGetTranslate(TranslateIdentifence language, Guuid? translateProviderId, Guuid translateItemId, out string? result)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(translateItemId);

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
