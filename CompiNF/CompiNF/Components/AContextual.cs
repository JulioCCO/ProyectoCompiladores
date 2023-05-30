﻿using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Compilador.TypesStructure;
using generated;
using CompiNF.Components.TypesManager;

namespace CompiNF.Components;

public class AContextual : AlphaParserBaseVisitor<object>
{
    public TablaSimbolos tabla; // tabla de simbolos global
    public ErrorBuilder errorBuilder = new ErrorBuilder();
    public int contadorGlobal = 0;
    public AContextual()
    {
        tabla = new TablaSimbolos();
    }

    public string obtenerCoordenadas(IToken token)
    {
        return " Linea: " + token.Line + " Columna: " + token.Column;
    }

    /*
    // program : (using)* CLASS ident LEFT_BRACE (varDecl | classDecl | methodDecl)* RIGHT_BRACE EOF   #ProgramClassAST;
    */
    public override object? VisitProgramClassAST(AlphaParser.ProgramClassASTContext context)
    {
        try
        {
            tabla.OpenScope();
            IToken Tok = (IToken)Visit(context.ident());
            ;
            ClassTypeD cls = new ClassTypeD(Tok, TablaSimbolos.nivelActual, context);
            tabla.Insertar(cls);
            foreach (var child in context.children)
            {
                Visit(child);
            }

            tabla.CloseScope();
        }
        catch (Exception e)
        {
            IToken Tok = (IToken)Visit(context.ident());
            errorBuilder.AddError("Error en visit ProgramClassAST" + e.Message + " " + obtenerCoordenadas(Tok));
            System.Diagnostics.Debug.WriteLine("Error en visit ProgramClassAST" + e.Message + " " +
                                               obtenerCoordenadas(Tok));
            throw;
        }

        List<string> TablaList = tabla.Imprimir();
        return errorBuilder.BuildMessage() + "\n" + string.Join("\n", TablaList);
    }

    /*
    // using : USING ident SEMICOLON    #UsingClassAST;
    */
    public override object? VisitUsingClassAST(AlphaParser.UsingClassASTContext context)
    {
        try
        {
            IToken Tok = (IToken)Visit(context.ident());
            UsingTypeD usingTypeD = new UsingTypeD(Tok, TablaSimbolos.nivelActual, context);
            tabla.Insertar(usingTypeD);
        }
        catch (Exception e)
        {
            IToken Tok = (IToken)Visit(context.ident());
            System.Diagnostics.Debug.WriteLine("Error en visit UsingClassAST " + e.Message + " " +
                                               obtenerCoordenadas(Tok));
            errorBuilder.AddError("Error en visit UsingClassAST " + e.Message + " " + obtenerCoordenadas(Tok));
            throw;
        }

        return null;
    }

    /*
    // varDecl : type ident (COMMA ident)* SEMICOLON    #VarDeclAST;
        int  carro, moto, avion;
        int[] x, y, z;
        int x;
        Clase x;
         int x, y, m, j;
    */
    public override object? VisitVarDeclAST(AlphaParser.VarDeclASTContext context)
    {
        try
        {
            var ultimasDosPosiciones =
                context.type().GetText()
                    .Substring(context.type().GetText().Length - 2); // [] para saber si es arreglo
            var textoSinUltimosDosCaracteres =
                context.type().GetText().Substring(0, context.type().GetText().Length - 2);

            if (BasicTypeD.isBasicType(context.type().GetText()) ==
                true) // si es tipo basico ----------------------------------------------------------------
            {
                foreach (var child in context.ident())
                {
                    IToken Tok = (IToken)Visit(child);
                    BasicTypeD.Types varTipo = BasicTypeD.showType(context.type().GetText());
                    BasicTypeD var = new BasicTypeD(Tok, varTipo, TablaSimbolos.nivelActual, context);
                    if (tabla.currentClass != null) // es atributo de clase 
                    {
                        if (!tabla.currentClass.BuscarAtributo(var.token.Text))
                        {
                            TypeD? tipo = tabla.Buscar(Tok.Text);
                            if (tipo != null && tipo.nivel == 0)
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    "Error en visit VarDeclAST tipo basico, variable ya existe: " +
                                    Tok.Text + " " + obtenerCoordenadas(Tok));

                                errorBuilder.AddError("Error en visit VarDeclAST tipo basico, variable ya existe: " +
                                                      Tok.Text + " " + obtenerCoordenadas(Tok));
                            }

                            tabla.Insertar(var);
                            child.declPointer = context;
                            context.isLocal = true;
                            context.indexVar = contadorGlobal;
                            contadorGlobal++;
                            tabla.currentClass.attributes.AddLast(var);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, atributo ya existe: " +
                                                               var.token.Text + " " + obtenerCoordenadas(Tok));
                            errorBuilder.AddError("Error en visit VarDeclAST, atributo ya existe: " +
                                                  var.token.Text + " " + obtenerCoordenadas(Tok));
                        }
                    }
                    else if (tabla.currentMethod != null) // es variable local de metodo
                    {
                        TypeD? tipo = tabla.Buscar(Tok.Text);
                        if (tipo != null)
                        {
                            if (tipo.nivel == 0) // es variable global
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    "Error en visit VarDeclAST tipo basico, variable ya existe: " +
                                    Tok.Text + " " + obtenerCoordenadas(Tok));
                                errorBuilder.AddError("Error en visit VarDeclAST tipo basico, variable ya existe: " +
                                                      Tok.Text + " " + obtenerCoordenadas(Tok));
                            }
                            else if (tipo.nivel >= 1) // es variable local
                            {
                                // TODO: revisar si es el mismo metodo
                                var validar = tabla.buscarEnMetodo(Tok.Text);
                                if (validar == true)
                                {
                                    System.Diagnostics.Debug.WriteLine(
                                        "Error en visit VarDeclAST tipo basico, variable ya existe: " +
                                        Tok.Text + " " + obtenerCoordenadas(Tok));
                                    errorBuilder.AddError(
                                        "Error en visit VarDeclAST tipo basico, variable ya existe: " +
                                        Tok.Text + " " + obtenerCoordenadas(Tok));
                                }
                                child.declPointer = context;
                                context.isLocal = true;
                                context.indexVar = contadorGlobal;
                                contadorGlobal++;
                                tabla.Insertar(var);
                            }
                        }
                        else
                        {
                            child.declPointer = context;
                            context.isLocal = true;
                            context.indexVar = contadorGlobal;
                            contadorGlobal++;
                            tabla.Insertar(var);
                        }
                    }
                    else if (tabla.currentClass == null && tabla.currentMethod == null) // es variable global
                    {
                        TypeD? tipo = tabla.Buscar(Tok.Text);
                        if (tipo != null && tipo.nivel == 0)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Error en visit VarDeclAST tipo basico, variable ya existe: " +
                                Tok.Text + " " + obtenerCoordenadas(Tok));
                            errorBuilder.AddError("Error en visit VarDeclAST tipo basico, variable ya existe: " +
                                                  Tok.Text + " " + obtenerCoordenadas(Tok));
                        }
                        else
                        {
                            child.declPointer = context;
                            context.isLocal = false;
                            context.indexVar = contadorGlobal;
                            contadorGlobal++;
                            tabla.Insertar(var);
                        }
                    }
                }
            }
            else if (ultimasDosPosiciones.Equals("[]")) // si es arreglo
            {
                ArrayTypeD.Types type = ArrayTypeD.showType(textoSinUltimosDosCaracteres);

                if (type is ArrayTypeD.Types.Error)
                {
                    System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, tipo recibido: " +
                                                       context.type().GetText() + " " +
                                                       obtenerCoordenadas(context.type().Start));
                    errorBuilder.AddError("Error en visit VarDeclAST, tipo recibido: " +
                                          context.type().GetText() + " " + obtenerCoordenadas(context.type().Start));
                }
                else
                {
                    foreach (var child in context.ident())
                    {
                        if (tabla.currentClass != null) // es una varaible de una clase
                        {
                            System.Diagnostics.Debug.WriteLine("la clase solo puede tener variables de tipo basico." +
                                                               obtenerCoordenadas(child.Start));
                            errorBuilder.AddError("la clase solo puede tener variables de tipo basico." +
                                                  obtenerCoordenadas(child.Start));
                        }
                        else if (tabla.currentMethod != null) // es variable local de metodo
                        {
                            IToken tok = (IToken)Visit(child);
                            TypeD? tipo = tabla.Buscar(tok.Text);
                            if (tipo != null && tipo.nivel <= TablaSimbolos.nivelActual)
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    "Error en visit VarDeclAST tipo arreglo, variable ya existe: " +
                                    tok.Text + " " + obtenerCoordenadas(tok));
                                errorBuilder.AddError("Error en visit VarDeclAST tipo arreglo, variable ya existe: " +
                                                      tok.Text + " " + obtenerCoordenadas(tok));
                            }
                            else
                            {
                                ArrayTypeD arr = new ArrayTypeD(tok, TablaSimbolos.nivelActual, type, context);
                                child.declPointer = context;
                                context.isLocal = true;
                                context.indexVar = contadorGlobal;
                                contadorGlobal++;
                                tabla.Insertar(arr);
                            }
                        }
                        else if (tabla.currentClass == null && tabla.currentMethod == null) // es variable global
                        {
                            IToken tok = (IToken)Visit(child);
                            TypeD? tipo = tabla.Buscar(tok.Text);
                            if (tipo != null)
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    "Error en visit VarDeclAST variable global, variable ya existe: " +
                                    tok.Text + " " + obtenerCoordenadas(tok));
                                errorBuilder.AddError(
                                    "Error en visit VarDeclAST variable global, variable ya existe: " +
                                    tok.Text + " " + obtenerCoordenadas(tok));
                            }
                            else
                            {
                                ArrayTypeD arr = new ArrayTypeD(tok, TablaSimbolos.nivelActual, type, context);
                                child.declPointer = context;
                                context.isLocal = false;
                                context.indexVar = contadorGlobal;
                                contadorGlobal++;
                                tabla.Insertar(arr);
                            }
                        }
                    }
                }
            }
            else if (tabla.Buscar(context.type().GetText()) != null &&
                     tabla.Buscar(context.type().GetText()) is ClassTypeD) // si es tipo compuesto
            {
                foreach (var child in context.ident())
                {
                    IToken tok = (IToken)Visit(child);
                    if (tabla.currentClass != null) // si estoy en una clase
                    {
                        System.Diagnostics.Debug.WriteLine("la clase solo puede tener variables de tipo basico.");
                    }
                    else if (tabla.currentMethod != null) // si estoy en un metodo
                    {
                        TypeD? tipo = tabla.Buscar(tok.Text);
                        if (tipo != null && tipo.nivel <= TablaSimbolos.nivelActual)
                        {
                            System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, variable ya existe: " +
                                                               tok.Text + " " + obtenerCoordenadas(tok));
                            errorBuilder.AddError("Error en visit VarDeclAST, variable ya existe: " +
                                                  tok.Text + " " + obtenerCoordenadas(tok));
                        }
                        else
                        {
                            CustomTypeD var = new CustomTypeD(tok, TablaSimbolos.nivelActual, context.type().GetText(),
                                context);
                            child.declPointer = context;
                            context.isLocal = true;
                            context.indexVar = contadorGlobal;
                            contadorGlobal++;
                            tabla.Insertar(var);
                        }
                    }
                    else if (tabla.currentClass == null && tabla.currentMethod == null) // es variable global
                    {
                        TypeD? tipo = tabla.Buscar(tok.Text);
                        if (tipo != null)
                        {
                            System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, variable ya existe: " +
                                                               tok.Text + " " + obtenerCoordenadas(tok));
                            errorBuilder.AddError("Error en visit VarDeclAST, variable ya existe: " +
                                                  tok.Text + " " + obtenerCoordenadas(tok));
                        }
                        else
                        {
                            CustomTypeD var = new CustomTypeD(tok, TablaSimbolos.nivelActual, context.type().GetText(),
                                context);
                            child.declPointer = context;
                            context.isLocal = false;
                            context.indexVar = contadorGlobal;
                            contadorGlobal++;
                            tabla.Insertar(var);
                        }
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, tipo recibido: " +
                                                   context.type().GetText() + " " +
                                                   obtenerCoordenadas(context.type().Start));
                errorBuilder.AddError("Error en visit VarDeclAST, tipo recibido: " +
                                      context.type().GetText() + " " + obtenerCoordenadas(context.type().Start));
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST: " + e.Message + " " +
                                               obtenerCoordenadas(context.type().Start));
            errorBuilder.AddError("Error en visit VarDeclAST: " + e.Message + " " +
                                  obtenerCoordenadas(context.type().Start));
            //throw;
        }

        return null;
    }

    /*
     classDecl : CLASS ident LEFT_BRACE (varDecl)* RIGHT_BRACE  #ClassDeclAST;     
     */
    public override object? VisitClassDeclAST(AlphaParser.ClassDeclASTContext context)
    {
        IToken tok = (IToken)Visit(context.ident());
        if (tabla.Buscar(context.ident().GetText()) != null)
        {
            System.Diagnostics.Debug.WriteLine("Error en visit ClassDeclAST, clase ya existe: " +
                                               context.ident().GetText() + " " + obtenerCoordenadas(tok));
            errorBuilder.AddError("Error en visit ClassDeclAST, clase ya existe: " +
                                  context.ident().GetText() + " " + obtenerCoordenadas(tok));
            return null;
        }

        ClassTypeD cls = new ClassTypeD(tok, TablaSimbolos.nivelActual, context);
        tabla.Insertar(cls);
        tabla.currentClass = cls; // para saber en que clase estoy
        tabla.OpenScope();

        foreach (var child in context.varDecl())
        {
            Visit(child);
        }

        tabla.CloseScope();

        tabla.currentClass = null; // como se sale de la clase, se pone en null
        return null;
    }

    /*
        methodDecl : (type | VOID) ident LEFT_PAREN formPars? RIGHT_PAREN block #MethodDeclAST;
     */
    public override object? VisitMethodDeclAST(AlphaParser.MethodDeclASTContext context)
    {
        IToken tok = (IToken)Visit(context.ident());
        TypeD tipo = tabla.Buscar(tok.Text) as TypeD;

        if (tabla.Buscar(tok.Text) != null)
        {
            System.Diagnostics.Debug.WriteLine("Error en visit MethodDeclAST ya existe un objeto llamado: "
                                               + context.ident().GetText() + " de tipo: " + tipo.GetType() + " " +
                                               obtenerCoordenadas(tok));
            errorBuilder.AddError("Error en visit MethodDeclAST ya existe un objeto llamado: "
                                  + context.ident().GetText() + " de tipo: " + tipo.GetType() + " " +
                                  obtenerCoordenadas(tok));
            return null;
        }

        LinkedList<TypeD> parametros = new LinkedList<TypeD>();

        if (context.formPars() != null)
        {
            tabla.OpenScope();
            parametros = (LinkedList<TypeD>)Visit(context.formPars());
            tabla.CloseScope();
        }

        if (context.type() != null) // Si tiene tipo 
        {
            if (BasicTypeD.isBasicType(context.type().GetText()) == true) // si es tipo basico
            {
                if (BasicTypeD.showType(context.type().GetText()) != BasicTypeD.Types.Error)
                {
                    MethodTypeD method = new MethodTypeD(tok, TablaSimbolos.nivelActual, parametros.Count,
                        context.type().GetText(), parametros, context);
                    tabla.Insertar(method);
                    tabla.currentMethod = method;
                }
            }
            else if (tabla.Buscar(context.type().GetText()) != null &&
                     tabla.Buscar(context.type().GetText()) is ClassTypeD) // si es tipo compuesto
            {
                MethodTypeD method = new MethodTypeD(tok, TablaSimbolos.nivelActual, parametros.Count,
                    context.type().GetText(), parametros, context);
                tabla.Insertar(method);
                tabla.currentMethod = method;
            }
            else // error
            {
                System.Diagnostics.Debug.WriteLine("Error en visit MethodDeclAST, tipo recibido: " +
                                                   context.type().GetText() + " " +
                                                   obtenerCoordenadas(context.type().Start));
                errorBuilder.AddError("Error en visit MethodDeclAST, tipo recibido: " +
                                      context.type().GetText() + " " + obtenerCoordenadas(context.type().Start));
            }
        }
        else if (context.VOID() != null) // si no tiene tipo, es void
        {
            MethodTypeD method = new MethodTypeD(tok, TablaSimbolos.nivelActual, parametros.Count, "void", parametros,
                context);
            tabla.Insertar(method);
            tabla.currentMethod = method;
        }
        else // error
        {
            System.Diagnostics.Debug.WriteLine("Error en visit MethodDeclAST, tipo recibido: " +
                                               context.type().GetText() + " " + obtenerCoordenadas(tok));
            errorBuilder.AddError("Error en visit MethodDeclAST, tipo recibido: " +
                                  context.type().GetText() + " " + obtenerCoordenadas(tok));
        }


        foreach (var child in parametros)
        {
            tabla.Insertar(child);
        }

        tabla.OpenScope();
        Visit(context.block());

        MethodTypeD metodo = (MethodTypeD)tabla.currentMethod;
        AlphaParser.ReturnStatementASTContext? returnAst = metodo?.returnStatement;

        // si el metodo no es void y no tiene return statement
        if (metodo.returnType != "void" && returnAst == null)
        {
            System.Diagnostics.Debug.WriteLine(
                "Error en visit MethodDeclAST, el metodo no es void y no tiene return statement: " +
                context.ident().GetText() + " " + obtenerCoordenadas(tok));
            errorBuilder.AddError("Error en visit MethodDeclAST, el metodo no es void y no tiene return statement: " +
                                  context.ident().GetText() + " " + obtenerCoordenadas(tok));
        }
        else
        {
            contextMethodReturn(returnAst);
        }

        if (metodo.returnType == "void" && returnAst != null)
        {
            System.Diagnostics.Debug.WriteLine(
                "Error en visit MethodDeclAST, el metodo es void y tiene return statement: " +
                context.ident().GetText() + " " + obtenerCoordenadas(tok));
            errorBuilder.AddError("Error en visit MethodDeclAST, el metodo es void y tiene return statement: " +
                                  context.ident().GetText() + " " + obtenerCoordenadas(tok));
        }

        tabla.Sacar(tok.Text);
        tabla.CloseScope();
        tabla.currentMethod = null;
        return null;
    }

    public object? contextMethodReturn(AlphaParser.ReturnStatementASTContext? context)
    {
        if (context == null)
        {
            return null;
        }
        
        if (tabla.currentMethod?.returnType == "void")
        {
            if (context.RETURN() != null)
            {
                System.Diagnostics.Debug.WriteLine(
                    "Error en visit ReturnStatementAST, los metodos void no retornan datos.");
                errorBuilder.AddError("Error en visit ReturnStatementAST, los metodos void no retornan datos.");
                return null;
            }
        }

        else if (tabla.currentMethod?.returnType != "void")
        {
            if (context.RETURN() != null && context.expr() != null)
            {
                string? tipoReturn = (string)Visit(context.expr());
                if (tipoReturn == null)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Error en visit ReturnStatementAST, tipo de retorno incorrecto: " +
                        " El tipo de return del metodo es: " +
                        tabla.currentMethod.returnType +
                        " y el tipo de retorno del return es: " + tipoReturn + " " +
                        obtenerCoordenadas(context.Start));
                    errorBuilder.AddError("Error en visit ReturnStatementAST, tipo de retorno incorrecto: " +
                                          " El tipo de return del metodo es: " +
                                          tabla.currentMethod.returnType +
                                          " y el tipo de retorno del return es: " + tipoReturn + " " +
                                          obtenerCoordenadas(context.Start));
                }
                else if (tabla.currentMethod.returnType == "void")
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Error en visit ReturnStatementAST, metodos void no retornan datos: " +
                        context.expr().GetText() + " " + obtenerCoordenadas(context.Start));
                    errorBuilder.AddError("Error en visit ReturnStatementAST, metodos void no retornan datos: " +
                                          context.expr().GetText() + " " + obtenerCoordenadas(context.Start));
                }
                else if (tipoReturn.ToLower() != tabla.currentMethod.returnType)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Error en visit ReturnStatementAST, tipo de retorno incorrecto: " +
                        " El tipo de return del metodo es: " +
                        tabla.currentMethod.returnType +
                        " y el tipo de retorno del return es: " + tipoReturn + " " +
                        obtenerCoordenadas(context.Start));
                    errorBuilder.AddError("Error en visit ReturnStatementAST, tipo de retorno incorrecto: " +
                                          " El tipo de return del metodo es: " +
                                          tabla.currentMethod.returnType +
                                          " y el tipo de retorno del return es: " + tipoReturn + " " +
                                          obtenerCoordenadas(context.Start));
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error en visit ReturnStatementAST, falta return.");
                errorBuilder.AddError("Error en visit ReturnStatementAST, falta return.");
                return null;
            }
        }

        return null;
    }


    /*
     * formPars : type ident (COMMA type ident)*    #FormParsAST;
     *  int x, char y, int z
     *  int x, char y, int z[]
     */
    public override object VisitFormParsAST(AlphaParser.FormParsASTContext context)
    {
        LinkedList<TypeD> parametros = new LinkedList<TypeD>();

        for (int i = 0; i < context.ident().Length; i++)
        {
            var ultimasDosPosiciones =
                context.type(i).GetText()
                    .Substring(context.type(i).GetText().Length - 2); // [] para saber si es arreglo
            var textoSinUltimosDosCaracteres =
                context.type(i).GetText().Substring(0, context.type(i).GetText().Length - 2);

            IToken tok = (IToken)Visit(context.ident(i));
            if (BasicTypeD.isBasicType(context.type(i).GetText())) // si es tipo basico
            {
                BasicTypeD.Types varTipo = BasicTypeD.showType(context.type(i).GetText());
                BasicTypeD var = new BasicTypeD(tok, varTipo, TablaSimbolos.nivelActual, context);
                parametros.AddLast(var);
            }
            else if (tabla.Buscar(context.type(i).GetText()) != null &&
                     tabla.Buscar(context.type(i).GetText()) is ClassTypeD) // si es tipo compuesto
            {
                CustomTypeD var = new CustomTypeD(tok, TablaSimbolos.nivelActual, context.type(i).GetText(), context);
                parametros.AddLast(var);
            }
            else if (ultimasDosPosiciones.Equals("[]")) // si es arreglo
            {
                ArrayTypeD.Types type = ArrayTypeD.showType(textoSinUltimosDosCaracteres);

                if (type is ArrayTypeD.Types.Error)
                {
                    System.Diagnostics.Debug.WriteLine("Error en visit formPars, tipo recibido: " +
                                                       context.type(i).GetText() + " " + obtenerCoordenadas(tok));
                    errorBuilder.AddError("Error en visit formPars, tipo recibido: " +
                                          context.type(i).GetText() + " " + obtenerCoordenadas(tok));
                }
                else if (type is ArrayTypeD.Types.Int or ArrayTypeD.Types.Char)
                {
                    ArrayTypeD arr = new ArrayTypeD(tok, TablaSimbolos.nivelActual, type, context);
                    parametros.AddLast(arr);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error en visit FormParsAST, tipo recibido: " +
                                                   context.type(i).GetText() + " " + obtenerCoordenadas(tok));
                errorBuilder.AddError("Error en visit FormParsAST, tipo recibido: " +
                                      context.type(i).GetText() + " " + obtenerCoordenadas(tok));
            }
        }

        return parametros;
    }

    /*
     * type : ident array?  #TypeAST; 
     */
    public override object? VisitTypeAST(AlphaParser.TypeASTContext context)
    {
        IToken token = (IToken)Visit(context.ident());
        if (context.array() != null)
        {
            Visit(context.array());
        }

        return token;
    }

    /*
        statement : designator (ASSIGN expr | LEFT_PAREN actPars? RIGHT_PAREN | INC | DEC) SEMICOLON    #AssignStatementAST    
     */
    public override object? VisitAssignStatementAST(AlphaParser.AssignStatementASTContext context)
    {
        string tipoDesignator = (string)Visit(context.designator());
        if (context.expr() !=
            null) //  si es una asignacion ------------------------------------------------------------------------------------
        {
            string tipoExp = (string)Visit(context.expr());

            if (tipoDesignator != null && tipoExp != null)
            {
                if (tipoDesignator.Contains("[]") && context.expr().GetText().Contains("new"))
                {
                    if (!tipoDesignator.ToLower().Contains(tipoExp.ToLower()))
                    {
                        System.Diagnostics.Debug.WriteLine("Error de tipo en asignacion: " + tipoDesignator +
                                                           " diferente a " +
                                                           tipoExp + " " + obtenerCoordenadas(context.Start));
                        errorBuilder.AddError("Error de tipo en asignacion: " + tipoDesignator +
                                              " diferente a " +
                                              tipoExp + " " + obtenerCoordenadas(context.Start));
                    }

                    return null;
                }

                if (!String.Equals(tipoDesignator, tipoExp, StringComparison.CurrentCultureIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine("Error de tipo en asignacion: " + tipoDesignator +
                                                       " diferente a " +
                                                       tipoExp + " " + obtenerCoordenadas(context.Start));
                    errorBuilder.AddError("Error de tipo en asignacion: " + tipoDesignator +
                                          " diferente a " +
                                          tipoExp + " " + obtenerCoordenadas(context.Start));
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error de tipo en asignacion nula" + " " +
                                                   obtenerCoordenadas(context.Start));
                errorBuilder.AddError("Error de tipo en asignacion nula" + " " +
                                      obtenerCoordenadas(context.Start));
            }
        }
        else if
            (context.LEFT_PAREN() !=
             null) // si es una llamada a metodo ------------------------------------------------------------------------------------
        {
            TypeD? type = tabla.Buscar(context.designator().GetText());

            if (context.designator().GetText() ==
                "del") // si es del ------------------------------------------------------------------------------------
            {
                //del(arreglo, 1);
                // TODO: acceder al tipo de la variable y ver si es int y disminuir su valor
                if (context.actPars() == null)
                {
                    System.Diagnostics.Debug.WriteLine("Error metodo del sin parametros" + " " +
                                                       obtenerCoordenadas(context.Start));
                    errorBuilder.AddError("Error metodo del sin parametros" + " " +
                                          obtenerCoordenadas(context.Start));
                }
                else
                {
                    // lista de parametros del metodo
                    LinkedList<TypeD> actParm = (LinkedList<TypeD>)Visit(context.actPars());
                    if (actParm.Count != 2)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            "Error de parametros, cantidad de parametros no coinciden en el del" + " " +
                            obtenerCoordenadas(context.Start));
                        errorBuilder.AddError("Error de parametros, cantidad de parametros no coinciden en el del" +
                                              " " + obtenerCoordenadas(context.Start));
                    }
                    else
                    {
                        if (actParm.ElementAt(0) is ArrayTypeD &&
                            actParm.ElementAt(1).getType().Equals("Int"))
                        {
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Error de parametros, tipos de parametros no coinciden en el del" + " " +
                                obtenerCoordenadas(context.Start));
                            errorBuilder.AddError("Error de parametros, tipos de parametros no coinciden en el del" +
                                                  " " + obtenerCoordenadas(context.Start));
                            return null;
                        }
                    }
                }
            }
            else if
                (context.designator().GetText() ==
                 "len") // si es len ------------------------------------------------------------------------------------
            {
                //len(arreglo);
                // TODO: acceder al tipo de la variable y ver si es int y disminuir su valor
                if (context.actPars() == null)
                {
                    System.Diagnostics.Debug.WriteLine("Error metodo len sin parametros" + " " +
                                                       obtenerCoordenadas(context.Start));
                    errorBuilder.AddError("Error metodo len sin parametros" + " " +
                                          obtenerCoordenadas(context.Start));
                }
                else
                {
                    // lista de parametros del metodo
                    LinkedList<TypeD> actParm = (LinkedList<TypeD>)Visit(context.actPars());
                    if (actParm.Count != 1)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            "Error de parametros, cantidad de parametros no coinciden en el len" + " " +
                            obtenerCoordenadas(context.Start));
                        errorBuilder.AddError("Error de parametros, cantidad de parametros no coinciden en el len" +
                                              " " + obtenerCoordenadas(context.Start));
                    }
                    else
                    {
                        if (actParm.ElementAt(0) is ArrayTypeD)
                        {
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Error de parametros, tipos de parametros no coinciden en el len" + " " +
                                obtenerCoordenadas(context.Start));
                            errorBuilder.AddError("Error de parametros, tipos de parametros no coinciden en el len" +
                                                  " " + obtenerCoordenadas(context.Start));
                        }
                    }
                }
            }
            else if
                (context.designator().GetText() ==
                 "add") // si es add ------------------------------------------------------------------------------------
            {
                //add(arreglo, 0);
                // TODO: acceder al tipo de la variable y ver si es int y disminuir su valor
                if (context.actPars() == null)
                {
                    System.Diagnostics.Debug.WriteLine("Error metodo add sin parametros" + " " +
                                                       obtenerCoordenadas(context.Start));
                    errorBuilder.AddError("Error metodo add sin parametros" + " " +
                                          obtenerCoordenadas(context.Start));
                }
                else
                {
                    // lista de parametros del metodo
                    LinkedList<TypeD> actParm = (LinkedList<TypeD>)Visit(context.actPars());
                    if (actParm.Count != 2)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            "Error de parametros, cantidad de parametros no coinciden en el add" + " " +
                            obtenerCoordenadas(context.Start));
                        errorBuilder.AddError("Error de parametros, cantidad de parametros no coinciden en el add" +
                                              " " + obtenerCoordenadas(context.Start));
                    }
                    else
                    {
                        if (actParm.ElementAt(0).getType().Equals(actParm.ElementAt(1).getType()))
                        {
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Error de parametros, tipos de parametros no coinciden en el add" + " " +
                                obtenerCoordenadas(context.Start));
                            errorBuilder.AddError("Error de parametros, tipos de parametros no coinciden en el add" +
                                                  " " + obtenerCoordenadas(context.Start));
                        }
                    }
                }
            }
            else if
                (type is MethodTypeD method) // si es un metodo ------------------------------------------------------------------------------------
            {
                // TODO: Verificar que el metodo exista y que los parametros sean correctos
                if (context.actPars() != null)
                {
                    LinkedList<TypeD> actParm = new LinkedList<TypeD>();
                    actParm = (LinkedList<TypeD>)Visit(context.actPars());

                    if (actParm.Count == method.cantParams)
                    {
                        for (int i = 0; i < method.cantParams; i++)
                        {
                            if (method.paramsTypes.ElementAt(i).getType() != actParm.ElementAt(i).getType())
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    "Error de parametros, tipos de parametros no coinciden" + " " +
                                    obtenerCoordenadas(context.Start));
                                errorBuilder.AddError("Error de parametros, tipos de parametros no coinciden" + " " +
                                                      obtenerCoordenadas(context.Start));
                            }
                        }
                    }
                    else if (actParm.Count < method.cantParams)
                    {
                        System.Diagnostics.Debug.WriteLine("Error de parametros, faltan parametros" + " " +
                                                           obtenerCoordenadas(context.Start));
                        errorBuilder.AddError("Error de parametros, faltan parametros" + " " +
                                              obtenerCoordenadas(context.Start));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Error de parametros, sobran parametros " + " " +
                                                           obtenerCoordenadas(context.Start));
                        errorBuilder.AddError("Error de parametros, sobran parametros " + " " +
                                              obtenerCoordenadas(context.Start));
                    }
                }
                else if (method.cantParams > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Error en visit AssignStatementAST, faltan parametros: " +
                                                       context.GetText() + " " + obtenerCoordenadas(context.Start));
                    errorBuilder.AddError("Error en visit AssignStatementAST, faltan parametros: " +
                                          context.GetText() + " " + obtenerCoordenadas(context.Start));
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(
                    "Error en visit AssignStatementAST method validation, tipo recibido: " +
                    context.GetText() + " " + obtenerCoordenadas(context.Start));
                errorBuilder.AddError("Error en visit AssignStatementAST method validation, tipo recibido: " +
                                      context.GetText() + " " + obtenerCoordenadas(context.Start));
            }
        }
        else if
            (context.INC() !=
             null) // si es incremento ------------------------------------------------------------------------------------
        {
            if (tipoDesignator.ToLower() != "int")
            {
                System.Diagnostics.Debug.WriteLine("Error de asignacion: " + context.INC().GetText() +
                                                   " solo se puede incrementar " + '"' + "++" + '"' + " un entero"
                                                   + " " + obtenerCoordenadas(context.Start));
                errorBuilder.AddError("Error de asignacion: " + context.INC().GetText() +
                                      " solo se puede incrementar " + '"' + "++" + '"' + " un entero"
                                      + " " + obtenerCoordenadas(context.Start));
            }
        }
        else if
            (context.DEC() !=
             null) // si es decremento ------------------------------------------------------------------------------------
        {
            if (tipoDesignator.ToLower() != "int")
            {
                System.Diagnostics.Debug.WriteLine("Error de asignacion: " + context.DEC().GetText() +
                                                   " solo se puede decrementar " + '"' + "--" + '"' + " un entero"
                                                   + " " + obtenerCoordenadas(context.Start));
                errorBuilder.AddError("Error de asignacion: " + context.DEC().GetText() +
                                      " solo se puede decrementar " + '"' + "--" + '"' + " un entero"
                                      + " " + obtenerCoordenadas(context.Start));
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Error en visit AssignStatementAST, tipo recibido: " +
                                               context.GetText() + " " + obtenerCoordenadas(context.Start));
            errorBuilder.AddError("Error en visit AssignStatementAST, tipo recibido: " +
                                  context.GetText() + " " + obtenerCoordenadas(context.Start));
        }

        return null;
    }

    /*
    *  | IF LEFT_PAREN condition RIGHT_PAREN statement (ELSE statement)?    #IfStatementAST
    */
    public override object? VisitIfStatementAST(AlphaParser.IfStatementASTContext context)
    {
        tabla.OpenScope();
        bool conditionStatus = (bool)Visit(context.condition());
        if (conditionStatus)
        {
            Visit(context.statement(0));
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Error en visit IfStatementAST, condicion falsa: " +
                                               context.condition().GetText() + " " + obtenerCoordenadas(context.Start));
            errorBuilder.AddError("Error en visit IfStatementAST, condicion falsa: " +
                                  context.condition().GetText() + " " + obtenerCoordenadas(context.Start));
            if (context.statement(1) != null)
            {
                tabla.OpenScope();
                Visit(context.statement(1));
                tabla.CloseScope();
            }
        }

        tabla.CloseScope();
        return null;
    }

    /*
     * | FOR LEFT_PAREN expr SEMICOLON condition? SEMICOLON statement? RIGHT_PAREN statement #ForStatementAST 
     */
    public override object? VisitForStatementAST(AlphaParser.ForStatementASTContext context)
    {
        tabla.OpenScope();
        Visit(context.expr());
        if (context.condition() != null)
        {
            bool conditionStatus = (bool)Visit(context.condition());
            if (conditionStatus)
            {
                if (context.statement().Length > 1)
                {
                    Visit(context.statement(0));
                    Visit(context.statement(1));
                }
                else
                {
                    Visit(context.statement(0));
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error en visit ForStatementAST, condicion falsa: " +
                                                   context.GetText() + " " + obtenerCoordenadas(context.Start));
                errorBuilder.AddError("Error en visit ForStatementAST, condicion falsa: " +
                                      context.GetText() + " " + obtenerCoordenadas(context.Start));
            }

            tabla.CloseScope();
            return null;
        }

        if (context.statement().Length > 1)
        {
            Visit(context.statement(0));
            Visit(context.statement(1));
        }
        else
        {
            Visit(context.statement(0));
        }

        tabla.CloseScope();
        return null;
    }

    /*
     * | WHILE LEFT_PAREN condition RIGHT_PAREN statement   #WhileStatementAST
     */
    public override object? VisitWhileStatementAST(AlphaParser.WhileStatementASTContext context)
    {
        tabla.OpenScope();
        bool condition = (bool)Visit(context.condition());
        if (condition)
        {
            Visit(context.statement());
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Error en visit WhileStatementAST, condicion falsa: " +
                                               context.condition().GetText() + " " + obtenerCoordenadas(context.Start));
            errorBuilder.AddError("Error en visit WhileStatementAST, condicion falsa: " +
                                  context.condition().GetText() + " " + obtenerCoordenadas(context.Start));
        }

        tabla.CloseScope();
        return null;
    }

    /*
     *  | BREAK SEMICOLON #BreakStatementAST
     */
    public override object? VisitBreakStatementAST(AlphaParser.BreakStatementASTContext context)
    {
        return null;
    }

    /*
     * | RETURN expr? SEMICOLON #ReturnStatementAST
     */
    public override object? VisitReturnStatementAST(AlphaParser.ReturnStatementASTContext context)
    {
        tabla.currentMethod.returnStatement = (AlphaParser.ReturnStatementASTContext)context;
        return null;
    }

    /*
     * | READ LEFT_PAREN designator RIGHT_PAREN SEMICOLON #ReadStatementAST
     */
    public override object? VisitReadStatementAST(AlphaParser.ReadStatementASTContext context)
    {
        Visit(context.designator());
        return null;
    }

    /*
     * | WRITE LEFT_PAREN expr (COMMA (NUMBER|STRING_CONST))? RIGHT_PAREN SEMICOLON          #WriteStatementAST
     */
    public override object? VisitWriteStatementAST(AlphaParser.WriteStatementASTContext context)
    {
        foreach (var child in context.children)
        {
            Visit(child);
        }

        return null;
    }

    /*
     * | block #BlockStatementAST
     */
    public override object? VisitBlockStatementAST(AlphaParser.BlockStatementASTContext context)
    {
        Visit(context.block());
        return null;
    }

    /*
     | SEMICOLON  #SemicolonStatementAST
     */
    public override object VisitSemicolonStatementAST(AlphaParser.SemicolonStatementASTContext context)
    {
        return null;
    }

    /*
     * block : LEFT_BRACE (varDecl | statement)* RIGHT_BRACE #BlockAST;
     */
    public override object? VisitBlockAST(AlphaParser.BlockASTContext context)
    {
        foreach (var child in context.children)
        {
            if (child.Equals(context.LEFT_BRACE()) || child.Equals(context.RIGHT_BRACE()))
            {
                continue;
            }

            if (child is AlphaParser.StatementContext)
            {
                Visit(child);
            }
            else if (child is AlphaParser.VarDeclASTContext)
            {
                Visit(child);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERROR: Se esperaba un statement o una declaracion de variable: " +
                                                   child.GetText() + " " + obtenerCoordenadas(context.Start));
                errorBuilder.AddError("ERROR: Se esperaba un statement o una declaracion de variable: " +
                                      child.GetText() + " " + obtenerCoordenadas(context.Start));
            }
        }

        return null;
    }

    /*
     * actPars : expr (COMMA expr)* #ActParsAST;
     */
    public override object? VisitActParsAST(AlphaParser.ActParsASTContext context)
    {
        LinkedList<TypeD> tipos = new LinkedList<TypeD>();
        foreach (var child in context.expr())
        {
            string tipoB = (string)Visit(child);

            TypeD? tipo = tabla.Buscar(child.GetText());
            if (tipo != null)
            {
                tipos.AddLast(tipo);
            }
            else
            {
                if (tipoB != null)
                    tipos.AddLast(new BasicTypeD(child.Start, BasicTypeD.showType(tipoB.ToLower()),
                        TablaSimbolos.nivelActual, context));
            }
        }

        return tipos;
    }

    /*
     * condition : condTerm (LOGICAL_OR condTerm)*  #ConditionAST;
     */
    public override object VisitConditionAST(AlphaParser.ConditionASTContext context)
    {
        foreach (var child in context.condTerm())
        {
            bool comparationStatus = (bool)Visit(child);

            if (comparationStatus == true)
            {
                return true;
            }
        }

        System.Diagnostics.Debug.WriteLine("ERROR condition: La condicion no se cumple, no se puede continuar" +
                                           obtenerCoordenadas(context.Start));
        errorBuilder.AddError("ERROR condition: La condicion no se cumple, no se puede continuar" +
                              obtenerCoordenadas(context.Start));
        return false;
    }

    /*
     * condTerm : condFact (LOGICAL_AND condFact)*  #CondTermAST;
     */
    public override object? VisitCondTermAST(AlphaParser.CondTermASTContext context)
    {
        foreach (var child in context.condFact())
        {
            bool comparationStatus = (bool)Visit(child);
            if (comparationStatus == false)
            {
                System.Diagnostics.Debug.WriteLine("ERROR condTerm: La condicion no se cumple, no se puede continuar" +
                                                   obtenerCoordenadas(context.Start));
                errorBuilder.AddError("ERROR condTerm: La condicion no se cumple, no se puede continuar" +
                                      obtenerCoordenadas(context.Start));
                return false;
            }
        }

        return true;
    }

    /*
     * condFact : expr relop expr #CondFactAST;
     */
    public override object? VisitCondFactAST(AlphaParser.CondFactASTContext context)
    {
        string tipoA = (string)Visit(context.expr(0));
        Visit(context.relop());
        string tipoB = (string)Visit(context.expr(1));

        if (tipoA == null || tipoB == null)
        {
            System.Diagnostics.Debug.WriteLine("ERROR condFact: No se puede comparar un valor nulo" +
                                               obtenerCoordenadas(context.Start));
            errorBuilder.AddError("ERROR condFact: No se puede comparar un valor nulo" +
                                  obtenerCoordenadas(context.Start));
            return false;
        }

        if (tipoA.ToLower() == tipoB.ToLower())
        {
            return true;
        }

        System.Diagnostics.Debug.WriteLine("ERROR condFact: No se puede comparar tipos diferentes"
                                           + " tipoA: " + tipoA + " tipoB: " + tipoB +
                                           obtenerCoordenadas(context.Start));
        errorBuilder.AddError("ERROR condFact: No se puede comparar tipos diferentes"
                              + " tipoA:" + tipoA + " tipoB:" + tipoB + obtenerCoordenadas(context.Start));
        return false;
    }

    /*
     * cast : LEFT_PAREN type RIGHT_PAREN  #CastAST;
     */
    public override object? VisitCastAST(AlphaParser.CastASTContext context)
    {
        IToken tipo = (IToken)Visit(context.type());

        if (tipo.Text == null)
        {
            System.Diagnostics.Debug.WriteLine("ERROR cast: No se puede castear un valor nulo" +
                                               obtenerCoordenadas(context.Start));
            errorBuilder.AddError("ERROR cast: No se puede castear un valor nulo" +
                                  obtenerCoordenadas(context.Start));
        }

        return tipo.Text;
    }

    /*
     * expr : MINUSEXP? cast? term (addop term)* #ExprAST;
     */
    public override object VisitExprAST(AlphaParser.ExprASTContext context)
    {
        if (context.MINUSEXP() != null)
        {
            Visit(context.MINUSEXP());
        }

        if (context.cast() != null)
        {
            string tipoCast = (string)Visit(context.cast());
            return tipoCast;
        }

        string tipo = (string)Visit(context.term(0));
        if (tipo == null)
        {
            System.Diagnostics.Debug.WriteLine("Error de tipos en la expresion, la expresion es nula" +
                                               obtenerCoordenadas(context.Start));
            errorBuilder.AddError("Error de tipos en la expresion, la expresion es nula" +
                                  obtenerCoordenadas(context.Start));
            return null;
        }

        for (int i = 1; i < context.term().Length; i++)
        {
            Visit(context.addop(i - 1));
            string tipoLista = (string)Visit(context.term(i));
            if (tipoLista != tipo)
            {
                System.Diagnostics.Debug.WriteLine(
                    "Error de tipos en la expresion " + obtenerCoordenadas(context.Start));
                errorBuilder.AddError("Error de tipos en la expresion " + obtenerCoordenadas(context.Start));
                return null;
            }
        }

        return tipo;
    }

    /*
     * term : factor (mulop factor)*  #TermAST;
     */
    public override object? VisitTermAST(AlphaParser.TermASTContext context)
    {
        string tipo = (string)Visit(context.factor(0)); // tipo del primer factor
        //TODO verificar que el tipo del factor sea el correcto, ¿DEBERIA SER ENTERO?
        if (context.factor().Length > 1)
        {
            for (int i = 1; i < context.factor().Length; i++)
            {
                Visit(context.mulop(i - 1));
                string tipoLista = (string)Visit(context.factor(i));
                if (tipo != tipoLista)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: TIPOS DIFERENTES EN LA EXPRESION " +
                                                       obtenerCoordenadas(context.Start));
                    errorBuilder.AddError("ERROR: TIPOS DIFERENTES EN LA EXPRESION " +
                                          obtenerCoordenadas(context.Start));
                    return null;
                }
            }
        }
        return tipo;
    }

    /*
     * factor : designator (LEFT_PAREN actPars? RIGHT_PAREN)? #DesignatorFactorAST
     * cuando es un designator factor, se debe de verificar que el designator sea un metodo
     * y que el metodo tenga la cantidad de parametros correctos
     * y que el tipo de los parametros sea el correcto
     */
    public override object? VisitDesignatorFactorAST(AlphaParser.DesignatorFactorASTContext context)
    {
        string tipo = (string)Visit(context.designator());

        if (context.LEFT_PAREN() != null)
        {
            if (context.actPars() != null)
            {
                LinkedList<TypeD?> tipos = (LinkedList<TypeD>)Visit(context.actPars());
                TypeD? metodo = (TypeD)tabla.Buscar(context.designator().GetText());
                if (metodo is MethodTypeD)
                {
                    if (((MethodTypeD)metodo).cantParams == tipos.Count)
                    {
                        // verificar que los tipos de los parametros sean los correctos
                        for (int i = 0; i < tipos.Count; i++)
                        {
                            if (((MethodTypeD)metodo).paramsTypes.ElementAt(i).getType() != tipos.ElementAt(i).getType())
                            {
                                System.Diagnostics.Debug.WriteLine("ERROR: TIPOS DE PARAMETROS INCORRECTOS:"
                                                                   + ((MethodTypeD)metodo).paramsTypes.ElementAt(i)
                                                                   .getType()
                                                                   + " != " + tipos.ElementAt(i).getType() + " " +
                                                                   obtenerCoordenadas(context.Start));
                                errorBuilder.AddError("ERROR: TIPOS DE PARAMETROS INCORRECTOS:"
                                                      + ((MethodTypeD)metodo).paramsTypes.ElementAt(i).getType()
                                                      + " != " + tipos.ElementAt(i).getType() + " " +
                                                      obtenerCoordenadas(context.Start));
                                return null;
                            }
                        }

                        return ((MethodTypeD)metodo).returnType;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: CANTIDAD DE PARAMETROS INCORRECTA" +
                                                           obtenerCoordenadas(context.Start));
                        errorBuilder.AddError("ERROR: CANTIDAD DE PARAMETROS INCORRECTA" +
                                              obtenerCoordenadas(context.Start));
                        return null;
                    }
                }
                else if (context.designator().GetText().Equals("len"))
                {
                    return "Int";
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: NO SE ENCONTRO EL METODO" +
                                                       obtenerCoordenadas(context.Start));
                    errorBuilder.AddError("ERROR: NO SE ENCONTRO EL METODO" +
                                          obtenerCoordenadas(context.Start));
                    return null;
                }
            }
            else
            {
                TypeD? metodo = (TypeD)tabla.Buscar(context.designator().GetText());
                if (metodo is MethodTypeD)
                {
                    return ((MethodTypeD)metodo).returnType;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: NO SE ENCONTRO EL METODO SIN PARAMENTROS" +
                                                       obtenerCoordenadas(context.Start));
                    errorBuilder.AddError("ERROR: NO SE ENCONTRO EL METODO SIN PARAMENTROS" +
                                          obtenerCoordenadas(context.Start));
                    return null;
                }
            }
        }
        else
        {
            // TODO: verificar que el designator sea un metodo
            return tipo;
        }

        return null;
    }


/*
 * | CHAR_CONST  #CharFactorAST
 */
    public override object? VisitCharFactorAST(AlphaParser.CharFactorASTContext context)
    {
        return "Char";
    }

/*
 * | STRING_CONST #StringFactorAST
 */
    public override object? VisitStringFactorAST(AlphaParser.StringFactorASTContext context)
    {
        return "String";
    }

/*
 * | INT_CONST #IntFactorAST 
 */
    public override object? VisitIntFactorAST(AlphaParser.IntFactorASTContext context)
    {
        return "Int";
    }

/*
 * | DOUBLE_CONST #DoubleFactorAST
 */
    public override object? VisitDoubleFactorAST(AlphaParser.DoubleFactorASTContext context)
    {
        return "Double";
    }

/*
 * | BOOL_CONST  #BoolFactorAST
 */
    public override object? VisitBoolFactorAST(AlphaParser.BoolFactorASTContext context)
    {
        return "Boolean";
    }

/*
 * | NEW type #NewFactorAST
 */
    public override object? VisitNewFactorAST(AlphaParser.NewFactorASTContext context)
    {
        IToken ident = (IToken)Visit(context.type());
        TypeD? tipo = tabla.Buscar(ident.Text);
        if (tipo is ClassTypeD)
        {
            return tipo.getType();
        }

        ArrayTypeD.Types? typo = ArrayTypeD.showType(ident.Text);
        if (typo != ArrayTypeD.Types.Error)
        {
            return typo.ToString();
        }

        System.Diagnostics.Debug.WriteLine("ERROR: Tipo en el NEW no encontrado o no es un tipo valido" +
                                           obtenerCoordenadas(context.Start));
        errorBuilder.AddError("ERROR: Tipo en el NEW no encontrado o no es un tipo valido" +
                              obtenerCoordenadas(context.Start));
        return null;
    }

/*
 * | LEFT_PAREN expr RIGHT_PAREN #ParenFactorAST;
 */
    public override object? VisitParenFactorAST(AlphaParser.ParenFactorASTContext context)
    {
        string tipo = (string)Visit(context.expr());
        if (tipo != null) return tipo;
        return null;
    }

/*
 * designator : ident (DOT ident | LEFT_BRACKET expr RIGHT_BRACKET)*   #DesignatorAST;
 * Clase.array[1] = 1;
 */
    public override object? VisitDesignatorAST(AlphaParser.DesignatorASTContext context)
    {
        IToken ident = (IToken)Visit(context.ident(0));
        TypeD? tipo = tabla.Buscar(ident.Text);

        if (context.ident().Length > 1) // Cuando es un atributo de una clase
        {
            TypeD? tipo1 = tabla.BuscarCustomVar(ident.Text);

            if (tipo1 != null)
            {
                CustomTypeD tipoCustom = (CustomTypeD)tipo1;
                ClassTypeD classTypeD = (ClassTypeD)tabla.Buscar(tipoCustom.TypeOf);
                if (classTypeD != null)
                {
                    // cast to classtype

                    foreach (var data in classTypeD.attributes)
                    {
                        if (data.token.Text.Equals(context.ident(1).GetText()))
                        {
                            return data.getType();
                        }
                    }

                    System.Diagnostics.Debug.WriteLine(" No se encontro en dicha clase la variable " +
                                                       context.ident(1).GetText() + " " +
                                                       obtenerCoordenadas(context.Start));
                    errorBuilder.AddError(" No se encontro en dicha clase la variable " +
                                          context.ident(1).GetText() + " " + obtenerCoordenadas(context.Start));
                    return null;
                }
            }

            System.Diagnostics.Debug.WriteLine(" No se encontro en dicha clase " +
                                               context.ident(context.ident().Length - 2).GetText() + " " +
                                               obtenerCoordenadas(context.Start));
            errorBuilder.AddError(" No se encontro en dicha clase " +
                                  context.ident(context.ident().Length - 2).GetText() + " " +
                                  obtenerCoordenadas(context.Start));
            return null;
        }

        if (tipo is ArrayTypeD && context.expr().Length == 1) // cuando es un arreglo
        {

            string tipoExp = (string)Visit(context.expr(0));
            if (tipoExp != null)
            {
                if (tipoExp.Equals("Int"))
                {
                    return tipo.getType();
                }

                System.Diagnostics.Debug.WriteLine("ERROR: indice incorrecto: " + context.ident(0).GetText() +
                                                   obtenerCoordenadas(context.Start));
                errorBuilder.AddError("ERROR: indice incorrecto: " + context.ident(0).GetText() +
                                      obtenerCoordenadas(context.Start));
            }

            return null;
        }

        if (context.ident().Length == 1) // solo hay un id 
        {
            if (ident.Text.Equals("del"))
            {
                return "Boolean";
            }

            if (ident.Text.Equals("add"))
            {
                return "Boolean";
            }

            if (ident.Text.Equals("len"))
            {
                return "Int";
            }

            if (tipo is ArrayTypeD)
            {
                return tipo.getType();
            }

            if (tipo != null)
            {
                return tipo.getType();
            }

            System.Diagnostics.Debug.WriteLine(
                "No se encontro el siguente identificador: " + context.ident(0).GetText() + " " +
                obtenerCoordenadas(context.Start));
            errorBuilder.AddError(
                "No se encontro el siguente identificador: " + context.ident(0).GetText() + " " +
                obtenerCoordenadas(context.Start));
            return null;
        }

        if (tipo == null)
        {
            System.Diagnostics.Debug.WriteLine("ERROR: NO SE ENCONTRO LA VARIABLE " +
                                               context.ident(0).GetText() + " " +
                                               obtenerCoordenadas(context.Start));
            errorBuilder.AddError("ERROR: NO SE ENCONTRO LA VARIABLE " +
                                  context.ident(0).GetText() + " " +
                                  obtenerCoordenadas(context.Start));
            return null;
        }

        return null;
    }

/*
 * relop : EQUALS 
    | NOT_EQUALS 
    | GREATER_THAN 
    | GREATER_OR_EQUALS 
    | LESS_THAN 
    | LESS_OR_EQUALS;
 */
    public override object? VisitRelop(AlphaParser.RelopContext context)
    {
        return context.GetText();
    }

/*
 * addop : PLUS | MINUSEXP;
 */
    public override object? VisitAddop(AlphaParser.AddopContext context)
    {
        return context.GetText();
    }

/*
 * mulop : MULT | DIV | MOD;
 */
    public override object? VisitMulop(AlphaParser.MulopContext context)
    {
        return context.GetText();
    }

/*
 * ident : IDENTIFIER #IdentAST;
 */
    public override object VisitIdentAST(AlphaParser.IdentASTContext context)
    {
        //System.Diagnostics.Debug.WriteLine("visit IdentAST :" + context.IDENTIFIER().Symbol.Text);

        return context.IDENTIFIER().Symbol;
    }

/*
 * array : LEFT_BRACKET IDENTIFIER? RIGHT_BRACKET  #ArrayAST;
 */
    public override object? VisitArrayAST(AlphaParser.ArrayASTContext context)
    {
        return null;
    }
}