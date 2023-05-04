using Antlr4.Runtime;

namespace Compilador.Components.TypesManager;

public class ClassType: Type
{
    public readonly string type = "class";

    public ClassType(IToken t, int n): base(t, n){}
}