using System.Collections.Generic;
using System.IO;
using System.Text;
using Antlr4.Runtime;

namespace Compilador.Components;

public class MyErrorListener : IAntlrErrorListener<int>
{
    public LinkedList<string>  ErrorMsgs ;
    
    
    public MyErrorListener()
    {
        ErrorMsgs = new LinkedList<string>();
    }

    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine,
        string msg, RecognitionException e)
    {
        if (recognizer.GetType() == typeof(AlphaScanner))
            ErrorMsgs.AddFirst($"SCANNER ERROR - line {line}:{charPositionInLine} {msg}");
        
        else
            ErrorMsgs.AddFirst("Other Error");
    }
        
    public bool HasErrors()
    {
        return this.ErrorMsgs.Count > 0;
    }

    public override string ToString()
    {
        if (!HasErrors()) return "0 errors";
        StringBuilder builder = new StringBuilder();
        foreach (string s in ErrorMsgs)
        {
            //System.Diagnostics.Debug.WriteLine("\nError scanner :  "+$"{s}" + " \n" );
            builder.Append(s + " \n");
        }
        return builder.ToString();
    }
}