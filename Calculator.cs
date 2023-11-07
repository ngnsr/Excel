using Antlr4.Runtime;

namespace MyExcel
{
    public static class Calculator
    {
        public static string EvaluatingCellName;

        public static double Evaluate(string expression)
        {
            
            // TODO: розібратися чому зʼявляюсть ексепшини після зміни одніїє з залежних кл
            foreach (var OutdatedCellName in Table.cells[Calculator.EvaluatingCellName].DependsOn)
            {
                Table.cells[OutdatedCellName].AppearsIn.Remove(Calculator.EvaluatingCellName);
            }
            Table.cells[Calculator.EvaluatingCellName].DependsOn.Clear();

            if (expression.Equals(string.Empty)) return 0;

            var lexer = new GrammarLexer(new AntlrInputStream(expression));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new LexerErrorListener());

            var tokens = new CommonTokenStream(lexer);

            var parser = new GrammarParser(tokens);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new ParserErrorListener());

            var tree = parser.compileUnit();

            var visitor = new GrammarVisitor();
            return visitor.Visit(tree);
        }
    }

}

