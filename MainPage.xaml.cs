using System.Diagnostics;

namespace MyExcel;

public partial class MainPage : ContentPage
{
    int NumberOfCols = 5; // кількість стовпчиків (A to Z)
    int NumberOfRows = 10; // кількість рядків

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
        for (int col = 0; col <= NumberOfCols; col++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            var label = new Label
            {
                Text = GetColumnName(col),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(label, 0);
            Grid.SetColumn(label, col);
            grid.Children.Add(label);
            Debug.WriteLine("Label {0} added at index {1}", label.Text, grid.Children.Count - 1);
        }
    }

    private void AddRowsAndCellEntries()
    {
        // Додати рядки, підписи для рядків та комірки
        for (int row = 0; row < NumberOfRows; row++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            // Додати підпис для номера рядка
            var label = new Label
            {
                Text = (row + 1).ToString(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(label, row + 1);
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);
            Debug.WriteLine("Label {0} added at index {1}", label.Text, grid.Children.Count - 1);
            // Додати комірки (Entry) для вмісту
            for (int col = 0; col < NumberOfCols; col++)
            {
                var entry = new Entry
                {
                    Text = grid.Children.Count.ToString(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                entry.Unfocused += Entry_Unfocused; // обробник події Unfocused
                Grid.SetRow(entry, row + 1);
                Grid.SetColumn(entry, col + 1);
                grid.Children.Add(entry);
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
        var row = Grid.GetRow(entry) - 1;
        var col = Grid.GetColumn(entry) - 1;
        var content = entry.Text;
        try
        {
            entry.Text = Calculator.Evaluate(content).ToString();
        }
        catch
        {
            await DisplayAlert("Помилка", "Введено недопустимий вираз", "Ок");
        }
    }

    private void CalculateButton_Clicked(object sender, EventArgs e)
    {
        //// Обробка кнопки "Порахувати"
        //// Use as test
        //int numOfCols = DefaultNumberOfCols + ColsAdded;
        //int lastInd = numOfCols + numOfCols * (DefaultNumberOfRows + RowsAdded);
        //Debug.WriteLine("lastInd = {0}", lastInd);
        //grid.ColumnDefinitions.RemoveAt(grid.ColumnDefinitions.Count - 1);

        //for (int i = lastInd; i < lastInd + DefaultNumberOfRows + RowsAdded; i++)
        //{
        //    int count = grid.Children.Count - 1;
        //    grid.Children.RemoveAt(lastInd);
        //    Debug.WriteLine("RemoveAt({0})", i);
        //    if(grid.Children.Count != count)
        //    {
        //        Debug.WriteLine("smt");
        //    }
        //}
        //ColsAdded--;
    }

    private void SaveButton_Clicked(object sender, EventArgs e)
    {
        int n = Int32.Parse(textInput.Text);
        grid.Children.RemoveAt(n);
        Debug.WriteLine("RemovaAt({0})", n);
    }

    private void ReadButton_Clicked(object sender, EventArgs e)
    {
        // Обробка кнопки "Прочитати"
    }

    private async void ExitButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Підтвердження", "Ви дійсно хочете вийти ? ", "Так", "Ні");
        if (answer)
        {
            System.Environment.Exit(0);
        }
    }

    private async void HelpButton_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Довідка", "Лабораторна робота 1. Студента Рісенгіна Владислава", "OK");
    }

    private void DeleteColumnButton_Clicked(object sender, EventArgs e)
    {
        if (grid.ColumnDefinitions.Count > 1)
        {
            int lastColumnIndex = grid.ColumnDefinitions.Count - 1;

            grid.ColumnDefinitions.RemoveAt(lastColumnIndex);
            //grid.Children.RemoveAt(lastColumnIndex); // Remove label
            
            for (int row = 0; row < NumberOfRows + 1; row++)
            {
                int removeIndex = row * (numberOfCols + 1) + lastColumnIndex - row;
                // Remove entry
                grid.Children.RemoveAt(removeIndex);
                Debug.WriteLine("RemoveAt({0})", removeIndex);
            }
            ColsAdded--;
        }
    }

    private void DeleteRowButton_Clicked(object sender, EventArgs e)
    {
        if (RowsAdded <= 0)
        {
            int lastRowIndex = grid.RowDefinitions.Count - 1;
            grid.RowDefinitions.RemoveAt(lastRowIndex);
            //grid.Children.RemoveAt(lastRowIndex * (DefaultNumberOfCols + 1)); // Remove label
            for (int col = 0; col < DefaultNumberOfCols; col++)
            {
                grid.Children.RemoveAt((col * DefaultNumberOfRows) + lastRowIndex); // Remove entry
            }
            RowsAdded--;
        }
    }


    private void AddRowButton_Clicked(object sender, EventArgs e)
    {
        int newRow = grid.RowDefinitions.Count;
        // Add a new row definition
        grid.RowDefinitions.Add(new RowDefinition());
        // Add label for the row number
        var label = new Label
        {
            Text = newRow.ToString(),
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };
        Grid.SetRow(label, newRow);
        Grid.SetColumn(label, 0);
        grid.Children.Add(label);
        Debug.WriteLine("Label {0} added at index {1}", label.Text, grid.Children.Count - 1);
        int numOfCols = DefaultNumberOfCols + ColsAdded;
        // Add entry cells for the new row
        for (int col = 0; col < numOfCols; col++)
        {
            var entry = new Entry
            {
                Text = grid.Children.Count.ToString(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            entry.Unfocused += Entry_Unfocused;
            Grid.SetRow(entry, newRow);
            Grid.SetColumn(entry, col + 1);
            grid.Children.Add(entry);
            Debug.WriteLine("Entry {0} added at index {1}", entry.Text, grid.Children.Count - 1);
        }
        RowsAdded++;
    }

    private void AddColumnButton_Clicked(object sender, EventArgs e)
    {
        int newColumn = grid.ColumnDefinitions.Count;
        // Add a new column definition
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        // Add label for the column name
        var label = new Label
        {
            Text = GetColumnName(newColumn),
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };
        Grid.SetRow(label, 0);
        Grid.SetColumn(label, newColumn);
        grid.Children.Add(label);
        Debug.WriteLine("Label {0} added at index {1}", label.Text, grid.Children.Count - 1);
        int numOfRows = DefaultNumberOfRows + RowsAdded;
        // Add entry cells for the new column
        for (int row = 0; row < numOfRows; row++)
        {
            var entry = new Entry
            {
                Text = grid.Children.Count.ToString(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            entry.Unfocused += Entry_Unfocused;
            Grid.SetRow(entry, row + 1);
            Grid.SetColumn(entry, newColumn);
            grid.Children.Add(entry);
            Debug.WriteLine("Entry {0} added at index {1}", entry.Text, grid.Children.Count - 1);
        }
        ColsAdded++;
    }
}
