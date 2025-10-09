
using System;
using Microsoft.Xna.Framework;

public class Event
{
    public bool Started { get; set; }
    public bool Ended { get; set; }
    public Func<bool> ShouldStart;
    public Func<bool> ShouldEnd;
    public Action End;
    public Action Start;
    public Action<GameTime> Updating;
    public bool Finished { get; set; }

    public Event(Func<bool> shouldStart, Action start, Func<bool> shouldEnd, Action end, Action<GameTime> update)
    {
        Started = false;
        Ended = false;
        Finished = false;
        ShouldStart = shouldStart;
        Start = start;
        ShouldEnd = shouldEnd;
        End = end;
        Updating = update;
    }

    public void Update(GameTime gameTime)
    {
        Updating(gameTime);
    }
}