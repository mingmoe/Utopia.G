#region copyright
// This file(may named EventAssertionException.cs) is a part of the project: Utopia.Core.
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
