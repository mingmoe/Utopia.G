#region copyright
// This file(may named TranslatedString.cs) is a part of the project: Utopia.Core.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Core.
//
// Utopia.Core is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Core is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Core. If not, see <https://www.gnu.org/licenses/>.
#endregion

using CommunityToolkit.Diagnostics;
using Jeffijoe.MessageFormat;
using Utopia.Core.Events;
using Utopia.Core.Exceptions;
using Utopia.Core.Utilities;

namespace Utopia.Core.Translate;

/// <summary>
/// 通常应该是线程安全的
/// </summary>
public interface ITranslatedString
{
    /// <summary>
    /// 已经翻译的字符串
    /// </summary>
    public string Translated { get; }

    /// <summary>
    /// 翻译更新事件。事件的参数是旧翻译，事件的结果是新翻译。
    /// 最后的实际翻译为事件调用链结束后事件的结果值,不可为null.
    /// 可以被取消。
    /// </summary>
    public IEventManager<IEventWithParamAndResult<string, string>> TranslateUpdateEvent { get; }
}

public class TranslatedString : ITranslatedString
{
    private readonly object _lock = new();

    private string _value;

    public string Translated
    {
        get
        {
            lock (this._lock)
            {
                return this._value;
            }
        }
        set
        {
            lock (this._lock)
            {
                this._value = value;
            }
        }
    }

    public IEventManager<IEventWithParamAndResult<string, string>> TranslateUpdateEvent
    { get; } = new EventManager<IEventWithParamAndResult<string, string>>();

    public TranslatedString(string translated)
    {
        ArgumentNullException.ThrowIfNull(translated);
        this._value = translated;
    }
}

/// <summary>
/// 已翻译的ICU字符串
/// </summary>
public class ICUTranslatedString : ITranslatedString
{
    private readonly object _lock = new();
    private string _cached = null!;

    private string _translated;

    private void _ManagerUpdateHandle(IEventWithParam<ITranslateManager> manager)
    {
        this.UpdateTranslate(null);
    }

    public string Translated
    {
        get
        {
            lock (this._lock)
            {
                return this._translated;
            }
        }
        set
        {
            lock (this._lock)
            {
                this._translated = value;
            }
        }
    }

    private TranslateKey _key;

    /// <summary>
    /// 改变要从管理器获取翻译的Key.
    /// 但是不会刷新翻译,需要手动调用<see cref="UpdateTranslate(object?)"/>
    /// </summary>
    public TranslateKey Key
    {
        get
        {
            lock (this._lock)
            {
                return this._key;
            }
        }
        set
        {
            lock (this._lock)
            {
                ArgumentNullException.ThrowIfNull(value);
                this._key = value;
            }
        }
    }

    private ITranslateManager _manager;

    /// <summary>
    /// 改变获取翻译的管理器.
    /// 但是不会刷新翻译,需要手动调用<see cref="UpdateTranslate(object?)"/>
    /// </summary>
    public ITranslateManager Manager
    {
        get
        {
            lock (this._lock)
            {
                return this._manager;
            }
        }
        set
        {
            lock (this._lock)
            {
                ArgumentNullException.ThrowIfNull(value);
                this._manager.TranslateUpdatedEvent.Unregister(
                        this._ManagerUpdateHandle
                    );
                this._manager = value;
                this._manager.TranslateUpdatedEvent.Register(
                    this._ManagerUpdateHandle
                    );
            }
        }
    }

    private TranslateIdentifence _identifence;

    /// <summary>
    /// 改变翻译的标识符.
    /// 但是不会刷新翻译,需要手动调用<see cref="UpdateTranslate(object?)"/>
    /// </summary>
    public TranslateIdentifence Identifence
    {
        get
        {
            lock (this._lock)
            {
                return this._identifence;
            }
        }
        set
        {
            lock (this._lock)
            {
                ArgumentNullException.ThrowIfNull(value);
                this._identifence = value;
            }
        }
    }

    public IEventManager<IEventWithParamAndResult<string, string>> TranslateUpdateEvent { get; } =
        new EventManager<IEventWithParamAndResult<string, string>>();

    private object _data;

    /// <summary>
    /// 更新翻译.
    /// </summary>
    /// <param name="newData">
    /// 如果提供了新的数据对象(非null传入),那么就替换之前的数据对象.
    /// 否则沿用之前的数据对象.请确保newData是线程安全的对象.
    /// </param>
    public void UpdateTranslate(object? newData = null)
    {
        lock (this._lock)
        {
            if (newData != null)
            {
                this._data = newData;
            }

            if (!this._manager.TryGetTranslate(
                this._identifence,
                this.Key.TranslateProviderId == null ? null : Guuid.ParseString(this.Key.TranslateProviderId),
                Guuid.ParseString(this.Key.TranslateItemId),
                out string? got))
            {
                got = this._key.TranslateItemId;
            }

            var @new =
                MessageFormatter.Format(got!, this._data);

            this._cached = @new;

            this._FireReformatEvent();
        }
    }

    private void _FireReformatEvent()
    {
        ComplexEvent<string, string> @event = new(this._translated, this._cached);
        this.TranslateUpdateEvent.Fire(@event);
        this._translated = EventAssertionException.ThrowIfResultIsNull(@event);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data">
    /// 注意! 这个参数将会一直被本对象持有.
    /// 直到下一次以非null参数调用<see cref="UpdateTranslate(object?)"/>,
    /// 那么则会使用参数的new data来替换此data.
    /// 请确保此参数是线程安全的.
    /// </param>
    public ICUTranslatedString(TranslateKey msg, ITranslateManager manager,
        TranslateIdentifence identifence, object data)
    {
        Guard.IsNotNull(msg);
        Guard.IsNotNull(data);
        Guard.IsNotNull(manager);
        Guard.IsNotNull(identifence);

        this._key = msg;
        this._manager = manager;
        this._identifence = identifence;
        this._translated = null!;
        this._data = data;

        this.UpdateTranslate(null);
    }
}
