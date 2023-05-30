using System.Collections.Generic;
using Antlr4.Runtime;

namespace CompiNF.Components.TypesManager;

public class CustomTypeD : TypeD
{
    public readonly string Type = "Custom";
    public readonly string TypeOf;
    public LinkedList<TypeD> attributeList = new LinkedList<TypeD>();


    public CustomTypeD(IToken t, int n, string typeOf, ParserRuleContext c) : base(t, n, c)
    {
        TypeOf = typeOf;
    }

    public override string getType()
    {
        return TypeOf;
    }
}