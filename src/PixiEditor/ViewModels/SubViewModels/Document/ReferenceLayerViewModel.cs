﻿using System.Collections.Immutable;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChunkyImageLib.DataHolders;
using ChunkyImageLib.Operations;
using PixiEditor.ChangeableDocument.Actions.Generated;
using PixiEditor.DrawingApi.Core.Numerics;
using PixiEditor.Helpers;
using PixiEditor.Models.DocumentModels;

namespace PixiEditor.ViewModels.SubViewModels.Document;

#nullable enable
internal class ReferenceLayerViewModel : INotifyPropertyChanged
{
    private readonly DocumentViewModel doc;
    private readonly DocumentInternalParts internals;
    public event PropertyChangedEventHandler PropertyChanged;
    
    public WriteableBitmap? ReferenceBitmap { get; private set; }

    private ShapeCorners referenceShape;
    public ShapeCorners ReferenceShapeBindable 
    { 
        get => referenceShape; 
        set
        {
            if (!doc.UpdateableChangeActive)
                internals.ActionAccumulator.AddFinishedActions(new TransformReferenceLayer_Action(value));
        }
    }
    
    public Matrix ReferenceTransformMatrix
    {
        get
        {
            if (ReferenceBitmap is null)
                return Matrix.Identity;
            Matrix3X3 skiaMatrix = OperationHelper.CreateMatrixFromPoints((ShapeCorners)ReferenceShapeBindable, new VecD(ReferenceBitmap.Width, ReferenceBitmap.Height));
            return new Matrix(skiaMatrix.ScaleX, skiaMatrix.SkewY, skiaMatrix.SkewX, skiaMatrix.ScaleY, skiaMatrix.TransX, skiaMatrix.TransY);
        }
    }

    private bool isVisible;
    public bool IsVisibleBindable
    {
        get => isVisible;
        set
        {
            if (!doc.UpdateableChangeActive)
                internals.ActionAccumulator.AddFinishedActions(new ReferenceLayerIsVisible_Action(value));
        }
    }

    public ReferenceLayerViewModel(DocumentViewModel doc, DocumentInternalParts internals)
    {
        this.doc = doc;
        this.internals = internals;
    }

    private void RaisePropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    
    #region Internal methods

    public void InternalSetReferenceLayer(ImmutableArray<byte> imagePbgra32Bytes, VecI imageSize, ShapeCorners shape)
    {
        ReferenceBitmap = WriteableBitmapHelpers.FromPbgra32Array(imagePbgra32Bytes.ToArray(), imageSize);
        referenceShape = shape;
        isVisible = true;
        RaisePropertyChanged(nameof(ReferenceBitmap));
        RaisePropertyChanged(nameof(ReferenceShapeBindable));
        RaisePropertyChanged(nameof(ReferenceTransformMatrix));
        RaisePropertyChanged(nameof(IsVisibleBindable));
    }

    public void InternalDeleteReferenceLayer()
    {
        ReferenceBitmap = null;
        isVisible = false;
        RaisePropertyChanged(nameof(ReferenceBitmap));
        RaisePropertyChanged(nameof(ReferenceTransformMatrix));
        RaisePropertyChanged(nameof(IsVisibleBindable));
    }
    
    public void InternalTransformReferenceLayer(ShapeCorners shape)
    {
        referenceShape = shape;
        RaisePropertyChanged(nameof(ReferenceShapeBindable));
        RaisePropertyChanged(nameof(ReferenceTransformMatrix));
    }

    public void InternalSetReferenceLayerIsVisible(bool isVisible)
    {
        this.isVisible = isVisible;
        RaisePropertyChanged(nameof(IsVisibleBindable));
    }

    #endregion
}
