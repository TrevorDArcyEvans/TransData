using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper;
using CsvHelper.Configuration;
using ReactiveUI;
using TransData.App.ColumnActions;
using TransData.App.Interfaces;

namespace TransData.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
  public ReactiveCommand<Unit, Unit> OpenFromFileCommand { get; }
  public ReactiveCommand<Unit, Unit> SaveToFileCommand { get; }
  public ReactiveCommand<Unit, Unit> AddColumnActionCommand { get; }
  public ReactiveCommand<Unit, Unit> RemoveColumnActionCommand { get; }
  public ReactiveCommand<Unit, Unit> MoveUpColumnActionCommand { get; }
  public ReactiveCommand<Unit, Unit> MoveDownColumnActionCommand { get; }

  [ObservableProperty]
  public string _InputFilePath = string.Empty;

  [ObservableProperty]
  public string _OutputFilePath = string.Empty;

  [ObservableProperty]
  public DataColumn _SelectedColumn = new();

  public DataTable InputDataTable { get; set; } = new();
  public DataTable TransformedInputDataTable { get; set; } = new();

  public ObservableCollection<IColumnActionFactory> AvailableColumnActions { get; set; } =
  [
    new ReplaceBlankFactory(),
    new FixedTextFactory()
  ];

  [ObservableProperty]
  public IColumnActionFactory _SelectedAvailableColumnAction;

  public ObservableCollection<IColumnAction> ActiveColumnActions { get; set; } = new();

  [ObservableProperty]
  public IColumnAction _SelectedActiveColumnAction;

  private readonly App _parent;

  public MainWindowViewModel(App parent)
  {
    _parent = parent;

    OpenFromFileCommand = ReactiveCommand.CreateFromTask(DoOpenFromFileCommand);
    SaveToFileCommand = ReactiveCommand.CreateFromTask(DoSaveToFileCommand);

    AddColumnActionCommand = ReactiveCommand.Create(DoAddColumnActionCommand); //,
    // this.WhenAnyValue(
    //   x => x.SelectedAvailableColumnAction,
    // x => x != null));
    RemoveColumnActionCommand = ReactiveCommand.Create(DoRemoveColumnActionCommand); //,
    // this.WhenAnyValue(
    //   x => x.SelectedActiveColumnAction,
    // x => x != null));
    MoveUpColumnActionCommand = ReactiveCommand.Create(
      DoMoveUpColumnActionCommand,
      this.WhenAnyValue(
        x => x.SelectedActiveColumnAction,
        x => ActiveColumnActions.IndexOf(x) > 0));
    MoveDownColumnActionCommand = ReactiveCommand.Create(
      DoMoveDownColumnActionCommand,
      this.WhenAnyValue(
        x => x.SelectedActiveColumnAction,
        x => ActiveColumnActions.IndexOf(x) < ActiveColumnActions.Count - 1 && x != null));

    PropertyChanged += OnPropertyChanged;
  }

  private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(InputFilePath):
        if (File.Exists(InputFilePath))
        {
          LoadInputFile();
        }
        else
        {
          ClearInputDataTable();
        }

        break;
    }
  }

  private async Task DoOpenFromFileCommand()
  {
    var app = (IClassicDesktopStyleApplicationLifetime) _parent.ApplicationLifetime!;
    var topLevel = TopLevel.GetTopLevel(app.MainWindow);

    // Start async operation to open the dialog.
    var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
    {
      Title = "Open CSV File",
      FileTypeFilter =
      [
        new FilePickerFileType("CSV files (*.csv)") {Patterns = ["*.csv"]},
        new FilePickerFileType("All files (*.*)") {Patterns = ["*.*"]}
      ],
      AllowMultiple = false
    });

    if (!files.Any())
    {
      return;
    }

    // will fire property changed
    InputFilePath = files.Single().Path.LocalPath;
  }

  private void LoadInputFile()
  {
    // cannot reuse because DefaultView does not get updated
    InputDataTable = new();

    var topLines = File.ReadAllLines(InputFilePath).Take(200);
    var sb = new StringBuilder();
    foreach (var line in topLines)
    {
      sb.AppendLine(line);
    }

    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
      HasHeaderRecord = true,
      HeaderValidated = null,
      MissingFieldFound = null,
      IgnoreBlankLines = false,
      BadDataFound = null,
      TrimOptions = TrimOptions.Trim | TrimOptions.InsideQuotes
    };
    using var reader = new StringReader(sb.ToString());
    using var csv = new CsvReader(reader, config);
    using var dr = new CsvDataReader(csv);
    InputDataTable.Load(dr);
    TransformedInputDataTable = InputDataTable.Copy();

    OnPropertyChanged(nameof(InputDataTable));
  }

  private void ClearInputDataTable()
  {
    InputDataTable.Reset();

    OnPropertyChanged(nameof(InputDataTable));
  }

  private async Task DoSaveToFileCommand()
  {
    var app = (IClassicDesktopStyleApplicationLifetime) _parent.ApplicationLifetime!;
    var topLevel = TopLevel.GetTopLevel(app.MainWindow);

    // Start async operation to open the dialog.
    var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
    {
      Title = "Save CSV File",
      FileTypeChoices =
      [
        new FilePickerFileType("CSV files (*.csv)") {Patterns = ["*.csv"]},
        new FilePickerFileType("All files (*.*)") {Patterns = ["*.*"]}
      ],
      DefaultExtension = ".csv",
      ShowOverwritePrompt = true,
    });

    if (file is null)
    {
      return;
    }

    OutputFilePath = file.Path.AbsolutePath;
  }

  private void DoAddColumnActionCommand()
  {
    ActiveColumnActions.Add(SelectedAvailableColumnAction.Create());
    OnPropertyChanged(nameof(ActiveColumnActions));
  }

  private void DoRemoveColumnActionCommand()
  {
    ActiveColumnActions.Remove(SelectedActiveColumnAction);
    OnPropertyChanged(nameof(ActiveColumnActions));
  }

  private void DoMoveUpColumnActionCommand()
  {
    var idx = ActiveColumnActions.IndexOf(SelectedActiveColumnAction);
    Swap(ActiveColumnActions, idx, idx - 1);
    OnPropertyChanged(nameof(ActiveColumnActions));
  }

  private void DoMoveDownColumnActionCommand()
  {
    var idx = ActiveColumnActions.IndexOf(SelectedActiveColumnAction);
    Swap(ActiveColumnActions, idx, idx + 1);
    OnPropertyChanged(nameof(ActiveColumnActions));
  }

  private static void Swap<T>(IList<T> list, int indexA, int indexB)
  {
    (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
  }
}
