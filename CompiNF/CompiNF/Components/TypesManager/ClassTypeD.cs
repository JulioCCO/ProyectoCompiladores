using System.Collections.Generic;
using Antlr4.Runtime;

namespace CompiNF.Components.TypesManager;

public class ClassTypeD : TypeD
{
    public readonly string type = "class";
    public LinkedList<TypeD> attributes = new LinkedList<TypeD>();

    public ClassTypeD(IToken t, int n, ParserRuleContext c) : base(t, n, c)
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