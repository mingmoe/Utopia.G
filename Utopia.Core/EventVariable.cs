//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core
{
    /// <summary>
    /// 事件变量，对于这种变量，每次修改都会抛出一个事件。线程安全。
    /// </summary>
    /// <typeparam name="T">变量类型。</typeparam>
    public class EventVariable<T> : IEventVariable<T>
    {
        private T value;
        private readonly bool cancelAble;
        private readonly object locker = new();

        public EventVariable(T value, bool cancelAble = false)
        {
            this.value = value;
            this.cancelAble = cancelAble;
        }

        public IEventManager<IEvent> ModifyEvent { get; } = new EventManager<IEvent>();

        public T Value
        {
            get
            {
                lock (locker)
                    return value;
            }
            set
            {
                lock (locker)
                {
                    var e = new Event(value, null, this.cancelAble);
                    ModifyEvent.Fire(e);

                    if (e.Result != null)
                    {
                        this.value = (T)e.Result;
                    }
                    else
                    {
                        this.value = value;
                    }
                }
            }
        }
    }
}
