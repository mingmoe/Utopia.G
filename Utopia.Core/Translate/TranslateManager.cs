//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core.Translate;

/// <summary>
/// 默认翻译管理器，线程安全。
/// </summary>
public class TranslateManager : ITranslateManager
{
    private readonly ConcurrentDictionary<Guuid, ITranslateProvider> _translate = new();

    public bool ContainsTranslateItem(TranslateIdentifence language, Guuid? translateProviderId, Guuid translateItemId)
    {
        return this.TryGetTranslate(language, translateProviderId, translateItemId, out _);
    }

    public bool ContainsTranslateProvider(Guuid id)
    {
        ArgumentNullException.ThrowIfNull(id);
        return this._translate.ContainsKey(id);
    }

    public IReadOnlyCollection<KeyValuePair<Guuid, ITranslateProvider>> GetTranslateProviders()
    {
        return this._translate.ToArray();
    }

    public void RemoveTranslateProvider(Guuid id)
    {
        ArgumentNullException.ThrowIfNull(id);
        this._translate.Remove(id, out _);
    }

    public bool TryGetTranslate(TranslateIdentifence language, Guuid? translateProviderId, Guuid translateItemId, out string? result)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(translateItemId);

        if (translateProviderId is not null &&
            this._translate.TryGetValue(translateProviderId, out ITranslateProvider? value))
        {
            if (value.TryGetItem(language, translateItemId, out result))
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
            var vs = this._translate.Values;

            foreach (var provider in vs)
            {
                if (provider.TryGetItem(language, translateItemId, out result))
                {
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    public bool TryGetTranslateProvider(Guuid id, out ITranslateProvider? result)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (this._translate.TryGetValue(id, out result))
        {
            return true;
        }

        result = null;
        return false;
    }

    public bool TryRegisterTranslateProvider(Guuid id, ITranslateProvider provider)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(provider);

        return this._translate.TryAdd(id, provider);
    }
}
