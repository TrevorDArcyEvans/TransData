using System.Reactive;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace TransData.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
  public ReactiveCommand<Unit, Unit> ExitCommand { get; }

  private readonly App _parent;

  public MainWindowViewModel(App parent)
  {
    _parent = parent;
    ExitCommand = ReactiveCommand.Create(DoExitCommand);
  }

  private void DoExitCommand()
  {
    if (_parent.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app)
    {
      app.Shutdown();
    }
  }
}
