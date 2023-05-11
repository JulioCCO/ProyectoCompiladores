using System.Collections.Generic;
using Antlr4.Runtime;

namespace Compilador.Components.TypesManager;

public class ClassType: Type
{
    public readonly string type = "class";
    public LinkedList<Type> attributes = new LinkedList<Type>();

    public ClassType(IToken t, int n): base(t, n){}
    public override string getType()
    {
        return type;
    }
}