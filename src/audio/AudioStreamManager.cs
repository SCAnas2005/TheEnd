
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public static class AudioStreamPlayerManager
{
    private static readonly List<AudioStreamPlayer> _allStreams = new();

    public static void Add(AudioStreamPlayer stream)
    {
        if (!_allStreams.Contains(stream))
            _allStreams.Add(stream);
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
            if (s is AudioStreamPlayer2D s2d)
            {
                s2d.UpdateVolume();
            }
        }
    }
}
