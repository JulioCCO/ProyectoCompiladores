using Antlr4.Runtime;

namespace CompiNF.Components.TypesManager;

public abstract class TypeD
{
    public int nivel;
    public IToken token;
    public ParserRuleContext context;

    protected TypeD(IToken token, int nivel, ParserRuleContext ctx)
    {
        this.nivel = nivel;
        this.token = token;
        this.context = ctx;
    }

    public abstract string getType();
}