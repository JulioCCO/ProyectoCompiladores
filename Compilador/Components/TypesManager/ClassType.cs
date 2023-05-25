using System.Collections.Generic;
using Antlr4.Runtime;

namespace Compilador.Components.TypesManager;

public class ClassType : Type
{
    public readonly string type = "class";
    public LinkedList<Type> attributes = new LinkedList<Type>();

    public ClassType(IToken t, int n, ParserRuleContext c) : base(t, n, c)
    {
    }

    public bool BuscarAtributo(string name)
    {
        foreach (var attribute in attributes)
        {
            if (attribute.token.Text.Equals(name))
            {
                return true;
            }
        }
        return false;
    }
    public override string getType()
    {
        return token.Text;
    }
}