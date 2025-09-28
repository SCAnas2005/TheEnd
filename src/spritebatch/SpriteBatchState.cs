using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class SpriteBatchState
{
    public SpriteSortMode SortMode { get; set; }
    public BlendState BlendState { get; set; }
    public SamplerState SamplerState { get; set; }
    public DepthStencilState DepthStencilState { get; set; }
    public RasterizerState RasterizerState { get; set; }
    public Effect Effect { get; set; }
    public Matrix TransformMatrix { get; set; }

    public SpriteBatchState(
        SpriteSortMode sortMode = SpriteSortMode.Deferred,
        BlendState blendState = null,
        SamplerState samplerState = null,
        DepthStencilState depthStencilState = null,
        RasterizerState rasterizerState = null,
        Effect effect = null,
        Matrix? transformMatrix = null)
    {
        SortMode = sortMode;
        BlendState = blendState ?? BlendState.AlphaBlend;
        SamplerState = samplerState ?? SamplerState.LinearClamp;
        DepthStencilState = depthStencilState ?? DepthStencilState.None;
        RasterizerState = rasterizerState ?? RasterizerState.CullCounterClockwise;
        Effect = effect;
        TransformMatrix = transformMatrix ?? Matrix.Identity;
    }
}
