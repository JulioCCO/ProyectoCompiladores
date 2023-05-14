using System.Collections.Generic;
using Antlr4.Runtime;

namespace Compilador.Components.TypesManager;

public class MethodType : Type
{
    public readonly int cantParams;
    public readonly string type = "method";
    public readonly string returnType;

    public LinkedList<Type> paramsTypes;

    public MethodType(IToken t, int n, int cantParams, string r, LinkedList<Type> paramsList) : base(t, n)
    {
        this.cantParams = cantParams;
        this.returnType = r;
        this.paramsTypes = paramsList;
    }

    public void imprimirParams()
    {
        System.Diagnostics.Debug.WriteLine("----- INICIO PARAMETROS ------");
        foreach (var child in paramsTypes)
        {
            if (child is BasicType basicType)
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + basicType.token.Text
                                                              + " Nivel: " + basicType.nivel
                                                              + " Tipo: " + basicType.type);
            }
            else if (child is CustomType customType)
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + customType.token.Text
                                                              + " Nivel: " + customType.nivel
                                                              + " Tipo: " + customType.TypeOf
                                                              + " Tipo de estructura:" + customType.Type);
            }
            else if (child is ArrayType arrayType)
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + arrayType.token.Text
                                                              + " Nivel: " + arrayType.nivel
                                                              + " Tipo: " + arrayType.type
                                                              + " Tipo de estructura:" + arrayType.dataType);
            }
        }

        System.Diagnostics.Debug.WriteLine("----- FIN PARAMETROS ------");
    }

    public override string getType()
    {
        return type;
    }
}