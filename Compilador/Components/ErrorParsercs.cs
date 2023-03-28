using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Compilador.Components;

public class ErrorParser : BaseErrorListener
{
    public ArrayList<string>  ErrorMsgs = new ArrayList<string>();
    
    
    public ErrorParser()
    {
        this.ErrorMsgs = new ArrayList<string>();
    }
    
    public bool HasErrors()
    {
        return this.ErrorMsgs.Count > 0;
    }

    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine,
        string msg, RecognitionException e)
    {
        if (recognizer is AlphaParser)
            ErrorMsgs.Add($"PARSER ERROR - line {line}:{charPositionInLine} {msg}");
        
        else
            ErrorMsgs.Add("Other Error");
    }

    public override string ToString()
    {
        if (!HasErrors()) return "0 errors";
        StringBuilder builder = new StringBuilder();
        foreach (string s in ErrorMsgs)
        {
            //System.Diagnostics.Debug.WriteLine("\nError:  "+$"{s}" + " \n" );
            builder.AppendLine(" \n" + s);
        }
        return builder.ToString();
    }
}