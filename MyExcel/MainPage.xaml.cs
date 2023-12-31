﻿using CommunityToolkit.Maui.Storage;
using Mopups.Interfaces;

namespace MyExcel;

public partial class MainPage : ContentPage
{
    IFileSaver fileSaver;
    IPopupNavigation popupNavigation;

    static int CountColumn = 5;
    static int CountRow = 10;

    GDriveManager driveManager;

    readonly static Dictionary<string, IView> Cells = new(capacity: (int)(CountColumn * CountRow * 1.6));

    public MainPage(IFileSaver fileSaver, IPopupNavigation popupNavigation)
    {
        InitializeComponent();

        this.fileSaver = fileSaver;
        this.popupNavigation = popupNavigation;
        CreateGrid();
    }

    private void CreateGrid()
    {
        AddColumnsAndColumnLabels();
        AddRowsAndCellEntries();
    }

    private void AddColumnsAndColumnLabels()
    {
        grid.RowDefinitions.Add(new RowDefinition()); // Add Row for labels

        for (int col = 0; col <= CountColumn; col++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            var label = NewLabel(GetColumnName(col));
            AddLabel(label, 0, col);
        }
    }

    private void AddRowsAndCellEntries()
    {
        for (int row = 1; row <= CountRow; row++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            var label = NewLabel(row.ToString());
            AddLabel(label, row, 0);
            for (int col = 1; col <= CountColumn; col++)
            {
                var entry = NewEmptyEntry();
                var CellName = GetColumnName(col) + label.Text;
                AddEntry(entry, CellName, row, col);
            }
        }
    }

    private static string GetColumnName(int colIndex)
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

    async private void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        var entry = (Entry) sender;
        var row = Grid.GetRow(entry);
        var col = Grid.GetColumn(entry);
        var CellName = GetColumnName(col) + row;
        try
        {
            Table.Refresh(CellName, entry.Text).ToString();
            Refresh(CellName);
        } catch(ArgumentException exp) {
            Refresh(CellName);
            await DisplayAlert("Помилка", "Введено недопустимий вираз", "Ок");
        }
        catch { }
    }

    public static void Refresh(string CellName)
    {
        var entry = (Entry)Cells[CellName];
        entry.Text = Table.GetValue(CellName).ToString();
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        await SaveToFile();
    }

    private async Task SaveToFile(){
        try
        {
            var jsonManager = new JsonFileManager(fileSaver);
            var filePath = await jsonManager.SaveToFileAsync(new FileRepresentation(CountRow, CountColumn, Table.cells), "Table.json");
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                await DisplayAlert("Збереження", "Таблицю успішно збережено за шляхом: " + filePath, "Ок");
            }
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
        }
       
    }

    private async void OpenButton_Clicked(object sender, EventArgs e){
        var jsonManager = new JsonFileManager(fileSaver);
        var fileRepresentation = await jsonManager.LoadFromFileAsync();
        OpenNewTable(fileRepresentation);
    } 

    private async void OpenNewTable(FileRepresentation fileRepresentation){
        var NewCountColumn = fileRepresentation.CountColumn;
        var NewCountRow = fileRepresentation.CountRow;
                // clear Cells and Table.cells
        Cells.Clear();
        Table.cells.Clear();

        // remove grid column/row definitions
        grid.ColumnDefinitions.Clear();
        grid.RowDefinitions.Clear();

        // remove grid Children
        grid.Children.Clear();

        // update CountRow and Count Column
        CountColumn = NewCountColumn;
        CountRow = NewCountRow;

        // recreate grid
        CreateGrid();

        foreach(var pair in fileRepresentation.cells)
        {
            //update gui
            string key = pair.Key;
            var entry = (Entry)Cells[key];
            Cell cell = pair.Value;
            if (cell.Expression.Equals(string.Empty)) entry.Text = string.Empty;
            else entry.Text = cell.Value.ToString();

            //update Table
            Table.cells[key] = cell;
        }

        await DisplayAlert("Новий файл", "Таблиця успішно відкрита!", "Ок");
    }

    private async void ExitButton_Clicked(object sender, EventArgs e)
    {
        bool saveChanges = await DisplayAlert("Збереження", "Зберегти таблицю ?", "Так", "Ні");
        if(!saveChanges) return;

        await SaveToFile();
        
        System.Environment.Exit(0);
    }

    private async void HelpButton_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Довідка", "Лабораторна робота 1*. Студента Рісенгіна Владислава", "OK");
    }

    private async void DeleteColumnButton_Clicked(object sender, EventArgs e)
    {
        if (grid.ColumnDefinitions.Count <= 2) return;
        string columnName = GetColumnName(CountColumn);

        for (int row = 1; row <= CountRow; row++)
        {
            var CellName = columnName + row;
            if (Table.cells[CellName].AppearsIn.Count != 0)
            {
                string firstCellName = Table.cells[CellName].AppearsIn.First();
                await DisplayAlert("Поимлка", "Значення клітини " + CellName + " використовується в кліинці " + firstCellName + "! Видалення неможливе.", "Ок");
                return;
            }
        }

        int lastColumnIndex = grid.ColumnDefinitions.Count - 1;
        grid.ColumnDefinitions.RemoveAt(lastColumnIndex);
        
        grid.Children.Remove(Cells[columnName]);
        Cells.Remove(columnName);

        for (int row = 1; row <= CountRow; row++)
        {
            var CellName = columnName + row;
            grid.Children.Remove(Cells[CellName]);
            Cells.Remove(CellName);
            Table.RemoveEntry(CellName);
        }
        CountColumn--;
    }

    private async void DeleteRowButton_Clicked(object sender, EventArgs e)
    {
        if (grid.RowDefinitions.Count <= 2) return;
        
        for (int col = 1; col <= CountColumn; col++)
        {
            var CellName = GetColumnName(col) + CountRow;
            if (Table.cells[CellName].AppearsIn.Count != 0)
            {
                await DisplayAlert("Поимлка", "Значення клітини " + CellName + " використовується! Видалення неможливе.", "Ок");
                return;
            }
        }

        int lastRowIndex = grid.RowDefinitions.Count - 1;
        grid.RowDefinitions.RemoveAt(lastRowIndex);
        
        grid.Children.Remove(Cells[CountRow.ToString()]);
        Cells.Remove(CountRow.ToString());
        
        for (int col = 1; col <= CountColumn; col++)
        {
            var CellName = GetColumnName(col) + CountRow;
            grid.Children.Remove(Cells[CellName]);
            Cells.Remove(CellName);
            Table.RemoveEntry(CellName);
        }
        CountRow--;
    }


    private void AddRowButton_Clicked(object sender, EventArgs e)
    {
        int newRowIndex = grid.RowDefinitions.Count;
        grid.RowDefinitions.Add(new RowDefinition());
        var label = NewLabel(newRowIndex.ToString());
        AddLabel(label, newRowIndex, 0);
        for (int col = 1; col <= CountColumn; col++)
        {
            var entry = NewEmptyEntry();
            var CellName = GetColumnName(col) + label.Text;
            AddEntry(entry, CellName, newRowIndex, col);
        }
        CountRow++;
    }

    private void AddColumnButton_Clicked(object sender, EventArgs e)
    {
        int newColumnIndex = grid.ColumnDefinitions.Count;
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        var label = NewLabel(GetColumnName(newColumnIndex));
        AddLabel(label, 0, newColumnIndex);
        for (int row = 1; row <= CountRow; row++)
        {
            var entry = NewEmptyEntry();
            var CellName = label.Text + row;
            AddEntry(entry, CellName, row, newColumnIndex);
        }
        CountColumn++;
    }

    private static Label NewLabel(string text)
    {
        return new Label
        {
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
            HorizontalOptions = LayoutOptions.Center,
            BackgroundColor = Color.FromHex("676767")
        };
        entry.Unfocused += Entry_Unfocused;
        entry.Focused += Entry_Focused;
        return entry;
    }

    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        var entry = (Entry)sender;
        var row = Grid.GetRow(entry);
        var col = Grid.GetColumn(entry);
        entry.Text = Table.GetExpression(GetColumnName(col) + row);
    }

    private void AddLabel(Label label, int row, int col)
    {
        Grid.SetRow(label, row);
        Grid.SetColumn(label, col);
        grid.Children.Add(label);
        Cells.Add(label.Text, label);
    }

    private void AddEntry(Entry entry, string CellName, int row, int col)
    {
        Grid.SetRow(entry, row);
        Grid.SetColumn(entry, col);
        grid.Children.Add(entry);

        Table.AddNewEntry(GetColumnName(col) + row);
        Cells.Add(CellName, entry);
    }

    private async void SaveOnGDriveButton_Clicked(object sender, EventArgs e)
    {
        var popup = new GDriveUploadPopup();
        await popupNavigation.PushAsync(popup);

        var fileName = await popup.PopupDismissedTask;
        
        if (string.IsNullOrEmpty(fileName)) return;

        if(driveManager == null)
        {
            driveManager = await GDriveManager.Create();
        }
        
        var result = await driveManager.SaveToFileAsync(new FileRepresentation(CountRow, CountColumn, Table.cells), fileName);
        await DisplayAlert("GDrive Upload", result, "Ок");
    }

    private async void OpenFromGDriveButton_Clicked(object sender, EventArgs e)
    {
        var popup = new GDriveSavePopup();
        await popupNavigation.PushAsync(popup);
        //if (driveManager == null)
        //{
        //    driveManager = await GDriveManager.Create();
        //}

        //var list = driveManager.ListFilesAsync().Result;

    }
}
