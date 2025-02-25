﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using ChunkyImageLib.Operations;
using PixiEditor.ChangeableDocument.Enums;
using PixiEditor.Models.Commands.Attributes.Commands;
using PixiEditor.Models.Dialogs;
using PixiEditor.Models.Enums;
using PixiEditor.Models.Events;
using PixiEditor.ViewModels.SubViewModels.Tools.Tools;
using PixiEditor.Views.UserControls.SymmetryOverlay;

namespace PixiEditor.ViewModels.SubViewModels.Document;
#nullable enable
[Command.Group("PixiEditor.Document", "Image")]
internal class DocumentManagerViewModel : SubViewModel<ViewModelMain>
{
    public ObservableCollection<DocumentViewModel> Documents { get; } = new ObservableCollection<DocumentViewModel>();
    public event EventHandler<DocumentChangedEventArgs>? ActiveDocumentChanged;


    private DocumentViewModel? activeDocument;
    public DocumentViewModel? ActiveDocument
    {
        get => activeDocument;
        // Use WindowSubViewModel.MakeDocumentViewportActive(document);
        private set
        {
            if (activeDocument == value)
                return;
            DocumentViewModel? prevDoc = activeDocument;
            activeDocument = value;
            RaisePropertyChanged(nameof(ActiveDocument));
            ActiveDocumentChanged?.Invoke(this, new(value, prevDoc));
            
            if (ViewModelMain.Current.ToolsSubViewModel.ActiveTool == null)
            {
                ViewModelMain.Current.ToolsSubViewModel.SetActiveTool<PenToolViewModel>(false);
            }
        }
    }

    public DocumentManagerViewModel(ViewModelMain owner) : base(owner)
    {
        owner.WindowSubViewModel.ActiveViewportChanged += (_, args) => ActiveDocument = args.Document;
    }

    public void MakeActiveDocumentNull() => ActiveDocument = null;

    [Evaluator.CanExecute("PixiEditor.HasDocument")]
    public bool DocumentNotNull() => ActiveDocument != null;

    [Command.Basic("PixiEditor.Document.ClipCanvas", "Clip Canvas", "Clip Canvas", CanExecute = "PixiEditor.HasDocument")]
    public void ClipCanvas() => ActiveDocument?.Operations.ClipCanvas();

    [Command.Basic("PixiEditor.Document.FlipImageHorizontal", FlipType.Horizontal, "Flip Image Horizontally", "Flip Image Horizontally", CanExecute = "PixiEditor.HasDocument")]
    [Command.Basic("PixiEditor.Document.FlipImageVertical", FlipType.Vertical, "Flip Image Vertically", "Flip Image Vertically", CanExecute = "PixiEditor.HasDocument")]
    public void FlipImage(FlipType type) => ActiveDocument?.Operations.FlipImage(type);

    [Command.Basic("PixiEditor.Document.FlipLayersHorizontal", FlipType.Horizontal, "Flip Selected Layers Horizontally", "Flip Selected Layers Horizontally", CanExecute = "PixiEditor.HasDocument")]
    [Command.Basic("PixiEditor.Document.FlipLayersVertical", FlipType.Vertical, "Flip Selected Layers Vertically", "Flip Selected Layers Vertically", CanExecute = "PixiEditor.HasDocument")]
    public void FlipLayers(FlipType type)
    {
        if (ActiveDocument?.SelectedStructureMember == null)
            return;
        
        ActiveDocument?.Operations.FlipImage(type, ActiveDocument.GetSelectedMembers());
    }
    
    [Command.Basic("PixiEditor.Document.Rotate90Deg", "Rotate Image 90 degrees", 
        "Rotate Image 90 degrees", CanExecute = "PixiEditor.HasDocument", Parameter = RotationAngle.D90)]
    [Command.Basic("PixiEditor.Document.Rotate180Deg", "Rotate Image 180 degrees", 
        "Rotate Image 180 degrees", CanExecute = "PixiEditor.HasDocument", Parameter = RotationAngle.D180)]
    [Command.Basic("PixiEditor.Document.Rotate270Deg", "Rotate Image -90 degrees", 
        "Rotate Image -90 degrees", CanExecute = "PixiEditor.HasDocument", Parameter = RotationAngle.D270)]
    public void RotateImage(RotationAngle angle) => ActiveDocument?.Operations.RotateImage(angle);

    [Command.Basic("PixiEditor.Document.Rotate90DegLayers", "Rotate Selected Layers 90 degrees", 
        "Rotate Selected Layers 90 degrees", CanExecute = "PixiEditor.HasDocument", Parameter = RotationAngle.D90)]
    [Command.Basic("PixiEditor.Document.Rotate180DegLayers", "Rotate Selected Layers 180 degrees", 
        "Rotate Selected Layers 180 degrees", CanExecute = "PixiEditor.HasDocument", Parameter = RotationAngle.D180)]
    [Command.Basic("PixiEditor.Document.Rotate270DegLayers", "Rotate Selected Layers -90 degrees", 
        "Rotate Selected Layers -90 degrees", CanExecute = "PixiEditor.HasDocument", Parameter = RotationAngle.D270)]
    public void RotateLayers(RotationAngle angle)
    {
        if (ActiveDocument?.SelectedStructureMember == null)
            return;
        
        ActiveDocument?.Operations.RotateImage(angle, ActiveDocument.GetSelectedMembers());
    }

    [Command.Basic("PixiEditor.Document.ToggleVerticalSymmetryAxis", "Toggle vertical symmetry axis", "Toggle vertical symmetry axis", CanExecute = "PixiEditor.HasDocument", IconPath = "SymmetryVertical.png")]
    public void ToggleVerticalSymmetryAxis()
    {
        if (ActiveDocument is null)
            return;
        ActiveDocument.VerticalSymmetryAxisEnabledBindable ^= true;
    }

    [Command.Basic("PixiEditor.Document.ToggleHorizontalSymmetryAxis", "Toggle horizontal symmetry axis", "Toggle horizontal symmetry axis", CanExecute = "PixiEditor.HasDocument", IconPath = "SymmetryHorizontal.png")]
    public void ToggleHorizontalSymmetryAxis()
    {
        if (ActiveDocument is null)
            return;
        ActiveDocument.HorizontalSymmetryAxisEnabledBindable ^= true;
    }

    [Command.Internal("PixiEditor.Document.DragSymmetry", CanExecute = "PixiEditor.HasDocument")]
    public void DragSymmetry(SymmetryAxisDragInfo info)
    {
        if (ActiveDocument is null)
            return;
        ActiveDocument.EventInlet.OnSymmetryDragged(info);
    }

    [Command.Internal("PixiEditor.Document.StartDragSymmetry", CanExecute = "PixiEditor.HasDocument")]
    public void StartDragSymmetry(SymmetryAxisDirection dir)
    {
        if (ActiveDocument is null)
            return;
        ActiveDocument.EventInlet.OnSymmetryDragStarted(dir);
        ActiveDocument.Tools.UseSymmetry(dir);
    }

    [Command.Internal("PixiEditor.Document.EndDragSymmetry", CanExecute = "PixiEditor.HasDocument")]
    public void EndDragSymmetry(SymmetryAxisDirection dir)
    {
        if (ActiveDocument is null)
            return;
        ActiveDocument.EventInlet.OnSymmetryDragEnded(dir);
    }

    [Command.Basic("PixiEditor.Document.DeletePixels", "Delete pixels", "Delete selected pixels", CanExecute = "PixiEditor.Selection.IsNotEmpty", Key = Key.Delete, IconPath = "Tools/EraserImage.png")]
    public void DeletePixels()
    {
        Owner.DocumentManagerSubViewModel.ActiveDocument?.Operations.DeleteSelectedPixels();
    }


    [Command.Basic("PixiEditor.Document.ResizeDocument", false, "Resize Document", "Resize Document", CanExecute = "PixiEditor.HasDocument", Key = Key.I, Modifiers = ModifierKeys.Control | ModifierKeys.Shift)]
    [Command.Basic("PixiEditor.Document.ResizeCanvas", true, "Resize Canvas", "Resize Canvas", CanExecute = "PixiEditor.HasDocument", Key = Key.C, Modifiers = ModifierKeys.Control | ModifierKeys.Shift)]
    public void OpenResizePopup(bool canvas)
    {
        DocumentViewModel? doc = Owner.DocumentManagerSubViewModel.ActiveDocument;
        if (doc is null)
            return;

        ResizeDocumentDialog dialog = new ResizeDocumentDialog(
            doc.Width,
            doc.Height,
            canvas);
        if (dialog.ShowDialog())
        {
            if (canvas)
            {
                doc.Operations.ResizeCanvas(new(dialog.Width, dialog.Height), dialog.ResizeAnchor);
            }
            else
            {
                doc.Operations.ResizeImage(new(dialog.Width, dialog.Height), ResamplingMethod.NearestNeighbor);
            }
        }
    }

    [Command.Basic("PixiEditor.Document.CenterContent", "Center Content", "Center Content", CanExecute = "PixiEditor.HasDocument")]
    public void CenterContent()
    {
        if(ActiveDocument?.SelectedStructureMember == null)
            return;
        
        ActiveDocument.Operations.CenterContent(ActiveDocument.GetSelectedMembers());
    }
}
