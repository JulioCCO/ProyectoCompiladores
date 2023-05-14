using System.Collections.Generic;
using Antlr4.Runtime;

namespace Compilador.Components.TypesManager;

public class CustomType : Type
{
    public readonly string Type = "Custom";
    public readonly string TypeOf;
    public LinkedList<Type> attributeList = new LinkedList<Type>();


    public CustomType(IToken token, int level, string typeOf) : base(token, level)
    {
        TypeOf = typeOf;
    }

    public override string getType()
    {
        return TypeOf;
    }
}