using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using Avalonia.Controls;
using ReactiveUI;
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
    var vm = (MainWindowViewModel)DataContext!;
    switch (e.PropertyName)
    {
      case nameof(vm.InputDataTable):
      {
        // clear out any existing columns
        InputDataGrid.Columns.Clear();

        // assign the datatable to the grid
        InputDataGrid.ItemsSource = vm.InputDataTable.DefaultView;

        // create the grid columns based on the datatables columns
        foreach (DataColumn x in vm.InputDataTable.Columns)
        {
          DataGridBoundColumn gridCol = x.DataType == typeof(bool) ? new DataGridCheckBoxColumn() : new DataGridTextColumn();

          gridCol.Header = x.ColumnName;
          gridCol.Binding = new Avalonia.Data.Binding($"Row.ItemArray[{x.Ordinal}]");
          InputDataGrid.Columns.Add(gridCol);
        }

        break;
      }

      case nameof(vm.SelectedColumn):
      {
        // clear out any existing columns
        FilteredInputDataGrid.Columns.Clear();

        // assign the datatable to the grid
        FilteredInputDataGrid.ItemsSource = vm.InputDataTable.DefaultView;

        if (vm.SelectedColumn == null)
        {
          break;
        }

        FilteredInputDataGrid.Columns.Add(GetDataGridBoundColumn(vm));
        FilteredInputDataGrid.Columns.Add(GetDataGridBoundColumn(vm, " [Tx]"));

        break;
      }
    }
  }

  private static DataGridBoundColumn GetDataGridBoundColumn(MainWindowViewModel vm, string suffix = null)
  {
    DataGridBoundColumn gridCol = vm.SelectedColumn.DataType == typeof(bool) ? new DataGridCheckBoxColumn() : new DataGridTextColumn();

    gridCol.Header = vm.SelectedColumn.ColumnName + suffix;
    gridCol.Binding = new Avalonia.Data.Binding($"Row.ItemArray[{vm.SelectedColumn.Ordinal}]");
    return gridCol;
  }
}
