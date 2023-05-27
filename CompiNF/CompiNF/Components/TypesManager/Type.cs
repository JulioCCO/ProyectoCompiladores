using Antlr4.Runtime;

namespace CompiNF.Components.TypesManager;

public abstract class Type
{
    public int nivel;
    public IToken token;
    public ParserRuleContext context;

    protected Type(IToken token, int nivel, ParserRuleContext ctx)
    {
        this.nivel = nivel;
        this.token = token;
        this.context = ctx;
    }

    public abstract string getType();
}