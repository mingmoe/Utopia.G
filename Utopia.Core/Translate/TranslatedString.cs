using CommunityToolkit.Diagnostics;
using Jeffijoe.MessageFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// 翻译更新事件
    /// </summary>
    public IEventManager<IEventWithParamAndResult<string, string>> TranslateUpdateEvent { get; }
}

/// <summary>
/// 已经翻译过的字符串
/// </summary>
public class TranslatedString : ITranslatedString
{

    public string Translated { get; private set; }

    public IEventManager<IEventWithParamAndResult<string, string>> TranslateUpdateEvent { get; } =
        (IEventManager<IEventWithParamAndResult<string, string>>)new EventManager<ComplexEvent<string, string>, string, string>();

    public TranslatedString(string translated)
    {
        Guard.IsNotNullOrEmpty(translated);
        this.Translated = translated;
    }

    public void Update(string newMsg)
    {
        Guard.IsNotNull(newMsg);
        var e = new ComplexEvent<string, string>(this.Translated, newMsg, false);
        this.TranslateUpdateEvent.Fire(e);
        this.Translated = e.Result ?? e.Parameter!;
    }
}

public class ICUTranslatedString : ITranslatedString
{

    public string Translated { get; private set; }

    public object Data { get; private set; }

    public IEventManager<IEventWithParamAndResult<string, string>> TranslateUpdateEvent { get; } =
        (IEventManager<IEventWithParamAndResult<string, string>>)new EventManager<ComplexEvent<string, string>>();

    public ICUTranslatedString(string msg, object data)
    {
        Guard.IsNotNull(msg);
        Guard.IsNotNull(data);

        this.Translated = MessageFormatter.Format(msg, data);
        this.Data = data;
    }

    public void Update(string newMsg, object data)
    {
        Guard.IsNotNull(newMsg);
        Guard.IsNotNull(data);

        newMsg = MessageFormatter.Format(newMsg, data);

        var e = new ComplexEvent<string, string>(this.Translated, newMsg, false);
        this.TranslateUpdateEvent.Fire(e);
        this.Translated = e.Result ?? e.Parameter!;

        this.Translated = newMsg;
        this.Data = data;
    }
}
