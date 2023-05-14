﻿using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Compilador.Components.TypesManager;
using Type = Compilador.Components.TypesManager.Type;

namespace Compilador.Components;

public class TablaSimbolos
{
    LinkedList<Object> tabla;

    public static int nivelActual;

    public MethodType? currentMethod;
    public ClassType? currentClass;
    
    
    public TablaSimbolos()
    {
        tabla = new LinkedList<Object>();
        nivelActual = -1;
    }

    public void Insertar(Type t)
    {
        tabla.AddLast(t);
    }

    public Type? Buscar(string id)
    {
        foreach (Type? i in tabla)
        {
            //if (i.token.Text.Equals(id) && i.nivel <= nivelActual)
            if (i.token.Text.Equals(id))
                return i;
        }

        return null;
    }
    
    public Type? BuscarCustomVar(string id)
    {
        foreach (Type? i in tabla)
        {
            //if (i.token.Text.Equals(id) && i.nivel <= nivelActual)
            if (i.token.Text.Equals(id) && i.GetType() == typeof(CustomType))
                return i;
        }
        return null;
    }
    public void Sacar(string nombreMetodo)
    {
        int posMethod=0;
        LinkedList<Object> slicedList = new LinkedList<Object>();
        
        // Busco la posicion del metodo en la tabla
        for (int i = 0; i <tabla.Count ; i++)
        {
            if (((Type)tabla.ElementAt(i)).token.Text.Equals(nombreMetodo))
            {
                posMethod = i;
            }
        }
        
        if (posMethod ==0)
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
        

    public void OpenScope()
    {
        nivelActual++;
    }

    public void CloseScope()
    {
        tabla.Remove(new Func<Type, bool>(n => n.nivel == nivelActual));
        nivelActual--;
    }

    public void Imprimir()
    {
        System.Diagnostics.Debug.WriteLine("----- INICIO TABLA ------");
        for (int i = 0; i < tabla.Count; i++)
        {
            if (tabla.ElementAt(i).GetType() == typeof(BasicType))
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + ((BasicType)tabla.ElementAt(i)).token.Text
                                                              + " Nivel: " + ((BasicType)tabla.ElementAt(i)).nivel
                                                              + " Tipo: " + ((BasicType)tabla.ElementAt(i)).type);
            }
            else if (tabla.ElementAt(i).GetType() == typeof(ClassType))
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + ((ClassType)tabla.ElementAt(i)).token.Text
                                                              + " Nivel: " + ((ClassType)tabla.ElementAt(i)).nivel
                                                              + " Tipo: " + ((ClassType)tabla.ElementAt(i)).type);
            }
            else if (tabla.ElementAt(i).GetType() == typeof(MethodType))
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + ((MethodType)tabla.ElementAt(i)).token.Text
                                                              + " Nivel: " + ((MethodType)tabla.ElementAt(i)).nivel
                                                              + " Tipo: " + ((MethodType)tabla.ElementAt(i)).type
                                                              + " Cantidad de parametros: " +
                                                              ((MethodType)tabla.ElementAt(i)).cantParams
                                                              + " Tipo de retorno: " +
                                                              ((MethodType)tabla.ElementAt(i)).returnType);

                if (((MethodType)tabla.ElementAt(i)).paramsTypes.Count > 0)
                {
                    ((MethodType)tabla.ElementAt(i)).imprimirParams();
                }
            }
            else if (tabla.ElementAt(i).GetType() == typeof(ArrayType))
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + ((ArrayType)tabla.ElementAt(i)).token.Text
                                                              + " Nivel: " + ((ArrayType)tabla.ElementAt(i)).nivel
                                                              + " Tipo: " + ((ArrayType)tabla.ElementAt(i)).type
                                                              + " Tipo de dato: " +
                                                              ((ArrayType)tabla.ElementAt(i)).dataType);
            }
            else if (tabla.ElementAt(i).GetType() == typeof(CustomType))
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + ((CustomType)tabla.ElementAt(i)).token.Text
                                                              + " Nivel: " + ((CustomType)tabla.ElementAt(i)).nivel
                                                              + " Tipo: " + ((CustomType)tabla.ElementAt(i)).Type
                                                              + " Tipo de dato: " +
                                                              ((CustomType)tabla.ElementAt(i)).TypeOf);
            }
            else if (tabla.ElementAt(i).GetType() == typeof(UsingType))
            {
                System.Diagnostics.Debug.WriteLine("Nombre: " + ((UsingType)tabla.ElementAt(i)).token.Text
                                                              + " Nivel: " + ((UsingType)tabla.ElementAt(i)).nivel
                                                              + " Tipo: " + ((UsingType)tabla.ElementAt(i)).type);
            }
        }

        System.Diagnostics.Debug.WriteLine("----- FIN TABLA ------");
    }
}