// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using CommunityToolkit.Diagnostics;
using Jeffijoe.MessageFormat;
using Utopia.Core.Events;
using Utopia.Core.Exceptions;
using Utopia.Core.Utilities;

namespace Utopia.Core.Transition;

/// <summary>
/// A formatted,translated and thread-safe string for human.
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
            lock (_lock)
            {
                return _value;
            }
        }
        set
        {
            lock (_lock)
            {
                _value = value;
            }
        }
    }

    public IEventManager<IEventWithParamAndResult<string, string>> TranslateUpdateEvent
    { get; } = new EventManager<IEventWithParamAndResult<string, string>>();

    public TranslatedString(string translated)
    {
        ArgumentNullException.ThrowIfNull(translated);
        _value = translated;
    }
}

/// <summary>
/// 已翻译的ICU format字符串
/// </summary>
public struct ICUTranslatedString : ITranslatedString
{
    private readonly object _lock = new();
    private bool _reformatNeed;
    private string _translated;

    private void _ManagerUpdateHandle(IEventWithParam<ITranslateManager> manager)
    {
        lock (_lock)
        {
            _reformatNeed = true;
        }
    }

    public string Translated
    {
        get
        {
            lock (_lock)
            {
                if (_reformatNeed)
                {
                    _UpdateTranslation(null);
                    _reformatNeed = false;
                }
                return _translated;
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
            lock (_lock)
            {
                return _key;
            }
        }
        set
        {
            lock (_lock)
            {
                ArgumentNullException.ThrowIfNull(value);
                _key = value;
                _reformatNeed = true;
            }
        }
    }

    private ITranslateManager _manager;

    public ITranslateManager Manager
    {
        get
        {
            lock (_lock)
            {
                return _manager;
            }
        }
        set
        {
            lock (_lock)
            {
                ArgumentNullException.ThrowIfNull(value);
                _manager.TranslateUpdatedEvent.Unregister(
                        _ManagerUpdateHandle
                    );
                _manager = value;
                _reformatNeed = true;
                _manager.TranslateUpdatedEvent.Register(
                    _ManagerUpdateHandle
                    );
            }
        }
    }

    private TranslateIdentifence _identifence;

    public TranslateIdentifence Identifence
    {
        get
        {
            lock (_lock)
            {
                return _identifence;
            }
        }
        set
        {
            lock (_lock)
            {
                ArgumentNullException.ThrowIfNull(value);
                _identifence = value;
                _reformatNeed = true;
            }
        }
    }

    public IEventManager<IEventWithParamAndResult<string, string>> TranslateUpdateEvent { get; } =
        new EventManager<IEventWithParamAndResult<string, string>>();

    private object _data;

    /// <summary>
    /// force 更新翻译.
    /// </summary>
    /// <param name="newData">
    /// 如果提供了新的数据对象(非null传入),那么就替换之前的数据对象.
    /// 否则沿用之前的数据对象.请确保newData是线程安全的对象.
    /// </param>
    public void UpdateTranslate(object? newData = null)
    {
        lock (_lock)
        {
            _UpdateTranslation(newData);
        }
    }

    private void _UpdateTranslation(object? newData)
    {
        if (newData != null)
        {
            _data = newData;
        }

        if (!_manager.TryGetTranslate(
            _identifence,
            Key.TranslateProviderId == null ? null : Guuid.Parse(Key.TranslateProviderId),
            Guuid.Parse(Key.TranslateItemId),
            out string? got))
        {
            got = _key.TranslateItemId;
        }

        string @new =
            MessageFormatter.Format(got!, _data);

        _FireReformatEvent(@new);
    }

    private void _FireReformatEvent(string @new)
    {
        ComplexEvent<string, string> @event = new(_translated, @new);
        TranslateUpdateEvent.Fire(@event);
        _translated = EventAssertionException.ThrowIfResultIsNull(@event);
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

        _key = msg;
        _manager = manager;
        _identifence = identifence;
        _translated = null!;
        _data = data;
        _reformatNeed = false;

        UpdateTranslate(null);
    }
}
