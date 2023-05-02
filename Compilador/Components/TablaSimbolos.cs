using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

namespace Compilador.Components;

public class TablaSimbolos
{
    LinkedList<Object> tabla;

    private static int nivelActual;
    public enum DataType
    {
        Method,
        Variable,
        Array,
        Class,
        If,
        Else,
        For,
        While,
        Using,
        Error,

    }
    
    public enum BasicType
    {
        Int,
        Double,
        String,
        Boolean,
        Char,
        Void,
        Null,
        Error,
    }

    public class Ident
    {
        public IToken token;
        public BasicType tipo;
        public int nivelGlobal;
        public int nivelLocal;
        public DataType tipoDato;
        
        public Ident(IToken t, BasicType BasicType, DataType tipoDatos)
        {
            token = t;
            nivelGlobal = nivelActual;
            tipo = BasicType;
            nivelLocal = -1;
            tipoDato = tipoDatos;
        }
        
        public void setNivelLocal(int v)
        {
            nivelLocal = v;
        }
    }
    
    public TablaSimbolos()
    {
        tabla = new LinkedList<object>();
        nivelActual = -1;
    }
    
    public void Insertar(IToken id, BasicType tp, DataType dt,int  v)
    {
        Ident i = new Ident(id, tp, dt);
        i.setNivelLocal(v);
        tabla.AddFirst(i);
    }
    
    public Ident Buscar(IToken id)
    {
        foreach (Ident i in tabla)
        {
            if (i.token.Text.Equals(id.Text))
                return i;
        }
        return null;
    }
    
    public void OpenScope()
    {
        nivelActual++;
    }
    
    public void CloseScope()
    {
        tabla.Remove(new Func<Ident, bool>(n => n.nivelGlobal == nivelActual));
        nivelActual--;
    }
    
    public void Imprimir()
    {
        System.Diagnostics.Debug.WriteLine("----- INICIO TABLA ------");
        for (int i = 0; i < tabla.Count; i++)
        {
            IToken s = ((Ident)tabla.ElementAt(i)).token;
            System.Diagnostics.Debug.WriteLine("Nombre: " + s.Text + " - Nivel global: " +
                                               ((Ident)tabla.ElementAt(i)).nivelGlobal
                                               + " - Tipo basico " + ((Ident)tabla.ElementAt(i)).tipo
                                               + " - Tipo dato: " + ((Ident)tabla.ElementAt(i)).tipoDato
                                               + " - Nivel: " + ((Ident)tabla.ElementAt(i)).nivelLocal);
        }
        System.Diagnostics.Debug.WriteLine("----- FIN TABLA ------");
    }
    
}