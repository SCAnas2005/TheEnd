

using System;
using Microsoft.Xna.Framework.Audio;

public static class AudioManager
{
    private static float _volume;
    private static bool _repeat;

    public static float Volume {get {return _volume; } set {_volume = value;}}
    
    public static void PrintInfo()
    {
        Console.WriteLine("===== AUDIO MANAGER ======");
        Console.WriteLine($"Volume: {_volume * 100}%, Repeat: {_repeat}");
        string str = "";
        Console.WriteLine($"Available sounds: {str}");
        Console.WriteLine("==========================");

    }

    public static void Init(float volume=1f, bool repeat=false)
    {
        _volume = volume;
        _repeat = repeat;
    }


    public static void Play(SoundEffect sound) {
        Play(sound, _volume);
    }
    public static void Play(SoundEffect sound, float volume)
    {
        if (sound == null) return;
        if (volume>1f) volume = 1f;
        if (volume < 0f) volume = 0f;
        // Console.WriteLine($"Playing sound: {sound}");
        sound.Play(volume:volume, pitch:0f ,pan:0f);
    }
}