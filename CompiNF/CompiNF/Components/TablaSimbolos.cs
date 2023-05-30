using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using CompiNF.Components.TypesManager;

namespace CompiNF.Components;

public class TablaSimbolos
{
    LinkedList<Object> tabla; // Lista de objetos de tipo TypeD

    public static int nivelActual;

    public MethodTypeD? currentMethod;
    public ClassTypeD? currentClass;

    public TablaSimbolos()
    {
        tabla = new LinkedList<Object>();
        nivelActual = -1;
    }

    public void Insertar(TypeD t)
    {
        tabla.AddLast(t);
    }

    public TypeD? Buscar(string id)
    {
        foreach (TypeD i in tabla)
        {
            if (i.token.Text.Equals(id))
            {
                return i;
            }
        }

        return null;
    }

    public TypeD? BuscarCustomVar(string id)
    {
        foreach (TypeD? i in tabla)
        {
            //if (i.token.Text.Equals(id) && i.nivel <= nivelActual)
            if (i.token.Text.Equals(id) && i.GetType() == typeof(CustomTypeD))
                return i;
        }

        return null;
    }

    public void Sacar(string nombreMetodo)
    {
        int posMethod = 0;
        LinkedList<Object> slicedList = new LinkedList<Object>();

        // Busco la posicion del metodo en la tabla
        for (int i = 0; i < tabla.Count; i++)
        {
            if (((TypeD)tabla.ElementAt(i)).token.Text.Equals(nombreMetodo))
            {
                posMethod = i;
            }
        }

        if (posMethod == 0)
        {
            System.Diagnostics.Debug.WriteLine("No se encontro el metodo");
        }
        else
        {
            // Agrego a la lista los elementos que estan antes del metodo
            for (int j = 0; j < tabla.Count; j++)
            {
                if (j <= posMethod)
                {
                    slicedList.AddLast(tabla.ElementAt(j));
                }
            }

            tabla.Clear();
            foreach (var child in slicedList)
            {
                tabla.AddLast(child);
            }
        }
    }

    public bool? buscarEnMetodo(string id)
    {
        bool found = false;
        int posMethod = 0;
        for (int i = 0; i < tabla.Count; i++)
        {
            if (((TypeD)tabla.ElementAt(i)).token.Text.Equals(currentMethod?.token.Text))
            {
                posMethod = i;
            }
        }

        if (posMethod == 0)
        {
            System.Diagnostics.Debug.WriteLine("No se encontro el metodo");
        }
        else
        {
            // Agrego a la lista los elementos que estan antes del metodo
            for (int j = 0; j < tabla.Count; j++)
            {
                if (j > posMethod)
                {
                    if (((TypeD)tabla.ElementAt(j)).token.Text.Equals(id))
                    {
                        found = true;
                        return found;
                    }
                }
            }
        }

        return found;
    }

    public void OpenScope()
    {
        nivelActual++;
    }

    public void CloseScope()
    {
        tabla.Remove(new Func<TypeD, bool>(n => n.nivel == nivelActual));
        nivelActual--;
    }

    public List<string> Imprimir()
    {
        List<string> lista = new List<string>();
        lista.Add("----- INICIO TABLA ------\n");
        System.Diagnostics.Debug.WriteLine("----- INICIO TABLA ------");
        for (int i = 0; i < tabla.Count; i++)
        {
            if (tabla.ElementAt(i).GetType() == typeof(BasicTypeD))
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + ((BasicTypeD)tabla.ElementAt(i)).token.Text
                                                              + " Nivel: " + ((BasicTypeD)tabla.ElementAt(i)).nivel
                                                              + " Tipo: " + ((BasicTypeD)tabla.ElementAt(i)).type);
                lista.Add("Nombre: " + ((BasicTypeD)tabla.ElementAt(i)).token.Text
                                     + " Nivel: " + ((BasicTypeD)tabla.ElementAt(i)).nivel
                                     + " Tipo: " + ((BasicTypeD)tabla.ElementAt(i)).type + "\n");
            }
            else if (tabla.ElementAt(i).GetType() == typeof(ClassTypeD))
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + ((ClassTypeD)tabla.ElementAt(i)).token.Text
                                                              + " Nivel: " + ((ClassTypeD)tabla.ElementAt(i)).nivel
                                                              + " Tipo: " + ((ClassTypeD)tabla.ElementAt(i)).type);
                lista.Add("Nombre: " + ((ClassTypeD)tabla.ElementAt(i)).token.Text
                                     + " Nivel: " + ((ClassTypeD)tabla.ElementAt(i)).nivel
                                     + " Tipo: " + ((ClassTypeD)tabla.ElementAt(i)).type + "\n");
            }
            else if (tabla.ElementAt(i).GetType() == typeof(MethodTypeD))
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + ((MethodTypeD)tabla.ElementAt(i)).token.Text
                                                              + " Nivel: " + ((MethodTypeD)tabla.ElementAt(i)).nivel
                                                              + " Tipo: " + ((MethodTypeD)tabla.ElementAt(i)).type
                                                              + " Cantidad de parametros: " +
                                                              ((MethodTypeD)tabla.ElementAt(i)).cantParams
                                                              + " Tipo de retorno: " +
                                                              ((MethodTypeD)tabla.ElementAt(i)).returnType);
                lista.Add("Nombre: " + ((MethodTypeD)tabla.ElementAt(i)).token.Text
                                     + " Nivel: " + ((MethodTypeD)tabla.ElementAt(i)).nivel
                                     + " Tipo: " + ((MethodTypeD)tabla.ElementAt(i)).type
                                     + " Cantidad de parametros: " +
                                     ((MethodTypeD)tabla.ElementAt(i)).cantParams
                                     + " Tipo de retorno: " +
                                     ((MethodTypeD)tabla.ElementAt(i)).returnType);

                if (((MethodTypeD)tabla.ElementAt(i)).paramsTypes.Count > 0)
                {
                    lista.Add(((MethodTypeD)tabla.ElementAt(i)).imprimirParams());
                }
                else
                {
                    lista.Add("\n");
                }
            }
            else if (tabla.ElementAt(i).GetType() == typeof(ArrayTypeD))
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + ((ArrayTypeD)tabla.ElementAt(i)).token.Text
                                                              + " Nivel: " + ((ArrayTypeD)tabla.ElementAt(i)).nivel
                                                              + " Tipo: " + ((ArrayTypeD)tabla.ElementAt(i)).type
                                                              + " Tipo de dato: " +
                                                              ((ArrayTypeD)tabla.ElementAt(i)).dataType);
                lista.Add("Nombre: " + ((ArrayTypeD)tabla.ElementAt(i)).token.Text
                                     + " Nivel: " + ((ArrayTypeD)tabla.ElementAt(i)).nivel
                                     + " Tipo: " + ((ArrayTypeD)tabla.ElementAt(i)).type
                                     + " Tipo de dato: " +
                                     ((ArrayTypeD)tabla.ElementAt(i)).dataType + "\n");
            }
            else if (tabla.ElementAt(i).GetType() == typeof(CustomTypeD))
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + ((CustomTypeD)tabla.ElementAt(i)).token.Text
                                                              + " Nivel: " + ((CustomTypeD)tabla.ElementAt(i)).nivel
                                                              + " Tipo: " + ((CustomTypeD)tabla.ElementAt(i)).Type
                                                              + " Tipo de dato: " +
                                                              ((CustomTypeD)tabla.ElementAt(i)).TypeOf);
                lista.Add("Nombre: " + ((CustomTypeD)tabla.ElementAt(i)).token.Text
                                     + " Nivel: " + ((CustomTypeD)tabla.ElementAt(i)).nivel
                                     + " Tipo: " + ((CustomTypeD)tabla.ElementAt(i)).Type
                                     + " Tipo de dato: " +
                                     ((CustomTypeD)tabla.ElementAt(i)).TypeOf + "\n");
            }
            else if (tabla.ElementAt(i).GetType() == typeof(UsingTypeD))
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + ((UsingTypeD)tabla.ElementAt(i)).token.Text
                                                              + " Nivel: " + ((UsingTypeD)tabla.ElementAt(i)).nivel
                                                              + " Tipo: " + ((UsingTypeD)tabla.ElementAt(i)).type);
                lista.Add("Nombre: " + ((UsingTypeD)tabla.ElementAt(i)).token.Text
                                     + " Nivel: " + ((UsingTypeD)tabla.ElementAt(i)).nivel
                                     + " Tipo: " + ((UsingTypeD)tabla.ElementAt(i)).type + "\n");
            }
        }

        lista.Add("----- FIN TABLA ------\n");
        System.Diagnostics.Debug.WriteLine("----- FIN TABLA ------");
        return lista;
    }
}