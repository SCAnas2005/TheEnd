using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class Camera2D
{
    public static Vector2 Position = Vector2.Zero;
    public static float Scale = 1f;
    public static float Zoom = 1f;
    public static float Rotation = 0f;
    public static bool StopX = false;
    public static bool StopY = false;
    public static bool FocusOnPlayer = false;
    public static float? MapZoom = null;

    public static Size CameraLogicalSize;

    private static Viewport _viewport;


    private static float _shakeDuration = 0f;
    private static float _shakeTimer = 0f;
    private static float _shakeIntensity = 0f;
    private static Vector2 _shakeOffset = Vector2.Zero;
    private static Random _rand = new Random();
    private static bool IsShaking = false;


    public static void Init(Viewport viewport, Map map)
    {
        _viewport = viewport;
        // CameraLogicalSize = new Size((int)(1080*1.4), (int)(720*1.3));
        CameraLogicalSize = new Size(Globals.ScreenSize.Width, Globals.ScreenSize.Height);

        float zoomX = map.Width < CameraLogicalSize.Width ?
            (float)CameraLogicalSize.Width / (float)map.Width : 1f;

        float zoomY = map.Height < CameraLogicalSize.Height ?
            (float)CameraLogicalSize.Height / (float)map.Height : 1f;

        Scale = Math.Max(zoomX, zoomY);
        SetZoom(map.Zoom, map);


        FocusOnPlayer = true;
    }

    public static void SetZoom(float zoom, Map map)
    {
        float effectiveZoom = zoom * Scale;
        float viewWidth = CameraLogicalSize.Width / effectiveZoom;
        float viewHeight = CameraLogicalSize.Height / effectiveZoom;


        if (viewWidth > map.Width || viewHeight > map.Height)
        {
            Zoom = 1f;
        }
        else
        {
            Zoom = zoom;
            if (Zoom > 4.5f)
            {
                Zoom = 4.5f;

            }
        }
        ClampToMapBounds(map);
    }

    public static void ClampToMapBounds(Map map)
    {
        float effectiveZoom = Zoom * Scale;
        int mapWidth = map.Width;
        int cameraWidth = (int)(CameraLogicalSize.Width / effectiveZoom);
        if (Position.X < 0)
        {
            Position.X = 0;
            StopX = true;
        }
        else if (Position.X + cameraWidth > mapWidth)
        {
            Position.X = mapWidth - cameraWidth;
            StopX = true;
        }
        else
        {
            StopX = false;
        }

        int mapHeight = map.Height;
        int cameraHeight = (int)(CameraLogicalSize.Height / effectiveZoom);

        if (mapHeight <= cameraHeight)
        {
            Position.Y /= 2;
            StopY = true;
        }
        else if (Position.Y < 0)
        {
            Position.Y = 0;
            StopY = true;
        }
        else if (Position.Y > mapHeight - cameraHeight)
        {
            Position.Y = mapHeight - cameraHeight;
            StopY = true;
        }
        else
        {
            StopY = false;
        }
    }

    public static void LookAtPosition(Vector2 target)
    {
        // Position = target;
        float effectiveZoom = Zoom * Scale;
        Position = new Vector2(!StopX ? target.X - CameraLogicalSize.Width / effectiveZoom / 2 : Position.X, !StopY ? target.Y - CameraLogicalSize.Height / effectiveZoom / 2 : Position.Y);
    }

    public static void LookAtPlayer(Vector2 target, Map map)
    {
        LookAtPosition(target);
        ClampToMapBounds(map);
    }

    public static void Update(GameTime gameTime)
    {
        if (IsShaking) UpdateShake(gameTime);
    }

    public static void UpdateShake(GameTime gameTime)
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            float progress = _shakeTimer / _shakeDuration;
            float currentIntensity = _shakeIntensity * progress;

            float offsetX = ((float)_rand.NextDouble() * 2 - 1) * currentIntensity;
            float offsetY = ((float)_rand.NextDouble() * 2 - 1) * currentIntensity;

            _shakeOffset = new Vector2(offsetX, offsetY);
        }
        else
        {
            _shakeOffset = Vector2.Zero;
            IsShaking = false;
        }
    }


    public static void Move(Vector2 delta, Map map)
    {
        Position += delta;
        ClampToMapBounds(map);
    }

    public static Matrix GetViewMatrix()
    {
        return
            Matrix.CreateTranslation(-new Vector3(Position + _shakeOffset, 0f)) *
            Matrix.CreateScale(Zoom * Scale) *
            Matrix.CreateRotationZ(Rotation);
        // Matrix.CreateTranslation(screenCenter);
    }


    public static void Shake(float durationSeconds, float intensity)
    {
        IsShaking = true;
        _shakeDuration = durationSeconds;
        _shakeTimer = durationSeconds;
        _shakeIntensity = intensity;
    }


    public static Vector2 ScreenToMap(Vector2 screenPosition)
    {
        return (screenPosition / (Zoom * Scale)) + Position;
    }


}
