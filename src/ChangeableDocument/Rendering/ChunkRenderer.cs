﻿using ChangeableDocument.Changeables.Interfaces;
using ChunkyImageLib;
using ChunkyImageLib.DataHolders;
using SkiaSharp;

namespace ChangeableDocument.Rendering
{
    public static class ChunkRenderer
    {
        private static SKPaint PaintToDrawChunksWith = new SKPaint() { BlendMode = SKBlendMode.SrcOver };
        public static Chunk RenderWholeStructure(Vector2i pos, IReadOnlyFolder root)
        {
            return RenderChunkRecursively(pos, 0, root, null);
        }

        public static Chunk RenderSpecificLayers(Vector2i pos, IReadOnlyFolder root, HashSet<Guid> layers)
        {
            return RenderChunkRecursively(pos, 0, root, layers);
        }

        private static Chunk RenderChunkRecursively(Vector2i chunkPos, int depth, IReadOnlyFolder folder, HashSet<Guid>? visibleLayers)
        {
            Chunk targetChunk = Chunk.Create();
            targetChunk.Surface.SkiaSurface.Canvas.Clear();
            foreach (var child in folder.ReadOnlyChildren)
            {
                if (!child.IsVisible)
                    continue;
                if (child is IReadOnlyLayer layer && (visibleLayers == null || visibleLayers.Contains(layer.GuidValue)))
                {
                    IReadOnlyChunk? chunk = layer.ReadOnlyLayerImage.GetLatestChunk(chunkPos);
                    if (chunk == null)
                        continue;
                    PaintToDrawChunksWith.Color = new SKColor(255, 255, 255, (byte)Math.Round(child.Opacity * 255));
                    chunk.DrawOnSurface(targetChunk.Surface.SkiaSurface, new(0, 0), PaintToDrawChunksWith);
                }
                else if (child is IReadOnlyFolder innerFolder)
                {
                    using Chunk renderedChunk = RenderChunkRecursively(chunkPos, depth + 1, innerFolder, visibleLayers);
                    PaintToDrawChunksWith.Color = new SKColor(255, 255, 255, (byte)Math.Round(child.Opacity * 255));
                    renderedChunk.DrawOnSurface(targetChunk.Surface.SkiaSurface, new(0, 0), PaintToDrawChunksWith);
                }
            }
            return targetChunk;
        }
    }
}
