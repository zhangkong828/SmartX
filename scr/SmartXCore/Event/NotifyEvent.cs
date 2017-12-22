using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SmartXCore.Event
{
    public class NotifyEvent : EventArgs
    {
        private static readonly ConcurrentDictionary<NotifyEventType, NotifyEvent> EmptyEvents
            = new ConcurrentDictionary<NotifyEventType, NotifyEvent>();

        public NotifyEventType Type { get; }
        public object Target { get; }

        private NotifyEvent(NotifyEventType type, object target = null)
        {
            Type = type;
            Target = target;
        }

        public static NotifyEvent CreateEvent(NotifyEventType type, object target = null)
        {
            return target == null ? EmptyEvents.GetOrAdd(type, key => new NotifyEvent(key)) : new NotifyEvent(type, target);
        }
    }
}
