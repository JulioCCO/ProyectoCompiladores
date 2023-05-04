using Antlr4.Runtime;

namespace Compilador.Components.TypesManager;

public class CustomType:Type
{
    public readonly string Type = "Custom";
    public readonly string TypeOf;

    public CustomType(IToken token, int level, string typeOf) : base(token, level)
    {
        TypeOf = typeOf;
    }
}