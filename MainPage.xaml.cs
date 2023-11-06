using System.Diagnostics;

namespace MyExcel;

public partial class MainPage : ContentPage
{
    static int CountColumn = 5;
    static int CountRow = 10;

    Dictionary<string, IView> cells = new Dictionary<string, IView>(capacity: (int)(CountColumn*CountRow*1.6));

    public MainPage()
    {
        InitializeComponent();
        CreateGrid();
    }

    //створення таблиці
    private void CreateGrid()
    {
        AddColumnsAndColumnLabels();
        AddRowsAndCellEntries();
    }

    private void AddColumnsAndColumnLabels()
    {
        grid.RowDefinitions.Add(new RowDefinition()); // Row for labels
        // Додати стовпці та підписи для стовпців
        for (int col = 0; col <= CountColumn; col++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            var label = NewLabel(GetColumnName(col));
            AddLabel(label, 0, col);
            cells.Add(label.Text, label);
            Debug.WriteLine("Label {0} added at index {1}", label.Text, grid.Children.Count - 1);
        }
    }

    private void AddRowsAndCellEntries()
    {
        // Додати рядки, підписи для рядків та комірки
        for (int row = 1; row <= CountRow; row++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            // Додати підпис для номера рядка
            var label = NewLabel(row.ToString());
            AddLabel(label, row, 0);
            cells.Add(label.Text, label);
            Debug.WriteLine("Label {0} added at index {1}", label.Text, grid.Children.Count - 1);
            // Додати комірки (Entry) для вмісту
            for (int col = 1; col <= CountColumn; col++)
            {
                var entry = NewEmptyEntry();
                AddEntry(entry, row, col);
                cells.Add(GetColumnName(col) + label.Text, entry);
                Debug.WriteLine("Entry {0} added at index {1}", entry.Text, grid.Children.Count - 1);
            }
        }
    }

    private string GetColumnName(int colIndex)
    {
        int dividend = colIndex;
        string columnName = string.Empty;
        while (dividend > 0)
        {
            int modulo = (dividend - 1) % 26;
            columnName = Convert.ToChar(65 + modulo) + columnName;
            dividend = (dividend - modulo) / 26;
        }
        return columnName;
    }

    // викликається, коли користувач вийде зі зміненої клітинки(втратить фокус)
    async private void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        var entry = (Entry)sender;
        var row = Grid.GetRow(entry);
        var col = Grid.GetColumn(entry);
        var content = entry.Text;
        try
        {
            entry.Text = Table.Refresh(GetColumnName(col) + row, entry.Text).ToString();
        }
        catch
        {
            if(entry.Text != string.Empty)
                await DisplayAlert("Помилка", "Введено недопустимий вираз", "Ок");
        }
    }

    private void SaveButton_Clicked(object sender, EventArgs e)
    {
    }

    private async void ExitButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Підтвердження", "Ви дійсно хочете вийти ? ", "Так", "Ні");
        if (!answer) return;
        System.Environment.Exit(0);
    }

    private async void HelpButton_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Довідка", "Лабораторна робота 1. Студента Рісенгіна Владислава", "OK");
    }

    private void DeleteColumnButton_Clicked(object sender, EventArgs e)
    {
        if (grid.ColumnDefinitions.Count <= 1) return;

        // TODO: Check if rest of entry's not depent on any of column entry

        int lastColumnIndex = grid.ColumnDefinitions.Count - 1;
        grid.ColumnDefinitions.RemoveAt(lastColumnIndex);
        string columnName = GetColumnName(CountColumn);
        grid.Children.Remove(cells[columnName]); // Remove column label
        
        for (int row = 1; row <= CountRow; row++)
        {
            grid.Children.Remove(cells[columnName+row]);
            // TODO: Delete from cells
        }
        CountColumn--;
    }

    private void DeleteRowButton_Clicked(object sender, EventArgs e)
    {
        if (grid.RowDefinitions.Count <= 1) return;

        // TODO: Check if rest of entry's not depent on any of row entry

        int lastRowIndex = grid.RowDefinitions.Count - 1;
        grid.RowDefinitions.RemoveAt(lastRowIndex); // Remove row label

        grid.Children.Remove(cells[CountRow.ToString()]);
        for (int col = 1; col <= CountColumn; col++)
        {
            grid.Children.Remove(cells[GetColumnName(col) + CountRow]);
            // TODO: Delete from cells
        }
        CountRow--;
    }


    private void AddRowButton_Clicked(object sender, EventArgs e)
    {
        int newRowIndex = grid.RowDefinitions.Count;
        // Add a new row definition
        grid.RowDefinitions.Add(new RowDefinition());
        // Add label for the row number
        var label = NewLabel(newRowIndex.ToString());
        AddLabel(label, newRowIndex, 0);
        cells.Add(label.Text, label);
        Debug.WriteLine("Label {0} added at index {1}", label.Text, grid.Children.Count - 1);
        // Add entry cells for the new row
        for (int col = 1; col <= CountColumn; col++)
        {
            var entry = NewEmptyEntry();
            AddEntry(entry, newRowIndex, col);
            cells.Add(GetColumnName(col) + label.Text, entry);
            Debug.WriteLine("Entry {0} added at index {1}", entry.Text, grid.Children.Count - 1);
        }
        CountRow++;
    }

    private void AddColumnButton_Clicked(object sender, EventArgs e)
    {
        int newColumnIndex = grid.ColumnDefinitions.Count;
        // Add a new column definition
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        // Add label for the column name
        var label = NewLabel(GetColumnName(newColumnIndex));
        AddLabel(label, 0, newColumnIndex);
        cells.Add(label.Text, label);
        Debug.WriteLine("Label {0} added at index {1}", label.Text, grid.Children.Count - 1);
        // Add entry cells for the new column
        for (int row = 1; row <= CountRow; row++)
        {
            var entry = NewEmptyEntry();
            AddEntry(entry, row, newColumnIndex);
            cells.Add(label.Text + row, entry);
            Debug.WriteLine("Entry {0} added at index {1}", entry.Text, grid.Children.Count - 1);
        }
        CountColumn++;
    }

    private static Label NewLabel(string text)
    {
        return new Label {
            Text = text,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };
    }

    private Entry NewEmptyEntry()
    {
        var entry = new Entry
        {
            Text = string.Empty,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };
        entry.Unfocused += Entry_Unfocused;
        entry.Focused += Entry_Focused;
        return entry;
    }

    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        var entry = (Entry) sender;
        var row = Grid.GetRow(entry);
        var col = Grid.GetColumn(entry);
        entry.Text = Table.GetExpression(GetColumnName(col) + row);
    }

    private void AddLabel(Label label, int row, int col)
    {
        Grid.SetRow(label, row);
        Grid.SetColumn(label, col);
        grid.Children.Add(label);
    }

    private void AddEntry(Entry entry, int row, int col)
    {
        Grid.SetRow(entry, row);
        Grid.SetColumn(entry, col);
        grid.Children.Add(entry);

        Table.AddNewEntry(GetColumnName(col) + row);
    }
}
