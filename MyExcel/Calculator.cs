using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Antlr4.Runtime;

namespace MyExcel
{
    public static class Calculator
    {
        public static string EvaluatingCellName;

        public static double Evaluate(string expression)
        {
            if (!string.IsNullOrEmpty(EvaluatingCellName))
            {
                foreach (var OutdatedCellName in Table.cells[Calculator.EvaluatingCellName].DependsOn)
                {
                    Table.cells[OutdatedCellName].AppearsIn.Remove(Calculator.EvaluatingCellName);
                }
                Table.cells[Calculator.EvaluatingCellName].DependsOn.Clear();
            }

            if (expression.Equals(string.Empty)) return 0;

            if (Regex.Match(expression, @"^([+-]?\d+(\.\d+)?)$").Success)
            {
                return Double.Parse(expression);
            }

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

        public static bool HasIdentifier(string expression, string identifier)
        {
            string identifierRegex = @"[a-zA-Z]+[1-9][0-9]*";
            foreach (Match match in Regex.Matches(expression, identifierRegex).Cast<Match>())
            {
                if (match.Value.Equals(identifier)) return true;
            }
            return false;
        }

        public static List<string> ListOfIdentifiers(string expression)
        {
            string identifierRegex = @"[a-zA-Z]+[1-9][0-9]*";
            return Regex.Matches(expression, identifierRegex).Cast<Match>().Select(match => match.Value).ToList();
        }
    }

}

