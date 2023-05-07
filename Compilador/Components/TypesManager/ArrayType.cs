using Antlr4.Runtime;

namespace Compilador.Components.TypesManager;

public class ArrayType: Type
{
    public readonly string type = "array";
    
    public enum Types
    {
        Int,
        Char,
        Error,

    }
    public Types dataType;
    
    public ArrayType(IToken t, int n, Types dt) : base(t, n)
    {
        dataType = dt;
    }
    
    public static Types showType(string type)
    {
        return type switch
        {
            "int" => Types.Int,
            "char" => Types.Char,
            _ => Types.Error,
        };
    }
    
}