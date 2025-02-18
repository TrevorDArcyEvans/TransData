using System.ComponentModel;
using System.Data;
using Avalonia.Controls;
using Avalonia.Data;
using TransData.App.ViewModels;

namespace TransData.App.Views;

public partial class MainWindow : Window
{
  private const string TransformSuffix = " [Tx]";

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
          gridCol.Binding = new Binding($"Row.ItemArray[{x.Ordinal}]");
          InputDataGrid.Columns.Add(gridCol);
        }

        break;
      }

      case nameof(vm.SelectedColumn):
      {
        // clear out any existing columns
        FilteredInputDataGrid.Columns.Clear();

        // assign the datatable to the grid
        FilteredInputDataGrid.ItemsSource = vm.TransformedInputDataTable.DefaultView;

        if (vm.SelectedColumn == null)
        {
          break;
        }

        var selTxColName = vm.SelectedColumn.ColumnName + TransformSuffix;
        if (!vm.TransformedInputDataTable.Columns.Contains(selTxColName))
        {
          vm.TransformedInputDataTable.Columns.Add(selTxColName);
        }

        FilteredInputDataGrid.Columns.Add(GetDataGridBoundColumn(vm));
        FilteredInputDataGrid.Columns.Add(GetDataGridBoundColumn(vm, TransformSuffix));

        UpdateTransformedColumn(vm);

        break;
      }

      case nameof(vm.ActiveColumnActions):
      {
        UpdateTransformedColumn(vm);

        break;
      }
    }
  }

  private static void UpdateTransformedColumn(MainWindowViewModel vm)
  {
    // "" --> MISSING
    foreach (DataRow row in vm.TransformedInputDataTable.Rows)
    {
      var currVal = (string)row[vm.SelectedColumn.ColumnName];
      row[vm.SelectedColumn.ColumnName + TransformSuffix] = string.IsNullOrWhiteSpace(currVal) ? "MISSING" : currVal;
    }
  }

  private static DataGridBoundColumn GetDataGridBoundColumn(MainWindowViewModel vm, string suffix = null)
  {
    DataGridBoundColumn gridCol = vm.SelectedColumn.DataType == typeof(bool) ? new DataGridCheckBoxColumn() : new DataGridTextColumn();

    var colName = vm.SelectedColumn.ColumnName + suffix;
    var colIdx = vm.TransformedInputDataTable.Columns.IndexOf(colName);
    var selCol = vm.TransformedInputDataTable.Columns[colIdx];
    var selColOrd = selCol.Ordinal;
    gridCol.Header = colName;
    gridCol.Binding = new Binding($"Row.ItemArray[{selColOrd}]");
    return gridCol;
  }
}
