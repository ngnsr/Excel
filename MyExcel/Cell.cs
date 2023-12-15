namespace MyExcel
{
    [Serializable]
    public class Cell
	{
		public string Expression { get; set; }
		public double Value { get; set; }
		
		public HashSet<string> DependsOn { get; set; }
		public HashSet<string> AppearsIn { get; set; }

		public Cell()
		{
			DependsOn = new HashSet<string>();
			AppearsIn = new HashSet<string>();
			Expression = string.Empty;
			Value = 0;
		}

		public Cell(double Value)
		{
            DependsOn = new HashSet<string>();
            AppearsIn = new HashSet<string>();
            Expression = string.Empty;
            this.Value = Value;
        }
	}
}

