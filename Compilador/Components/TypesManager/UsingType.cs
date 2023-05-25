using Antlr4.Runtime;

namespace Compilador.Components.TypesManager;

public class UsingType: Type
{
    public readonly string type = "using";
    public UsingType(IToken t,  int n, ParserRuleContext c) : base(t, n, c){}

    public override string getType()
    {
        return token.Text;
    }
}