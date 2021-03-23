﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PixiEditor.Helpers;
using PixiEditor.Helpers.Extensions;

namespace PixiEditor.Models.Layers
{
    public class StructuredLayerTree : NotifyableObject
    {
        private readonly List<Layer> layersInStructure = new ();

        public ObservableCollection<object> RootDirectoryItems { get; } = new ObservableCollection<object>();

        public StructuredLayerTree(ObservableCollection<Layer> layers, LayerStructure structure)
        {
            if (structure.Folders == null || structure.Folders.Count == 0)
            {
                RootDirectoryItems.AddRange(layers);
                return;
            }

            var parsedFolders = ParseFolders(structure.Folders, layers);

            parsedFolders = parsedFolders.OrderBy(x => x.DisplayIndex).ToList();

            RootDirectoryItems.AddRange(parsedFolders);

            RootDirectoryItems.AddRange(layers.Where(x => !layersInStructure.Contains(x)));

            UpdateFoldersDisplayIndexes(parsedFolders);

            layersInStructure.Clear();
        }

        private void UpdateFoldersDisplayIndexes(IList<LayerFolder> parsedFolders)
        {
            for (int i = parsedFolders.Count - 1; i > 0; i--)
            {
                LayerFolder layerFolder = parsedFolders[i];
                int oldDisplayIndex = layerFolder.DisplayIndex;
                int collapsedLayers = ((List<object>)parsedFolders[i - 1].Items).Count;
                int newIndex = oldDisplayIndex - collapsedLayers + 1;
                int difference = newIndex - oldDisplayIndex;
                if (layerFolder.Subfolders.Count > 0)
                {
                    UpdateDisplayIndexes(layerFolder.Subfolders, difference);
                }

                layerFolder.DisplayIndex = newIndex;
                layerFolder.TopIndex += newIndex - oldDisplayIndex;
                RootDirectoryItems.Move(RootDirectoryItems.IndexOf(layerFolder), layerFolder.DisplayIndex);
            }

            if (parsedFolders.Count > 0)
            {
                RootDirectoryItems.Move(0, parsedFolders[0].DisplayIndex);
            }
        }

        private void UpdateDisplayIndexes(ObservableCollection<LayerFolder> subfolders, int difference)
        {
            for (int i = 0; i < subfolders.Count; i++)
            {
                if (subfolders[i].Subfolders.Count > 0)
                {
                    UpdateDisplayIndexes(subfolders[i].Subfolders, difference);
                }

                subfolders[i].DisplayIndex += difference;
            }
        }

        private List<LayerFolder> ParseFolders(IEnumerable<GuidStructureItem> folders, ObservableCollection<Layer> layers)
        {
            List<LayerFolder> parsedFolders = new ();
            foreach (var structureItem in folders)
            {
                parsedFolders.Add(ParseFolder(structureItem, layers));
            }

            return parsedFolders;
        }

        private LayerFolder ParseFolder(GuidStructureItem structureItem, ObservableCollection<Layer> layers)
        {
            List<Layer> structureItemLayers = new ();

            Guid[] layersInFolder = GetLayersInFolder(layers, structureItem);

            var subFolders = new List<LayerFolder>();

            if (structureItem.Subfolders.Count > 0)
            {
                subFolders = ParseFolders(structureItem.Subfolders, layers);
            }

            foreach (var guid in layersInFolder)
            {
                var layer = layers.First(x => x.LayerGuid == guid);
                if (!layersInStructure.Contains(layer))
                {
                    layersInStructure.Add(layer);
                    structureItemLayers.Add(layer);
                }
            }

            int displayIndex = layersInFolder.Length > 0 ? layersInStructure.Min(x => layers.IndexOf(x)) : 0;

            structureItemLayers.Reverse();

            LayerFolder folder = new (structureItemLayers, subFolders, structureItem.Name,
                structureItem.FolderGuid, displayIndex, displayIndex + structureItemLayers.Count - 1)
            {
                IsExpanded = structureItem.IsExpanded
            };
            return folder;
        }

        private Guid[] GetLayersInFolder(ObservableCollection<Layer> layers, GuidStructureItem structureItem)
        {
            if (structureItem.EndLayerGuid == null || structureItem.StartLayerGuid == null)
            {
                return Array.Empty<Guid>();
            }

            int startIndex = layers.IndexOf(layers.First(x => x.LayerGuid == structureItem.StartLayerGuid));
            int endIndex = layers.IndexOf(layers.First(x => x.LayerGuid == structureItem.EndLayerGuid));

            if (startIndex > endIndex)
            {
                Swap(ref startIndex, ref endIndex);
            }

            int len = endIndex - startIndex + 1;

            Guid[] guids = new Guid[len];

            for (int i = 0; i < len; i++)
            {
                guids[i] = layers[i + startIndex].LayerGuid;
            }

            return guids;
        }

        private static void Swap(ref int startIndex, ref int endIndex)
        {
            int tmp = startIndex;
            startIndex = endIndex;
            endIndex = tmp;
        }
    }
}