using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class TimerHandle
{
    internal bool IsDone = false;
    public bool Done => IsDone;
}

public static class TimerManager
{
    private class Timer
    {
        public float TimeLeft;
        public Action Callback;
        public TimerHandle Handle;
    }

    private static List<Timer> _timers = new();

    public static TimerHandle Wait(float seconds, Action callback = null)
    {
        var handle = new TimerHandle();
        _timers.Add(new Timer
        {
            TimeLeft = seconds,
            Callback = callback,
            Handle = handle
        });
        return handle;
    }

    public static void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        for (int i = _timers.Count - 1; i >= 0; i--)
        {
            _timers[i].TimeLeft -= dt;
            if (_timers[i].TimeLeft <= 0)
            {
                _timers[i].Callback?.Invoke();
                _timers[i].Handle.IsDone = true;
                _timers.RemoveAt(i);
            }
        }
    }
}
