namespace MyExcel
{
	public static class Table
	{
        public static Dictionary<string, Cell> cells = new Dictionary<string, Cell>();

		public static void AddNewEntry(string CellName)
		{
			cells.Add(CellName, new Cell());
		}

		public static void RemoveEntry(string CellName)
		{
			Cell cell;
			if (!cells.TryGetValue(CellName, out cell)) return;

			if (cell.AppearsIn.Count == 0) cells.Remove(CellName);
		}

		public static double GetValue(string CellName)
		{
			Cell cell;
			return (cells.TryGetValue(CellName, out cell)) ? cell.Value : 0;
		}

		public static string GetExpression(string CellName)
		{
            Cell cell;
            return (cells.TryGetValue(CellName, out cell)) ? cell.Expression : string.Empty;
        }

		public static double Refresh(string CellName, string expression)
		{
			Calculator.EvaluatingCellName = CellName;

			if (Calculator.HasIdentifier(expression, CellName))
				throw new ArgumentException();

			var listOfIdentifiers = Calculator.ListOfIdentifiers(expression);
			foreach(var identifier in listOfIdentifiers)
			{
				if(HasCyclicDependency(identifier))
                    throw new ArgumentException();
            }

			return Calculate(CellName, expression);
		}

		public static double Calculate(string CellName, string expression)
		{
            Calculator.EvaluatingCellName = CellName;
            double value = Calculator.Evaluate(expression);
            cells[CellName].Expression = expression;
            cells[CellName].Value = value;
            var OutdatedCells = Table.cells[Calculator.EvaluatingCellName].AppearsIn.ToArray();
            foreach (var OutdatedCellName in OutdatedCells)
            {
                Calculate(OutdatedCellName, Table.GetExpression(OutdatedCellName));
                MainPage.Refresh(OutdatedCellName);
            }

            return value;
        }

		private static bool HasCyclicDependency(string CellName)
		{
			// глянути чи в cellname нема depends on evaluating cell a також у всіх від яких вона залежить
			Cell cell = Table.cells[CellName];

			if (cell.DependsOn.Contains(Calculator.EvaluatingCellName)) return true;

			foreach(var c in cell.DependsOn)
			{
				return HasCyclicDependency(c);
			}
			return false;
		}

	}
}

