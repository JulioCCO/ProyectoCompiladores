using System.Collections.Generic;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using generated;

namespace CompiNF.Components;

public class MyErrorParser : BaseErrorListener
{
    public LinkedList<string> ErrorMsgs;

    public MyErrorParser()
    {
        ErrorMsgs = new LinkedList<string>();
    }

    public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line,
        int charPositionInLine,
        string msg, RecognitionException e)
    {
        if (recognizer.GetType() == typeof(AlphaParser))
            ErrorMsgs.AddFirst($"PARSER ERROR - line {line}:{charPositionInLine} {msg}");

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
            //System.Diagnostics.Debug.WriteLine("\nError parser:  "+$"{s}" + " \n" );
            builder.Append(s + " \n");
        }

        return builder.ToString();
    }
}