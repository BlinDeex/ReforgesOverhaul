using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ModifiersOverhaul.Assets.Misc;

public struct SpriteBatchSnapshot(
    SpriteSortMode sortMode,
    BlendState blendState,
    SamplerState samplerState,
    DepthStencilState depthStencilState,
    RasterizerState rasterizerState,
    Effect effect,
    Matrix transformMatrix)
{
    public SpriteSortMode SortMode = sortMode;
    public BlendState BlendState = blendState;
    public SamplerState SamplerState = samplerState;
    public DepthStencilState DepthStencilState = depthStencilState;
    public RasterizerState RasterizerState = rasterizerState;
    public Effect Effect = effect;
    public Matrix TransformMatrix = transformMatrix;

    public void Begin(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(in this);
    }

    public static SpriteBatchSnapshot Capture(SpriteBatch spriteBatch)
    {
        var sortMode = (SpriteSortMode)SpriteBatchSnapshotCache.SortModeField.GetValue(spriteBatch)!;

        var blendState = (BlendState)SpriteBatchSnapshotCache.BlendStateField.GetValue(spriteBatch);

        var samplerState = (SamplerState)SpriteBatchSnapshotCache.SamplerStateField.GetValue(spriteBatch);

        var depthStencilState =
            (DepthStencilState)SpriteBatchSnapshotCache.DepthStencilStateField.GetValue(spriteBatch);

        var rasterizerState =
            (RasterizerState)SpriteBatchSnapshotCache.RasterizerStateField.GetValue(spriteBatch);

        var effect = (Effect)SpriteBatchSnapshotCache.EffectField.GetValue(spriteBatch);

        var transformMatrix = (Matrix)SpriteBatchSnapshotCache.TransformMatrixField.GetValue(spriteBatch)!;

        return new SpriteBatchSnapshot(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect,
            transformMatrix);
    }
}