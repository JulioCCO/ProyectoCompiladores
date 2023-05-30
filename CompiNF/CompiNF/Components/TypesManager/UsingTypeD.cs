using Antlr4.Runtime;

namespace CompiNF.Components.TypesManager;

public class UsingTypeD: TypeD
{
    public readonly string type = "using";
    public UsingTypeD(IToken t,  int n, ParserRuleContext c) : base(t, n, c){}

    public override string getType()
    {
        return token.Text;
    }
}