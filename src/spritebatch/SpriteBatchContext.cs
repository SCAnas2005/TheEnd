
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

public static class SpriteBatchContext
{
    public static Stack<SpriteBatchState> states = new();
    public static SpriteBatchState Top
    {
        get { return states.Count > 0 ? states.Peek() : null; }
    }


    public static void Push(SpriteBatchState state)
    {
        states.Push(state);
    }

    public static SpriteBatchState Pop()
    {
        return states.Pop();
    }


    public static void ApplyToContext(SpriteBatch _spritebatch, SpriteBatchState _state)
    {
        _spritebatch.Begin(
            _state.SortMode,
            _state.BlendState,
            _state.SamplerState,
            _state.DepthStencilState,
            _state.RasterizerState,
            _state.Effect,
            _state.TransformMatrix
        );
    }

    public static void Restart(SpriteBatch _spriteBatch)
    {
        _spriteBatch.End();
        ApplyToContext(_spriteBatch, Top);
    }
}