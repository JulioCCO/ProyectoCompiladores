using Antlr4.Runtime;

namespace Compilador.Components.TypesManager;

public abstract class Type
{
    public int nivel;
    public IToken token;

    protected Type(IToken token, int nivel)
    {
        this.nivel = nivel;
        this.token = token;
    }

    public abstract string getType();
}