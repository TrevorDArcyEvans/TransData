using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using Avalonia.Controls.ApplicationLifetimes;
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

  private readonly App _parent;

  public MainWindowViewModel(App parent)
  {
    _parent = parent;

    OpenFromFileCommand = ReactiveCommand.Create(() => { Debug.WriteLine("OpenFromFileCommand"); });
    OpenFromDatabaseCommand = ReactiveCommand.Create(() => { Debug.WriteLine("OpenFromDatabaseCommand"); });
    SaveToFileCommand = ReactiveCommand.Create(() => { Debug.WriteLine("SaveToFileCommand"); });
    SaveToDatabaseCommand = ReactiveCommand.Create(() => { Debug.WriteLine("SaveToDatabaseCommand"); });

    ExitCommand = ReactiveCommand.Create(DoExitCommand);

    FileMenuItems =
    [
      new MenuItemViewModel
      {
        Header = "_Open",
        Items =
        [
          new MenuItemViewModel { Header = "From _file...", Command = OpenFromFileCommand },
          new MenuItemViewModel { Header = "From _database...", Command = OpenFromDatabaseCommand, IsEnabled = false }
        ]
      },
      new MenuItemViewModel
      {
        Header = "_Save",
        Items =
        [
          new MenuItemViewModel { Header = "To _file...", Command = SaveToFileCommand },
          new MenuItemViewModel { Header = "To _database...", Command = SaveToDatabaseCommand, IsEnabled = false }
        ]
      },
      new MenuItemViewModel { Header = "-" },
      new MenuItemViewModel { Header = "E_xit", Command = ExitCommand }
    ];
  }

  private void DoExitCommand()
  {
    if (_parent.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app)
    {
      app.Shutdown();
    }
  }
}
