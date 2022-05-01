using System;
using System.Collections.Generic;
using System.Text;


namespace AridArnold
{
    delegate void EventCallback(EArgs args);

    struct EArgs
    {
        public object sender;
    }

    enum EventType
    {
        PlayerDead,
    }

    internal class EventManager : Singleton<EventManager>
    {
        
        Dictionary<EventType, EventCallback> mEventListeners = new Dictionary<EventType, EventCallback>();

        public void AddListener(EventType type, EventCallback callback)
        {
            if (mEventListeners.ContainsKey(type))
            {
                mEventListeners[type] += callback;
            }
            else
            {
                mEventListeners.Add(type, callback);
            }
        }

        public void SendEvent(EventType type, EArgs args)
        {
            if(mEventListeners.ContainsKey(type))
            {
                mEventListeners[type].Invoke(args);
            }
        }
    }
}
