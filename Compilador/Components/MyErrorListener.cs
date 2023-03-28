﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Compilador.Components;

public class MyErrorListener : IAntlrErrorListener<int>
{
    public ArrayList<string>  ErrorMsgs = new ArrayList<string>();
    
    
    public MyErrorListener()
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
        if (recognizer is AlphaScanner)
            ErrorMsgs.Add($"SCANNER ERROR - line {line}:{charPositionInLine} {msg}");
        
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