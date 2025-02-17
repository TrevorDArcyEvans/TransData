using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

  public ObservableCollection<string> AvailableColumnActions { get; set; } = ["aaa", "bbb", "ccc", "ddd", "eee"];

  [ObservableProperty]
  public string _SelectedAvailableColumnAction = string.Empty;

  public ObservableCollection<string> ActiveColumnActions { get; set; } = ["fff", "ggg", "hhh", "iii", "jjj"];

  [ObservableProperty]
  public string _SelectedActiveColumnAction = string.Empty;

  private readonly App _parent;

  public MainWindowViewModel(App parent)
  {
    _parent = parent;

    OpenFromFileCommand = ReactiveCommand.CreateFromTask(DoOpenFromFileCommand);
    SaveToFileCommand = ReactiveCommand.CreateFromTask(DoSaveToFileCommand);

    AddColumnActionCommand = ReactiveCommand.Create(() => { Debug.WriteLine("AddColumnActionCommand"); });
    RemoveColumnActionCommand = ReactiveCommand.Create(() => { Debug.WriteLine("RemoveColumnActionCommand"); });
    MoveUpColumnActionCommand = ReactiveCommand.Create(() => { Debug.WriteLine("MoveUpColumnActionCommand"); });
    MoveDownColumnActionCommand = ReactiveCommand.Create(() => { Debug.WriteLine("MoveDownColumnActionCommand"); });

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
}
