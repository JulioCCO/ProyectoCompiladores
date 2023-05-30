using System;
using Antlr4.Runtime;

namespace CompiNF.Components.TypesManager;

public class BasicTypeD : TypeD
{
    public enum Types
    {
        Int,
        Double,
        String,
        Boolean,
        Char,
        Error,
    }

    public Types type;

    public BasicTypeD(IToken t, Types bs, int n, ParserRuleContext c) : base(t, n, c)
    {
        type = bs;
    }

    public static Types showType(string type)
    {
        return type switch
        {
            "int" => Types.Int,
            "double" => Types.Double,
            "string" => Types.String,
            "boolean" => Types.Boolean,
            "char" => Types.Char,
            _ => Types.Error,
        };
    }

    public static Boolean isBasicType(string type)
    {
        return type switch
        {
            "int" => true,
            "double" => true,
            "string" => true,
            "boolean" => true,
            "char" => true,
            _ => false,
        };
    }


    public override string getType()
    {
        return type.ToString();
    }
}