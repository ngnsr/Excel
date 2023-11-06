using Antlr4.Runtime;

namespace MyExcel
{
    public static class Calculator
    {
        static Table table = new Table();

        public static double Evaluate(string expression)
        {

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

