using System.Collections.Generic;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{
    private Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();
/*    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                if (!eventManager)
                {
                    Debug.LogError("can't find event manager!");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }*/

    public void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction listener)
    {
        if (Instance == null) return;
        
        UnityEvent thisEvent;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}