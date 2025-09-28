
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

public class AudioStreamPlayer2D : AudioStreamPlayer
{
    public Entity Entity;
    private Vector2 Position;
    public float MinDistance { get; set; } = 50f;
    public float MaxDistance { get; set; } = 500f;

    public Player Player;

    public AudioStreamPlayer2D() : base()
    {
        Entity = null;
        SetListener(EntityManager.Player);
    }

    public AudioStreamPlayer2D(string name, SoundEffectInstance sound, Entity entity) : base(name, sound)
    {
        Entity = entity;
        Position = entity.Position;
    }

    public AudioStreamPlayer2D(string name, string path, Entity entity) : base(name, path)
    {
        Entity = entity;
        Position = entity.Position;
    }

    public void SetListener(Player player)
    {
        Player = player;
    }


    public void Update()
    {
        Position = Entity.Position;
        UpdateVolume();
    }

    public void UpdateVolume()
    {
        if (Instance == null || Player == null ) return;

        float dist = Vector2.Distance(Player.Position, Position);
        if (dist <= MinDistance) Volume = 1f;
        else if (dist >= MaxDistance) Volume = 0f;
        else
        {
            float t = (dist - MinDistance) / (MaxDistance - MinDistance);
            Volume = float.Lerp(1f, 0f, t); // atténuation linéaire }
        }
    }

}