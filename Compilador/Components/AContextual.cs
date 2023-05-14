using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using generated;
using Compilador.Components.TypesManager;
using Type = Compilador.Components.TypesManager.Type;

namespace Compilador.Components;

public class AContextual : AlphaParserBaseVisitor<object>
{
    public TablaSimbolos tabla; // tabla de simbolos global


    public AContextual()
    {
        tabla = new TablaSimbolos();
    }

    public bool isMultitype(string op)
    {
        return op switch
        {
            "==" => true,
            "!=" => true,
            _ => false
        };
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
            ClassType cls = new ClassType(Tok, TablaSimbolos.nivelActual);
            tabla.Insertar(cls);
            foreach (var child in context.children)
            {
                Visit(child);
            }

            tabla.CloseScope();
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("Error en visit ProgramClassAST" + e.Message);
            throw;
        }

        tabla.Imprimir();
        return null;
    }

    /*
    // using : USING ident SEMICOLON    #UsingClassAST;
    */
    public override object? VisitUsingClassAST(AlphaParser.UsingClassASTContext context)
    {
        //TODO : implementar using para insertar en la tabla de simbolos
        return null;
    }

    /*
    // varDecl : type ident (COMMA ident)* SEMICOLON    #VarDeclAST;
        int  carro, moto, avion;
        int[] x, y, z;
        int x;
        Clase x;
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

            if (BasicType.isBasicType(context.type().GetText()) == true) // si es tipo basico
            {
                foreach (var child in context.ident())
                {
                    IToken tok = (IToken)Visit(child);
                    BasicType.Types varTipo = BasicType.showType(context.type().GetText());
                    BasicType var = new BasicType(tok, varTipo, TablaSimbolos.nivelActual);
                    if (tabla.currentClass != null) // es atributo de clase
                    {
                        if (!tabla.currentClass.BuscarAtributo(var.token.Text))
                        {
                            tabla.Insertar(var);
                            tabla.currentClass.attributes.AddLast(var);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, atributo ya existe: " +
                                                               var.token.Text);
                        }
                    }
                    else if (tabla.currentMethod != null) // es variable local de metodo
                    {
                        Type? tipo = tabla.Buscar(tok.Text);
                        if (tipo != null && tipo.nivel == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, variable ya existe: " +
                                                               tok.Text);
                        }
                        else
                        {
                            tabla.Insertar(var);
                        }
                    }
                    else if (tabla.currentClass == null && tabla.currentMethod == null) // es variable global
                    {
                        tabla.Insertar(var);
                    }
                }
            }
            else if (ultimasDosPosiciones.Equals("[]")) // si es arreglo
            {
                ArrayType.Types type = ArrayType.showType(textoSinUltimosDosCaracteres);

                if (type is ArrayType.Types.Error)
                {
                    System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, tipo recibido: " +
                                                       context.type().GetText());
                }
                else
                {
                    foreach (var child in context.ident())
                    {
                        if (tabla.currentClass != null)
                        {
                            System.Diagnostics.Debug.WriteLine("la clase solo puede tener variables de tipo basico.");
                        }
                        else if (tabla.currentMethod != null)
                        {
                            IToken tok = (IToken)Visit(child);
                            Type? tipo = tabla.Buscar(tok.Text);
                            if (tipo != null && tipo.nivel <= TablaSimbolos.nivelActual)
                            {
                                System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, variable ya existe: " +
                                                                   tok.Text);
                            }
                            else
                            {
                                ArrayType arr = new ArrayType(tok, TablaSimbolos.nivelActual, type);
                                tabla.Insertar(arr);
                            }
                        }
                        else if (tabla.currentClass == null && tabla.currentMethod == null) // es variable global
                        {
                            IToken tok = (IToken)Visit(child);
                            Type? tipo = tabla.Buscar(tok.Text);
                            if (tipo != null)
                            {
                                System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, variable ya existe: " +
                                                                   tok.Text);
                            }
                            else
                            {
                                ArrayType arr = new ArrayType(tok, TablaSimbolos.nivelActual, type);
                                tabla.Insertar(arr);
                            }
                        }
                    }
                }
            }
            else if (tabla.Buscar(context.type().GetText()) != null &&
                     tabla.Buscar(context.type().GetText()) is ClassType) // si es tipo compuesto
            {
                foreach (var child in context.ident())
                {
                    IToken tok = (IToken)Visit(child);
                    if (tabla.currentClass != null)  // si estoy en una clase
                    {
                        System.Diagnostics.Debug.WriteLine("la clase solo puede tener variables de tipo basico.");
                    }
                    else if (tabla.currentMethod != null) // si estoy en un metodo
                    {
                        Type? tipo = tabla.Buscar(tok.Text);
                        if (tipo != null && tipo.nivel <= TablaSimbolos.nivelActual)
                        {
                            System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, variable ya existe: " +
                                                               tok.Text);
                        }
                        else
                        {
                            CustomType var = new CustomType(tok, TablaSimbolos.nivelActual, context.type().GetText());
                            tabla.Insertar(var);
                        }
                    }
                    else if (tabla.currentClass == null && tabla.currentMethod == null) // es variable global
                    {
                        Type? tipo = tabla.Buscar(tok.Text);
                        if (tipo != null)
                        {
                            System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, variable ya existe: " +
                                                               tok.Text);
                        }
                        else
                        {
                            CustomType var = new CustomType(tok, TablaSimbolos.nivelActual, context.type().GetText());
                            tabla.Insertar(var);
                        }
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, tipo recibido: " +
                                                   context.type().GetText());
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST" + e.Message);
            //throw;
        }

        return null;
    }

    /*
     classDecl : CLASS ident LEFT_BRACE (varDecl)* RIGHT_BRACE  #ClassDeclAST;     
     */
    public override object? VisitClassDeclAST(AlphaParser.ClassDeclASTContext context)
    {
        if (tabla.Buscar(context.ident().GetText()) != null)
        {
            System.Diagnostics.Debug.WriteLine("Error en visit ClassDeclAST, clase ya existe: " +
                                               context.ident().GetText());
            return null;
        }
        
        IToken tok = (IToken)Visit(context.ident());
        ClassType cls = new ClassType(tok, TablaSimbolos.nivelActual);
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
        Type tipo = tabla.Buscar(tok.Text) as Type;

        if (tabla.Buscar(tok.Text) != null)
        {
            System.Diagnostics.Debug.WriteLine("Error en visit MethodDeclAST ya existe un objeto llamado: "
                                               + context.ident().GetText() + " de tipo: " + tipo.GetType());
            return null;
        }

        LinkedList<Type> parametros = new LinkedList<Type>();

        if (context.formPars() != null)
        {
            tabla.OpenScope();
            parametros = (LinkedList<Type>)Visit(context.formPars());
            tabla.CloseScope();
        }

        if (context.type() != null) // Si tiene tipo 
        {
            if (BasicType.isBasicType(context.type().GetText()) == true) // si es tipo basico
            {
                if (BasicType.showType(context.type().GetText()) != BasicType.Types.Error)
                {
                    MethodType method = new MethodType(tok, TablaSimbolos.nivelActual, parametros.Count,
                        context.type().GetText(), parametros);
                    tabla.Insertar(method);
                    tabla.currentMethod = method;
                }
            }
            else if (tabla.Buscar(context.type().GetText()) != null &&
                     tabla.Buscar(context.type().GetText()) is ClassType) // si es tipo compuesto
            {
                MethodType method = new MethodType(tok, TablaSimbolos.nivelActual, parametros.Count,
                    context.type().GetText(), parametros);
                tabla.Insertar(method);
                tabla.currentMethod = method;
            }
            else // error
            {
                System.Diagnostics.Debug.WriteLine("Error en visit MethodDeclAST, tipo recibido: " +
                                                   context.type().GetText());
            }
        }
        else if (context.VOID() != null) // si no tiene tipo, es void
        {
            MethodType method = new MethodType(tok, TablaSimbolos.nivelActual, parametros.Count, "void", parametros);
            tabla.Insertar(method);
            tabla.currentMethod = method;
        }
        else // error
        {
            System.Diagnostics.Debug.WriteLine("Error en visit MethodDeclAST, tipo recibido: " +
                                               context.type().GetText());
        }


        foreach (var child in parametros)
        {
            tabla.Insertar(child);
        }

        tabla.OpenScope();
        Visit(context.block());
        tabla.Sacar(tok.Text);
        tabla.CloseScope();
        tabla.currentMethod = null;
        return null;
    }

    /*
     * formPars : type ident (COMMA type ident)*    #FormParsAST;
     *  int x, char y, int z
     *  int x, char y, int z[]
     */
    public override LinkedList<Type>? VisitFormParsAST(AlphaParser.FormParsASTContext context)
    {
        LinkedList<Type> parametros = new LinkedList<Type>();

        for (int i = 0; i < context.ident().Length; i++)
        {
            var ultimasDosPosiciones =
                context.type(i).GetText()
                    .Substring(context.type(i).GetText().Length - 2); // [] para saber si es arreglo
            var textoSinUltimosDosCaracteres =
                context.type(i).GetText().Substring(0, context.type(i).GetText().Length - 2);

            IToken tok = (IToken)Visit(context.ident(i));
            if (BasicType.isBasicType(context.type(i).GetText())) // si es tipo basico
            {
                BasicType.Types varTipo = BasicType.showType(context.type(i).GetText());
                BasicType var = new BasicType(tok, varTipo, TablaSimbolos.nivelActual);
                parametros.AddLast(var);
            }
            else if (tabla.Buscar(context.type(i).GetText()) != null &&
                     tabla.Buscar(context.type(i).GetText()) is ClassType) // si es tipo compuesto
            {
                CustomType var = new CustomType(tok, TablaSimbolos.nivelActual, context.type(i).GetText());
                parametros.AddLast(var);
            }
            else if (ultimasDosPosiciones.Equals("[]")) // si es arreglo
            {
                ArrayType.Types type = ArrayType.showType(textoSinUltimosDosCaracteres);

                if (type is ArrayType.Types.Error)
                {
                    System.Diagnostics.Debug.WriteLine("Error en visit formPars, tipo recibido: " +
                                                       context.type(i).GetText());
                }
                else if (type is ArrayType.Types.Int or ArrayType.Types.Char)
                {
                    ArrayType arr = new ArrayType(tok, TablaSimbolos.nivelActual, type);
                    parametros.AddLast(arr);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error en visit FormParsAST, tipo recibido: " +
                                                   context.type(i).GetText());
            }
        }

        return parametros;
    }

    /*
     * type : ident array?  #TypeAST; 
     */
    public override object? VisitTypeAST(AlphaParser.TypeASTContext context)
    {
        IToken ident = (IToken)Visit(context.ident());

        if (context.array() != null)
        {
            Visit(context.array());
        }

        return ident;
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
                                                           tipoExp);
                    }

                    return null;
                }

                if (!String.Equals(tipoDesignator, tipoExp, StringComparison.CurrentCultureIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine("Error de tipo en asignacion: " + tipoDesignator +
                                                       " diferente a " +
                                                       tipoExp);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error de tipo en asignacion nula");
            }
        }
        else if
            (context.LEFT_PAREN() !=
             null) // si es una llamada a metodo ------------------------------------------------------------------------------------
        {
            Type? type = tabla.Buscar(context.designator().GetText());

            if (context.designator().GetText() ==
                "del") // si es del ------------------------------------------------------------------------------------
            {
                //del(arreglo, 1);
                // TODO: acceder al tipo de la variable y ver si es int y disminuir su valor
                if (context.actPars() == null)
                {
                    System.Diagnostics.Debug.WriteLine("Error metodo  del sin parametros");
                }
                else
                {
                    // lista de parametros del metodo
                    LinkedList<Type> actParm = (LinkedList<Type>)Visit(context.actPars());
                    if (actParm.Count != 2)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            "Error de parametros, cantidad de parametros no coinciden en el del");
                    }
                    else
                    {
                        if (actParm.ElementAt(0) is ArrayType &&
                            actParm.ElementAt(1).getType().Equals("Int"))
                        {
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Error de parametros, tipos de parametros no coinciden en el del");
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
                    System.Diagnostics.Debug.WriteLine("Error metodo len sin parametros");
                }
                else
                {
                    // lista de parametros del metodo
                    LinkedList<Type> actParm = (LinkedList<Type>)Visit(context.actPars());
                    if (actParm.Count != 1)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            "Error de parametros, cantidad de parametros no coinciden en el len");
                    }
                    else
                    {
                        if (actParm.ElementAt(0) is ArrayType)
                        {
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Error de parametros, tipos de parametros no coinciden en el len");
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
                    System.Diagnostics.Debug.WriteLine("Error metodo add sin parametros");
                }
                else
                {
                    // lista de parametros del metodo
                    LinkedList<Type> actParm = (LinkedList<Type>)Visit(context.actPars());
                    if (actParm.Count != 2)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            "Error de parametros, cantidad de parametros no coinciden en el add");
                    }
                    else
                    {
                        if (actParm.ElementAt(0).getType().Equals(actParm.ElementAt(1).getType()))
                        {
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Error de parametros, tipos de parametros no coinciden en el add");
                        }
                    }
                }
            }
            else if
                (type is MethodType method) // si es un metodo ------------------------------------------------------------------------------------
            {
                // TODO: Verificar que el metodo exista y que los parametros sean correctos
                if (context.actPars() != null)
                {
                    LinkedList<Type> actParm = new LinkedList<Type>();
                    actParm = (LinkedList<Type>)Visit(context.actPars());

                    if (actParm.Count == method.cantParams)
                    {
                        for (int i = 0; i < method.cantParams; i++)
                        {
                            if (method.paramsTypes.ElementAt(i).getType() != actParm.ElementAt(i).getType())
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    "Error de parametros, tipos de parametros no coinciden");
                            }
                        }
                    }
                    else if (actParm.Count < method.cantParams)
                    {
                        System.Diagnostics.Debug.WriteLine("Error de parametros, faltan parametros");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Error de parametros, sobran parametros");
                    }
                }
                else if (method.cantParams > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Error en visit AssignStatementAST, faltan parametros: " +
                                                       context.GetText());
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(
                    "Error en visit AssignStatementAST method validation, tipo recibido: " +
                    context.GetText());
            }
        }
        else if
            (context.INC() !=
             null) // si es un incremento o decremento ------------------------------------------------------------------------------------
        {
            // TODO: acceder al tipo de la variable y ver si es int y aumentar su valor 
        }
        else if (context.DEC() != null)
        {
            // TODO: acceder al tipo de la variable y ver si es int y disminuir su valor
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Error en visit AssignStatementAST, tipo recibido: " +
                                               context.GetText());
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
                                               context.condition().GetText());
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
                                                   context.GetText());
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
                                               context.condition().GetText());
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
        if (context.expr() != null)
        {
            string tipoReturn = (string)Visit(context.expr());
            if (tabla.currentMethod.returnType == "void")
            {
                System.Diagnostics.Debug.WriteLine(
                    "Error en visit ReturnStatementAST, metodos void no retornan datos: " +
                    context.expr().GetText());
            }
            else if (tipoReturn.ToLower() != tabla.currentMethod.returnType)
            {
                System.Diagnostics.Debug.WriteLine("Error en visit ReturnStatementAST, tipo de retorno incorrecto: " +
                                                   " El tipo de return del metodo es: " +
                                                   tabla.currentMethod.returnType +
                                                   " y el tipo de retorno del return es: " + tipoReturn);
            }
        }

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
                                                   child.GetText());
            }
        }

        return null;
    }

    /*
     * actPars : expr (COMMA expr)* #ActParsAST;
     */
    public override object? VisitActParsAST(AlphaParser.ActParsASTContext context)
    {
        LinkedList<Type> tipos = new LinkedList<Type>();
        foreach (var child in context.expr())
        {
            string tipoB = (string)Visit(child);

            Type? tipo = tabla.Buscar(child.GetText());
            if (tipo != null)
            {
                tipos.AddLast(tipo);
            }
            else
            {
                if (tipoB != null)
                    tipos.AddLast(new BasicType(child.Start, BasicType.showType(tipoB.ToLower()),
                        TablaSimbolos.nivelActual));
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

        System.Diagnostics.Debug.WriteLine("ERROR condition: La condicion no se cumple, no se puede continuar");
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
                System.Diagnostics.Debug.WriteLine("ERROR condTerm: La condicion no se cumple, no se puede continuar");
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
            System.Diagnostics.Debug.WriteLine("ERROR condFact: No se puede comparar un valor nulo");
            return false;
        }

        if (tipoA == tipoB)
        {
            return true;
        }

        System.Diagnostics.Debug.WriteLine("ERROR condFact: No se puede comparar tipos diferentes");
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
            System.Diagnostics.Debug.WriteLine("ERROR cast: No se puede castear un valor nulo");
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
            Console.WriteLine("Error de tipos en la expresion");
            return null;
        }

        for (int i = 1; i < context.term().Length; i++)
        {
            Visit(context.addop(i - 1));
            string tipoLista = (string)Visit(context.term(i));
            if (tipoLista != tipo)
            {
                Console.WriteLine("Error de tipos en la expresion");
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
                    System.Diagnostics.Debug.WriteLine("ERROR: TIPOS DIFERENTES EN LA EXPRESION");
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
            LinkedList<Type> tipos = (LinkedList<Type>)Visit(context.actPars());

            Type? metodo = (Type)tabla.Buscar(context.designator().GetText());
            if (metodo is MethodType)
            {
                if (((MethodType)metodo).cantParams == tipos.Count)
                {
                    // verificar que los tipos de los parametros sean los correctos
                    for (int i = 0; i < tipos.Count; i++)
                    {
                        if (((MethodType)metodo).paramsTypes.ElementAt(i).getType() != tipos.ElementAt(i).getType())
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: TIPOS DE PARAMETROS INCORRECTOS:"
                                                               + ((MethodType)metodo).paramsTypes.ElementAt(i).getType()
                                                               + " != " + tipos.ElementAt(i).getType());
                            return null;
                        }
                    }

                    return ((MethodType)metodo).returnType;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: CANTIDAD DE PARAMETROS INCORRECTA");
                    return null;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERROR: NO SE ENCONTRO EL METODO");
                return null;
            }
        }
        else
        {
            return tipo;
        }
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
        Type? tipo = tabla.Buscar(ident.Text);
        if (tipo is ClassType)
        {
            return tipo.getType();
        }

        ArrayType.Types? typo = ArrayType.showType(ident.Text);
        if (typo != ArrayType.Types.Error)
        {
            return typo.ToString();
        }

        System.Diagnostics.Debug.WriteLine("ERROR: Tipo en el NEW no encontrado o no es un tipo valido");
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
        //TODO validar el tipo de arreglo que sea correcto y q este en el alcance
        Type? tipo = tabla.Buscar(context.ident(0).GetText());

        if (context.ident().Length > 1) // Cuando es un atributo de una clase
        {
            Type? tipo1 = tabla.BuscarCustomVar(context.ident(0).GetText());

            if (tipo1 != null)
            {
                CustomType tipoCustom = (CustomType)tipo1;
                ClassType classType = (ClassType)tabla.Buscar(tipoCustom.TypeOf);
                if (classType != null)
                {
                    // cast to classtype

                    foreach (var data in classType.attributes)
                    {
                        if (data.token.Text.Equals(context.ident(1).GetText()))
                        {
                            return data.getType();
                        }
                    }

                    System.Diagnostics.Debug.WriteLine(" No se encontro en dicha clase la variable " +
                                                       context.ident(1).GetText());
                    return null;
                }
            }

            System.Diagnostics.Debug.WriteLine(" No se encontro en dicha clase " +
                                               context.ident(context.ident().Length - 2).GetText());
            return null;
        }

        if (tipo is ArrayType && context.expr().Length == 1) // cuando es un arreglo
        {
            string tipoExp = (string)Visit(context.expr(0));
            if (tipoExp != null)
            {
                if (tipoExp.Equals("Int"))
                {
                    return tipo.getType();
                }

                System.Diagnostics.Debug.WriteLine("ERROR: indice incorrecto: " + context.ident(0).GetText());
            }

            return null;
        }

        if (context.ident().Length == 1) // solo hay un id 
        {
            if (context.ident(0).GetText().Equals("del"))
            {
                return "Boolean";
            }

            if (context.ident(0).GetText().Equals("add"))
            {
                return "Boolean";
            }

            if (context.ident(0).GetText().Equals("len"))
            {
                return "Int";
            }

            if (tipo is ArrayType)
            {
                return tipo.getType() + "[]";
            }

            if (tipo != null)
            {
                return tipo.getType();
            }

            System.Diagnostics.Debug.WriteLine(
                "No se encontro el siguente identificador: " + context.ident(0).GetText());
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
    public override IToken VisitIdentAST(AlphaParser.IdentASTContext context)
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