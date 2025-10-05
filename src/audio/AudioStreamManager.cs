
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

public static class AudioStreamPlayerManager
{
    private static readonly List<AudioStreamPlayer> _allStreams = new();

    public static void Add(AudioStreamPlayer stream)
    {
        if (!_allStreams.Contains(stream))
            _allStreams.Add(stream);
    }

    public static void Remove(AudioStreamPlayer stream)
    {
        _allStreams.Remove(stream);
    }

    public static void Play(AudioStreamPlayer stream)
    {
        stream.Instance?.Play();
    }

    public static void Pause(AudioStreamPlayer stream)
    {
        stream.Instance?.Pause();
    }

    public static void Stop(AudioStreamPlayer stream)
    {
        stream.Instance?.Stop();
    }

    public static void Update()
    {
        foreach (var s in _allStreams)
        {
            s.Update();
        }
        foreach (var stream in _allStreams.Where(s => s.Destroyed).ToList())
        {
            Remove(stream);
        }

    }
}
