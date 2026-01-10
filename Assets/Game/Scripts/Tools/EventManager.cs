using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EventManager : Singleton<EventManager>, IDisposable
{
    private Dictionary<Events, Action<object>> listeners = new Dictionary<Events, Action<object>>();

    public void AddListener(Events name, Action<object> listener)
    {
        if (listeners.TryGetValue(name, out var existingListeners))
        {
            listeners[name] = existingListeners + listener;
        }
        else
        {
            listeners.Add(name, listener);
        }
    }

    public void RemoveListener(Events name, Action<object> listener)
    {
        try
        {
            if (listeners.TryGetValue(name, out var existingListeners))
            {
                listeners[name] = existingListeners - listener;
                if (listeners[name] == null) // 如果移除后没有剩余的委托，则删除该键
                {
                    listeners.Remove(name);
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void SendMessage(Events name, object mess = null)
    {
        if (listeners.TryGetValue(name, out var eventListeners) && eventListeners != null)
        {
            eventListeners.Invoke(mess);
        }
    }

    public void ClearListener(Events name)
    {
        listeners.Remove(name);
    }

    public int GetAllListener(Events name)
    {
        if (listeners.TryGetValue(name, out var eventListeners) && eventListeners != null)
        {
            return eventListeners.GetInvocationList().Length;
        }
        return 0;
    }
    public void GetAllListener( )
    {
        listeners?.Clear();
    }
    ~EventManager()
    {
        Dispose();
    }

    public void Dispose()
    {
        listeners?.Clear();
        GC.SuppressFinalize(this);
    }
}
