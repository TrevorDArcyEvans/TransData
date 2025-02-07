using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Controls;
using TransData.App.ViewModels;

namespace TransData.App.Views;

public partial class MainWindow : Window
{
  public MainWindow(MainWindowViewModel viewModel)
  {
    InitializeComponent();

    DataContext = viewModel;

    viewModel.PropertyChanged += OnPropertyChanged;
  }

  private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    var vm = (MainWindowViewModel) DataContext!;
    if (e.PropertyName == nameof(vm.InputDataTable))
    {
      // clear out any existing columns
      while (InputDataGrid.Columns.Count > 0)
      {
        InputDataGrid.Columns.RemoveAt(InputDataGrid.Columns.Count - 1);
      }

      // assign the datatable to the grid
      InputDataGrid.ItemsSource = vm.InputDataTable.DefaultView;

      // create the grid columns based on the datatables columns
      foreach (System.Data.DataColumn x in vm.InputDataTable.Columns)
      {
        DataGridBoundColumn gridCol = x.DataType == typeof(bool) ? new DataGridCheckBoxColumn() : new DataGridTextColumn();

        gridCol.Header = x.ColumnName;
        gridCol.Binding = new Avalonia.Data.Binding($"Row.ItemArray[{x.Ordinal}]");
        InputDataGrid.Columns.Add(gridCol);
      }
    }
  }
}
