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
        public int nivel;
        public int tipo;
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
        tabla.AddLast(id);
    }
    
    public Ident Buscar(IToken id)
    {
        foreach (Ident i in tabla)
        {
            if (i.token.Text == id.Text)
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
        // hay que sacar todos los ident del nivel que se está cerrando
        LinkedListNode<object> current = tabla.First;
        while (current != null)
        {
            LinkedListNode<object> next = current.Next;
            if (((Ident)current.Value).nivel == nivelActual)
            {
                tabla.Remove(current);
            }
            current = next;
        }
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