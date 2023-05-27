using System.Collections.Generic;

namespace Compilador.TypesStructure;

public class ErrorBuilder
{
    public List<string> ErrorList { get; set; }

    public ErrorBuilder()
    {
        ErrorList = new List<string>();
    }
    
    public void AddError(string message)
    {
        ErrorList.Add( message );
    }
    
    public string BuildMessage()
    {
        string message = "";
        foreach (var error in ErrorList)
        {
            message += error + "\n";
        }

        return message;
    }
    
}