namespace MyExcel
{
    public struct FileRepresentation
    {
        public int CountRow {get; set;}
        public int CountColumn {get; set;}

        public Dictionary<string, Cell> cells {get; set;}

        public FileRepresentation(int CountRow, int CountColumn, Dictionary<string, Cell> cells)
        {
            this.CountRow = CountRow;
            this.CountColumn = CountColumn;
            this.cells = cells;
        }
    }
}

