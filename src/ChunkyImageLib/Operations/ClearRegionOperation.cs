﻿using ChunkyImageLib.DataHolders;
using PixiEditor.DrawingApi.Core.Numerics;

namespace ChunkyImageLib.Operations;

internal class ClearRegionOperation : IMirroredDrawOperation
{
    private RectI rect;

    public bool IgnoreEmptyChunks => true;

    public ClearRegionOperation(RectI rect)
    {
        this.rect = rect;
    }

    public void DrawOnChunk(Chunk chunk, VecI chunkPos)
    {
        VecI convPos = OperationHelper.ConvertForResolution(rect.Pos, chunk.Resolution);
        VecI convSize = OperationHelper.ConvertForResolution(rect.Size, chunk.Resolution);

        chunk.Surface.DrawingSurface.Canvas.Save();
        chunk.Surface.DrawingSurface.Canvas.ClipRect(RectD.Create(convPos - chunkPos.Multiply(chunk.PixelSize), convSize));
        chunk.Surface.DrawingSurface.Canvas.Clear();
        chunk.Surface.DrawingSurface.Canvas.Restore();
    }

    public AffectedArea FindAffectedArea(VecI imageSize)
    {
        return new AffectedArea(OperationHelper.FindChunksTouchingRectangle(rect, ChunkPool.FullChunkSize), rect);
    }
    public void Dispose() { }

    public IDrawOperation AsMirrored(int? verAxisX, int? horAxisY)
    {
        var newRect = rect;
        if (verAxisX is not null)
            newRect = newRect.ReflectX((int)verAxisX);
        if (horAxisY is not null)
            newRect = newRect.ReflectY((int)horAxisY);
        return new ClearRegionOperation(newRect);
    }
}
