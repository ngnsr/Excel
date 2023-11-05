using System;
using Antlr4.Runtime;

namespace MyExcel
{
    public class LexerErrorListener : IAntlrErrorListener<int>
    {

        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            string sourceName = recognizer.InputStream.SourceName;
            Console.WriteLine("line:{0} col:{1} src:{2} msg:{3}", line, charPositionInLine, sourceName, msg);
            Console.WriteLine("--------------------");
            Console.WriteLine(e);
            Console.WriteLine("--------------------");
            throw new ArgumentException("lexer: invalid");
        }
    }

}

