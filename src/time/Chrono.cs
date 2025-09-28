
using System;

public class Chrono
{
    private double _seconds;
    private bool _started;
    private bool _paused;

    private double? _timerWaited;

    public Chrono(double seconds=0)
    {
        _seconds = seconds;
        _started = false;
        _paused = false;

        _timerWaited=null;
    }

    public void Update(double s) {
        _seconds += s;
    }

    public void PrintTime()
    {
        Console.WriteLine($"[TIMER] Actual time: {_seconds}");
    }

    public double GetSeconds() { return _seconds; }
    public int GetSecondsInt() { return (int)_seconds; }

    public bool Wait(double seconds)
    {
        if (_timerWaited == null)
        {
            _timerWaited = GetSeconds();
        }

        double? dt = GetSeconds()-_timerWaited;
        if (dt >= seconds)
        {
            _timerWaited = null;
            return true;
        }
        return false;
    }

    public void Pause() { _paused = true; }
    public void Resume() { _paused = false; }
    public void Stop() { _started = false; }
    public void Start() { _started = true; }
}