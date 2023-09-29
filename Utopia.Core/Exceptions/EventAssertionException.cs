using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Events;

namespace Utopia.Core.Exceptions;

public enum EventAssertionFailedCode
{
    /// <summary>
    /// 事件的结果不能为null，结果却发生了
    /// </summary>
    ResultIsNull,
    /// <summary>
    /// 事件不能被取消，却发生了
    /// </summary>
    EventCancled
}

/// <summary>
/// 事件的某项条件。如返回值不能为null，事件不能被取消，引发。
/// </summary>
public class EventAssertionException : System.Exception
{
    public EventAssertionFailedCode ErrorCode { get; }

    public EventAssertionException(EventAssertionFailedCode error)
    {
        this.ErrorCode = error;
    }

    public override string ToString()
    {
        return "Error Code:" + this.ErrorCode.ToString() + "\n" + base.ToString();
    }

    public static T ThrowIfResultIsNull<T>(IEventWithResult<T> e)
    {
        if (e.Result is null)
        {
            throw new EventAssertionException(EventAssertionFailedCode.ResultIsNull);
        }
        return e.Result;
    }
}
