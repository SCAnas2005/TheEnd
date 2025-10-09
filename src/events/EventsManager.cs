

using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class EventsManager
{
    public static EventsManager Instance;
    private List<Event> _events;

    public EventsManager()
    {
        _events = [];
    }


    public void Add(Event e)
    {
        _events.Add(e);
    }

    public void Remove(Event e)
    {
        _events.Remove(e);
    }

    public void Clear()
    {
        _events.Clear();
    }

    public void Update(GameTime gameTime)
    {
        List<Event> toRemove = [];
        foreach (var e in _events)
        {
            if (e == null) continue;
            if (!e.Started)
            {
                e.Started = e.ShouldStart();
                if (e.Started)
                {
                    e.Start();
                }
            }
            else
            {
                e.Update(gameTime);

                e.Ended = e.ShouldEnd();
                if (e.Ended)
                {
                    e.End();
                    toRemove.Add(e);
                }
            }
        }

        foreach (var e in toRemove)
        {
            _events.Remove(e);
        }
    }
}