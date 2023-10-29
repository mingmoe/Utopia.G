// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

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

    public EventAssertionException(EventAssertionFailedCode error) => ErrorCode = error;

    public override string ToString() => "Error Code:" + ErrorCode.ToString() + "\n" + base.ToString();

    public static T ThrowIfResultIsNull<T>(IEventWithResult<T> e) => e.Result is null ? throw new EventAssertionException(EventAssertionFailedCode.ResultIsNull) : e.Result;
}
