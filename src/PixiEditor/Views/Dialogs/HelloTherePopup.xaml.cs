﻿using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using PixiEditor.Helpers;
using PixiEditor.Models.Commands;
using PixiEditor.Models.DataHolders;
using PixiEditor.ViewModels.SubViewModels.Main;

namespace PixiEditor.Views.Dialogs;

/// <summary>
/// Interaction logic for HelloTherePopup.xaml.
/// </summary>
internal partial class HelloTherePopup : Window
{
    public RecentlyOpenedCollection RecentlyOpened { get => FileViewModel.RecentlyOpened; }

    public static readonly DependencyProperty FileViewModelProperty =
        DependencyProperty.Register(nameof(FileViewModel), typeof(FileViewModel), typeof(HelloTherePopup));

    public static readonly DependencyProperty RecentlyOpenedEmptyProperty =
        DependencyProperty.Register(nameof(RecentlyOpenedEmpty), typeof(bool), typeof(HelloTherePopup));

    public static string VersionText =>
        $"v{VersionHelpers.GetCurrentAssemblyVersionString()}";

    public FileViewModel FileViewModel { get => (FileViewModel)GetValue(FileViewModelProperty); set => SetValue(FileViewModelProperty, value); }

    public bool RecentlyOpenedEmpty { get => (bool)GetValue(RecentlyOpenedEmptyProperty); set => SetValue(RecentlyOpenedEmptyProperty, value); }

    public RelayCommand OpenFileCommand { get; set; }

    public RelayCommand OpenNewFileCommand { get; set; }

    public RelayCommand OpenRecentCommand { get; set; }

    public RelayCommand OpenInExplorerCommand { get; set; }

    public bool IsClosing { get; private set; }

    public HelloTherePopup(FileViewModel fileViewModel)
    {
        DataContext = this;
        Owner = Application.Current.MainWindow;
        FileViewModel = fileViewModel;

        OpenFileCommand = new RelayCommand(OpenFile);
        OpenNewFileCommand = new RelayCommand(OpenNewFile);
        OpenRecentCommand = new RelayCommand(OpenRecent);
        OpenInExplorerCommand = new RelayCommand(OpenInExplorer, CanOpenInExplorer);

        RecentlyOpenedEmpty = RecentlyOpened.Count == 0;
        RecentlyOpened.CollectionChanged += RecentlyOpened_CollectionChanged;

        Closing += (_, _) => { IsClosing = true; };

        InitializeComponent();

        if (RecentlyOpenedEmpty)
        {
            Width = 400;
            Height = 500;
        }
        else if (RecentlyOpened.Count < 4)
        {
            Width = 445;
            Height = 500;
        }
        else if (RecentlyOpened.Count < 7)
        {
            Width = 475;
            Height = 670;
        }
    }

    private void RecentlyOpened_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RecentlyOpenedEmpty = FileViewModel.RecentlyOpened.Count == 0;
    }

    private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
    {
        SystemCommands.CloseWindow(this);
    }

    private void OpenFile(object parameter)
    {
        Application.Current.MainWindow.Activate();
        Close();
        FileViewModel.OpenFromOpenFileDialog();
    }

    private void OpenNewFile(object parameter)
    {
        Application.Current.MainWindow.Activate();
        Close();
        FileViewModel.CreateFromNewFileDialog();
    }

    private void OpenRecent(object parameter)
    {
        Application.Current.MainWindow.Activate();
        Close();
        FileViewModel.OpenRecent(parameter);
    }

    private void OpenInExplorer(object parameter)
    {
        if (parameter is not string value) return;
        ProcessHelper.OpenInExplorer(value);
    }

    private bool CanOpenInExplorer(object parameter) => File.Exists((string)parameter);
}
