using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;

namespace Compilador.Components;

public class MyErrorListener : BaseErrorListener
{
    public LinkedList<string> ErrorMsgs;
    
    public MyErrorListener()
    {
        this.ErrorMsgs = new LinkedList<string>();
    }
    
    public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine,
        string msg, RecognitionException e)
    {
        if (recognizer is AlphaParser)
            ErrorMsgs.AddFirst($"PARSER ERROR - line {line}:{charPositionInLine} {msg}");
        else if (recognizer is AlphaScanner)
            ErrorMsgs.AddFirst($"SCANNER ERROR - line {line}:{charPositionInLine} {msg}");
        else
            ErrorMsgs.AddFirst("Other Error");
    }

    public bool HasErrors => ErrorMsgs.Count > 0;

    public override string ToString()
    {
        if (!HasErrors) return "0 errors";
        var builder = new System.Text.StringBuilder();
        foreach (var s in ErrorMsgs)
        {
            System.Diagnostics.Debug.WriteLine("\nError:  "+$"{s}" + " \n" );
            builder.AppendLine($"{s}");
        }
        return builder.ToString();
    }
}