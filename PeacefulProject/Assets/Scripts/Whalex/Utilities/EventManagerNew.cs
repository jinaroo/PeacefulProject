using System;
using System.Collections.Generic;

public abstract class MyEvent {}

// right now we only have one version of event manager,
// so we don't need to separate service interface and service provider class
// we may also want a logging decorator class to add log optionally
public class EventManagerNew : Singleton<EventManagerNew>
{
    private readonly Dictionary<Type, Action<MyEvent>> registeredHandlers = new Dictionary<Type, Action<MyEvent>>();
    private readonly Dictionary<Delegate, Action<MyEvent>> registeredHandlersLookup = new Dictionary<Delegate, Action<MyEvent>>();
    private readonly List<MyEvent> queuedEvents = new List<MyEvent>();

    // type safe version of Register function
    public void Register<T> (Action<T> handler) where T : MyEvent
    {
        // you can't register same handler twice
        // however, in type unsafe version you can register the same handler to different events
        if (registeredHandlersLookup.ContainsKey(handler))
        {
            return;
        }

        // we store "internalDelegate" into the list, rather than store handler itself in type unsafe version
        // here T is a sub event so it is type safe
        Action<MyEvent> internalDelegate = e => handler((T)e);
        registeredHandlersLookup[handler] = internalDelegate;
        
        Type type = typeof(T);
        if (registeredHandlers.ContainsKey(type))
        {
            registeredHandlers[type] += internalDelegate;
        }
        else
        {
            registeredHandlers[type] = internalDelegate;
        }
    }
    
    // it is not type safe since you could possibly register a EventOne Handler to EventTwo
    public void Register<T>(Action<MyEvent> handler) where T : MyEvent
    {
        Type type = typeof(T);
        if (registeredHandlers.ContainsKey(type))
        {
            registeredHandlers[type] += handler;
        }
        else
        {
            registeredHandlers[type] = handler;
        }
    }

    public void Unregister<T>(Action<T> handler) where T : MyEvent
    {
        Type type = typeof(T);
        Action<MyEvent> internalDelegate;
        if (registeredHandlersLookup.TryGetValue(handler, out internalDelegate)) {
            Action<MyEvent> tempDel;
            if (registeredHandlers.TryGetValue(typeof(T), out tempDel)) {
                tempDel -= internalDelegate;
                if (tempDel == null) {
                    registeredHandlers.Remove(type);
                } else {
                    registeredHandlers[type] = tempDel;
                }
            }
            registeredHandlersLookup.Remove(handler);
        }
    }
    
    public void Unregister<T>(Action<MyEvent> handler) where T : MyEvent
    {
        Type type = typeof(T);
        Action<MyEvent> handlers;
        if (registeredHandlers.TryGetValue(type, out handlers))
        {
            handlers -= handler;
            if (handlers == null)
            {
                registeredHandlers.Remove(type);
            }
            else
            {
                registeredHandlers[type] = handlers;
            }
        }
    }

    public void Fire(MyEvent e)
    {
        Type type = e.GetType();
        Action<MyEvent> handlers;
        if (registeredHandlers.TryGetValue(type, out handlers))
        {
            handlers(e);
        }
    }

    public void QueueEvent(MyEvent e) {
        queuedEvents.Add(e);
    }

    public void ProcessQueuedEvents() {
        for (int i = queuedEvents.Count - 1; i >= 0; --i) {
            Fire(queuedEvents[i]);
            queuedEvents.RemoveAt(i);
        }
    }
}