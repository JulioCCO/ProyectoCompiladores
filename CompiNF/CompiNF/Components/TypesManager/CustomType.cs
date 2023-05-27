using System.Collections.Generic;
using Antlr4.Runtime;

namespace CompiNF.Components.TypesManager;

public class CustomType : Type
{
    public readonly string Type = "Custom";
    public readonly string TypeOf;
    public LinkedList<Type> attributeList = new LinkedList<Type>();


    public CustomType(IToken t, int n, string typeOf, ParserRuleContext c) : base(t, n, c)
    {
        TypeOf = typeOf;
    }

    public override string getType()
    {
        return TypeOf;
    }
}