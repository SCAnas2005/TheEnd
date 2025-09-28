using System;
using Microsoft.Xna.Framework;

public static class CameraCinematicController
{
    private static Vector2 _start;
    private static Vector2 _target;
    private static float _duration;
    private static float _elapsed;
    private static bool _isRunning;
    private static Action _onComplete;
    private static Map _map;

    public static bool IsRunning => _isRunning;

    public static void Start(Vector2 from, Vector2 to, float durationSeconds, Map map, Action onComplete = null)
    {
        _start = from;
        _target = to;
        _duration = durationSeconds;
        _elapsed = 0f;
        _isRunning = true;
        _onComplete = onComplete;
        _map = map;

        Camera2D.FocusOnPlayer = false;
    }

    public static void Update(GameTime gameTime)
    {
        if (!_isRunning) return;

        _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        float t = Math.Min(_elapsed / _duration, 1f);
        Camera2D.Position = Vector2.Lerp(_start, _target, SmoothStep(t));
        Camera2D.ClampToMapBounds(_map);

        if (_elapsed >= _duration)
        {
            _isRunning = false;
            _onComplete?.Invoke();
        }
    }

    private static float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t); // interpolation plus douce qu’un simple Lerp linéaire
    }
}
