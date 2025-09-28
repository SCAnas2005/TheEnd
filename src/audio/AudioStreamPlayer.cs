
using System;
using Microsoft.Xna.Framework.Audio;

public class AudioStreamPlayer
{
    protected SoundEffectInstance _instance;
    public SoundEffectInstance Instance { get => _instance; set { _instance = value; } }

    public string Name { get; }
    public bool IsPlaying => _instance != null ? _instance?.State == SoundState.Playing : false;
    public float Volume { get => _instance != null ? _instance.Volume : 0f; set { if (_instance != null) _instance.Volume = Math.Clamp(value, 0f, 1f); } }
    public bool Loop { get => _instance != null ? _instance.IsLooped : false; set { if (Instance != null) _instance.IsLooped = value; } }


    public AudioStreamPlayer()
    {
        AudioStreamPlayerManager.Add(this);
    }

    public AudioStreamPlayer(string name, SoundEffectInstance soundEffect = null)
    {
        Name = name;
        Instance = soundEffect;
        AudioStreamPlayerManager.Add(this);
    }

    public AudioStreamPlayer(string name, string path)
    {
        Name = name;
        FromSource(path);
        AudioStreamPlayerManager.Add(this);
    }

    public void FromSource(string path)
    {
        var s = Globals.Content.Load<SoundEffect>(path);
        Instance = s.CreateInstance();
    }


    public virtual void Play()
    {
        AudioStreamPlayerManager.Play(this);
    }

    public virtual void Pause()
    {
        AudioStreamPlayerManager.Pause(this);
    }

    public virtual void Stop()
    {
        AudioStreamPlayerManager.Stop(this);
    }
}