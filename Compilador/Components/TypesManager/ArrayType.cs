using Antlr4.Runtime;

namespace Compilador.Components.TypesManager;

public class ArrayType : Type
{
    public enum Types
    {
        Int,
        Char,
        Error,
    }

    public readonly string type = "array";
    public int size = 0;
    public Types dataType;

    public ArrayType(IToken t, int n, Types dt, ParserRuleContext c) : base(t, n, c)
    {
        dataType = dt;
    }

    public int Size
    {
        get => size;
        set => size = value;
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

    public override string getType()
    {
        return dataType.ToString();
    }
}