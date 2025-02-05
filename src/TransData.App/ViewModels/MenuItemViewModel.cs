using System.Collections.Generic;
using System.Windows.Input;

namespace TransData.App.ViewModels;

public class MenuItemViewModel
{
  public string Header { get; set; }
  public ICommand Command { get; set; }
  public object CommandParameter { get; set; }
  public bool IsEnabled { get; set; } = true;
  public IList<MenuItemViewModel> Items { get; set; }
}
