using CommunityToolkit.Diagnostics;
using Jeffijoe.MessageFormat;
using Utopia.Core.Exception;

namespace Utopia.Core.Translate;

/// <summary>
/// 通常不是线程安全的
/// </summary>
public interface ITranslatedString
{
    /// <summary>
    /// 已经翻译的字符串
    /// </summary>
    public string Translated { get; }

    /// <summary>
    /// 翻译更新事件。事件的参数是旧翻译，事件的结果是新翻译。
    /// 最后的实际翻译为事件调用链结束后事件的结果值。
    /// 可以被取消。
    /// </summary>
    public IEventManager<IEventWithParamAndResult<string, string>> TranslateUpdateEvent { get; }
}

public class TranslatedString : ITranslatedString
{
    public string Translated { get; set; } = string.Empty;

    public IEventManager<IEventWithParamAndResult<string, string>> TranslateUpdateEvent
    { get; } = new EventManager<IEventWithParamAndResult<string, string>>();
}

/// <summary>
/// 已翻译的ICU字符串
/// </summary>
public class ICUTranslatedString : ITranslatedString
{
    private string _cached = null!;

    public string Translated { get; private set; }

    private object _data;

    /// <summary>
    /// 改变这个只会重新格式化，使用上次获取到的缓存的翻译。
    /// 不会从<see cref="Manager"/>重新获取翻译
    /// </summary>
    public object Data
    {
        get
        {
            return this._data;
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            this._data = value;
            this._Reformat();
        }
    }

    private TranslateKey _key;

    /// <summary>
    /// 改变这个将会重新获取翻译（从<see cref="Manager"/>中），重新格式化。
    /// </summary>
    public TranslateKey Key
    {
        get
        {
            return this._key;
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            this._key = value;
            this.UpdateTranslate();
        }
    }

    private ITranslateManager _manager;

    /// <summary>
    /// 改变这个将会重新获取翻译（从<see cref="Manager"/>中），重新格式化。
    /// </summary>
    public ITranslateManager Manager
    {
        get { return this._manager; }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            this._manager = value;
            this.UpdateTranslate();
        }
    }

    private TranslateIdentifence _identifence;

    /// <summary>
    /// 改变这个将会重新获取翻译（从<see cref="Manager"/>中），重新格式化。
    /// </summary>
    public TranslateIdentifence Identifence
    {
        get { return this._identifence; }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            this._identifence = value;
            this.UpdateTranslate();
        }
    }

    public IEventManager<IEventWithParamAndResult<string, string>> TranslateUpdateEvent { get; } =
        (IEventManager<IEventWithParamAndResult<string, string>>)new EventManager<ComplexEvent<string, string>>();

    public void UpdateTranslate()
    {
        var @new =
            MessageFormatter.Format(this.Manager.Activate(this.Key, this.Identifence),
            this.Data);

        this._cached = @new;

        this._Reformat();
    }

    private void _Reformat()
    {
        ComplexEvent<string, string> @event = new(this.Translated, this._cached, true);
        this.TranslateUpdateEvent.Fire(@event);
        this.Translated = @event.Result ??
            throw new EventAssertionException(EventAssertionFailedCode.ResultIsNull);
    }

    public ICUTranslatedString(TranslateKey msg, ITranslateManager manager,
        TranslateIdentifence identifence, object data)
    {
        Guard.IsNotNull(msg);
        Guard.IsNotNull(data);
        Guard.IsNotNull(manager);
        Guard.IsNotNull(identifence);

        this.Key = msg;
        this.Manager = manager;
        this.Identifence = identifence;
        this.Data = data;

        this.UpdateTranslate();
    }
}
