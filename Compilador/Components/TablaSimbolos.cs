using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

namespace Compilador.Components;

public class TablaSimbolos
{
    LinkedList<Object> tabla;

    private static int nivelActual;

    public class Ident
    {
        public IToken token;
        public int tipo;
        public int nivel;
        public int valor;
        public bool isMethod;
        
        public Ident(IToken t, int tp, bool isM)
        {
            token = t;
            nivel = nivelActual;
            tipo = tp;
            valor = 0;
            isMethod = isM;
        }
        public void setValor(int v)
        {
            valor = v;
        }
    }
    
    public TablaSimbolos()
    {
        tabla = new LinkedList<object>();
        nivelActual = -1;
    }
    
    public void Insertar(IToken id, int tp, bool isM)
    {
        Ident i = new Ident(id, tp, isM);
        tabla.AddFirst(id);
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
        tabla.Remove(new Func<Ident, bool>(n => n.nivel == nivelActual));
        nivelActual--;
    }
    
    public void Imprimir()
    {
        Console.WriteLine("----- INICIO TABLA ------");
        for (int i = 0; i < tabla.Count; i++)
        {
            IToken s = ((Ident)tabla.ElementAt(i)).token;
            Console.WriteLine("Nombre: " + s.Text + " - " + ((Ident)tabla.ElementAt(i)).nivel + " - " + ((Ident)tabla.ElementAt(i)).tipo);
        }
        Console.WriteLine("----- FIN TABLA ------");
    }
    
}