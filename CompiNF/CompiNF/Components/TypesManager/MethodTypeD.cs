using System.Collections.Generic;
using Antlr4.Runtime;
using generated;

namespace CompiNF.Components.TypesManager;

public class MethodTypeD : TypeD
{
    public readonly int cantParams;
    public readonly string type = "method";
    public readonly string returnType;
    public string paramsNames = "";
    public AlphaParser.ReturnStatementASTContext? returnStatement = null;

    public LinkedList<TypeD> paramsTypes;

    public MethodTypeD(IToken t, int n, int cantParams, string r, LinkedList<TypeD> paramsList, ParserRuleContext c) : base(t, n, c)
    {
        this.cantParams = cantParams;
        this.returnType = r;
        this.paramsTypes = paramsList;
    }

    public string imprimirParams()
    {
        paramsNames =("----- INICIO PARAMETROS ------\n");
        System.Diagnostics.Debug.WriteLine("----- INICIO PARAMETROS ------");
        foreach (var child in paramsTypes)
        {
            if (child is BasicTypeD basicType)
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + basicType.token.Text
                                                              + " Nivel: " + basicType.nivel
                                                              + " Tipo: " + basicType.type);
                paramsNames += ("Nombre: " + basicType.token.Text
                                + " Nivel: " + basicType.nivel
                                + " Tipo: " + basicType.type + "\n");
            }
            else if (child is CustomTypeD customType)
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + customType.token.Text
                                                              + " Nivel: " + customType.nivel
                                                              + " Tipo: " + customType.TypeOf
                                                              + " Tipo de estructura:" + customType.Type);
                paramsNames += ("Nombre: " + customType.token.Text
                                + " Nivel: " + customType.nivel
                                + " Tipo: " + customType.TypeOf
                                + " Tipo de estructura:" + customType.Type + "\n");
               
            }
            else if (child is ArrayTypeD arrayType)
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + arrayType.token.Text
                                                              + " Nivel: " + arrayType.nivel
                                                              + " Tipo: " + arrayType.type
                                                              + " Tipo de estructura:" + arrayType.dataType);
                
                paramsNames += ("Nombre: " + arrayType.token.Text
                                + " Nivel: " + arrayType.nivel
                                + " Tipo: " + arrayType.type
                                + " Tipo de estructura:" + arrayType.dataType);
                
            }
        }

        System.Diagnostics.Debug.WriteLine("----- FIN PARAMETROS ------");
        paramsNames += ("----- FIN PARAMETROS ------\n");
        return paramsNames;
    }

    public override string getType()
    {
        return type;
    }
}