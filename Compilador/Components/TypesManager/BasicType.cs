using Antlr4.Runtime;

namespace Compilador.Components.TypesManager;

public class BasicType: Type
{
    public enum Types
    {
        Int,
        Double,
        String,
        Boolean,
        Char,
        Null,
        Error,
    }

    private Types type;

    public BasicType(IToken t ,Types bs, int n): base(t, n)
    {
        type = bs;
    }
}