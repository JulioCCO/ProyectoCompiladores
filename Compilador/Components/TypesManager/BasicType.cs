﻿using System;
using Antlr4.Runtime;

namespace Compilador.Components.TypesManager;

public class BasicType: Type
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

    public BasicType(IToken t ,Types bs, int n): base(t, n)
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
}