using System.Collections.Generic;
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
using ReactiveUI;

namespace TransData.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
  public IReadOnlyList<MenuItemViewModel> FileMenuItems { get; set; }

  public ReactiveCommand<Unit, Unit> OpenFromFileCommand { get; }
  public ReactiveCommand<Unit, Unit> OpenFromDatabaseCommand { get; }
  public ReactiveCommand<Unit, Unit> SaveToFileCommand { get; }
  public ReactiveCommand<Unit, Unit> SaveToDatabaseCommand { get; }
  public ReactiveCommand<Unit, Unit> ExitCommand { get; }

  [ObservableProperty]
  public string _InputFilePath = string.Empty;

  [ObservableProperty]
  public string _OutputFilePath = string.Empty;

  public DataTable InputDataTable { get; set; } = new();

  private readonly App _parent;

  public MainWindowViewModel(App parent)
  {
    _parent = parent;

    OpenFromFileCommand = ReactiveCommand.CreateFromTask(DoOpenFromFileCommand);
    OpenFromDatabaseCommand = ReactiveCommand.Create(() => { Debug.WriteLine("OpenFromDatabaseCommand"); });
    SaveToFileCommand = ReactiveCommand.CreateFromTask(DoSaveToFileCommand);
    SaveToDatabaseCommand = ReactiveCommand.Create(() => { Debug.WriteLine("SaveToDatabaseCommand"); });

    ExitCommand = ReactiveCommand.Create(DoExitCommand);

    FileMenuItems =
    [
      new MenuItemViewModel
      {
        Header = "_Open",
        Items =
        [
          new MenuItemViewModel {Header = "From _file...", Command = OpenFromFileCommand},
          new MenuItemViewModel {Header = "From _database...", Command = OpenFromDatabaseCommand, IsEnabled = false}
        ]
      },
      new MenuItemViewModel
      {
        Header = "_Save",
        Items =
        [
          new MenuItemViewModel {Header = "To _file...", Command = SaveToFileCommand},
          new MenuItemViewModel {Header = "To _database...", Command = SaveToDatabaseCommand, IsEnabled = false}
        ]
      },
      new MenuItemViewModel {Header = "-"},
      new MenuItemViewModel {Header = "E_xit", Command = ExitCommand}
    ];
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

    InputFilePath = files.Single().Path.AbsolutePath;
    var topLines = File.ReadAllLines(InputFilePath).Take(20);
    var sb = new StringBuilder();
    foreach (var line in topLines)
    {
      sb.AppendLine(line);
    }

    using var reader = new StringReader(sb.ToString());
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    using var dr = new CsvDataReader(csv);
    InputDataTable.Load(dr);
    OnPropertyChanged(nameof(InputDataTable));
    OnPropertyChanged(nameof(InputDataTable.DefaultView));
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

  private void DoExitCommand()
  {
    if (_parent.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app)
    {
      app.Shutdown();
    }
  }
}
