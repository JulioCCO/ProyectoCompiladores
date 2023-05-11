using System;
using System.Collections.Generic;
using Antlr4.Runtime;
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
            System.Diagnostics.Debug.WriteLine("visit ProgramClassAST");
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
            System.Diagnostics.Debug.WriteLine("visit VarDeclAST");
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
                    tabla.Insertar(var);
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
                        IToken tok = (IToken)Visit(child);
                        ArrayType arr = new ArrayType(tok, TablaSimbolos.nivelActual, type);
                        tabla.Insertar(arr);
                    }
                }
            }
            else if (tabla.Buscar(context.type().GetText()) != null &&
                     tabla.Buscar(context.type().GetText()) is ClassType) // si es tipo compuesto
            {
                foreach (var child in context.ident())
                {
                    IToken tok = (IToken)Visit(child);
                    CustomType var = new CustomType(tok, TablaSimbolos.nivelActual, context.type().GetText());
                    tabla.Insertar(var);
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
            throw;
        }

        return null;
    }

    /*
     classDecl : CLASS ident LEFT_BRACE (varDecl)* RIGHT_BRACE  #ClassDeclAST;     
     */
    public override object? VisitClassDeclAST(AlphaParser.ClassDeclASTContext context)
    {
        System.Diagnostics.Debug.WriteLine("visit ClassDeclAST: " + context.ident().GetText());

        IToken tok = (IToken)Visit(context.ident());
        ClassType cls = new ClassType(tok, TablaSimbolos.nivelActual);
        tabla.Insertar(cls);
        tabla.OpenScope();
        if (context.varDecl().Length > 0)
        {
            foreach (var child in context.varDecl())
            {
                Visit(child);
            }
        }

        tabla.CloseScope();
        return null;
    }

    /*
        methodDecl : (type | VOID) ident LEFT_PAREN formPars? RIGHT_PAREN block #MethodDeclAST;
     */
    public override object? VisitMethodDeclAST(AlphaParser.MethodDeclASTContext context)
    {
        System.Diagnostics.Debug.WriteLine("visit MethodDeclAST: " + context.ident().GetText());

        LinkedList<Type> parametros = new LinkedList<Type>();
        if (context.formPars() != null)
        {
            tabla.OpenScope();
            parametros = (LinkedList<Type>)Visit(context.formPars());
            tabla.CloseScope();
        }

        IToken tok = (IToken)Visit(context.ident());

        if (context.type() != null) // Si tiene tipo 
        {
            if (BasicType.isBasicType(context.type().GetText()) == true) // si es tipo basico
            {
                if (BasicType.showType(context.type().GetText()) != BasicType.Types.Error)
                {
                    MethodType method = new MethodType(tok, TablaSimbolos.nivelActual, parametros.Count,
                        context.type().GetText(), parametros);
                    tabla.Insertar(method);
                }
            }
            else if (tabla.Buscar(context.type().GetText()) != null &&
                     tabla.Buscar(context.type().GetText()) is ClassType) // si es tipo compuesto
            {
                MethodType method = new MethodType(tok, TablaSimbolos.nivelActual, parametros.Count,
                    context.type().GetText(), parametros);
                tabla.Insertar(method);
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
        }
        else // error
        {
            System.Diagnostics.Debug.WriteLine("Error en visit MethodDeclAST, tipo recibido: " +
                                               context.type().GetText());
        }

        tabla.OpenScope();
        Visit(context.block());
        tabla.CloseScope();
        return null;
    }

    /*
     * formPars : type ident (COMMA type ident)*    #FormParsAST;
     *  int x, char y, int z
     *  int x, char y, int z[]
     */
    public override LinkedList<Type>? VisitFormParsAST(AlphaParser.FormParsASTContext context)
    {
        System.Diagnostics.Debug.WriteLine("visit FormParsAST: " + context.GetText());
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
                    System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, tipo recibido: " +
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

        return ident.Text;
    }

    /*
        statement : designator (ASSIGN expr | LEFT_PAREN actPars? RIGHT_PAREN | INC | DEC) SEMICOLON    #AssignStatementAST    
     */
    public override object? VisitAssignStatementAST(AlphaParser.AssignStatementASTContext context)
    {

        string tipoDesignator = (string) Visit(context.designator());
        System.Diagnostics.Debug.WriteLine("visit AssignStatementAST designator: " + tipoDesignator);
        if (context.expr() != null) //  si es una asignacion 
        {
            string tipo = (string) Visit(context.expr());
            if (tipo != tipoDesignator)
            {
                System.Diagnostics.Debug.WriteLine("Error de tipo en asignacion" + tipo + " " + tipoDesignator);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Asignacion correcta " + tipo + " " + tipoDesignator);
                
            }
        }
        else if (context.LEFT_PAREN() != null) // si es una llamada a metodo
        {
            if (context.actPars() != null) Visit(context.actPars());
        }
        else if (context.INC() != null ) // si es un incremento o decremento
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
        Visit(context.condition());
        Visit(context.statement(0));
        tabla.CloseScope();

        if (context.statement(1) != null)
        {
            tabla.OpenScope();
            Visit(context.statement(1));
            tabla.CloseScope();
        }
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
            Visit(context.condition());
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
        Visit(context.condition());
        Visit(context.statement());
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
            Visit(context.expr());
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
            if (child is AlphaParser.StatementContext)
            {
                Visit(child);
            } else if (child is AlphaParser.VarDeclASTContext)
            {
                Visit(child);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error de sintaxis");
                //throw new SyntaxErrorException("Error de sintaxis");
            }
        }
        return null;
    }

    /*
     * actPars : expr (COMMA expr)* #ActParsAST;
     */
    public override object? VisitActParsAST(AlphaParser.ActParsASTContext context)
    {
        foreach (var child in context.children)
        {
            Visit(child);
        }
        return null;
    }

    /*
     * condition : condTerm (LOGICAL_OR condTerm)*  #ConditionAST;
     */
    public override object VisitConditionAST(AlphaParser.ConditionASTContext context)
    {
        foreach (var child in context.condTerm())
        {
            Visit(child);
        }
        return null;
    }

    /*
     * condTerm : condFact (LOGICAL_AND condFact)*  #CondTermAST;
     */
    public override object? VisitCondTermAST(AlphaParser.CondTermASTContext context)
    {
        foreach (var child in context.condFact())
        {
            Visit(child);
        }
        return null;
    }

    /*
     * condFact : expr relop expr #CondFactAST;
     */
    public override object? VisitCondFactAST(AlphaParser.CondFactASTContext context)
    {

        Visit(context.expr(0));
        Visit(context.relop());
        Visit(context.expr(1));
        return null;
    }

    /*
     * cast : LEFT_PAREN type RIGHT_PAREN  #CastAST;
     */
    public override object? VisitCastAST(AlphaParser.CastASTContext context)
    {
        Visit(context.type());
        return null;
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
            Visit(context.cast());
        }
        
        string tipo = (string) Visit(context.term(0));
        if(tipo == null)
        {
            Console.WriteLine("Error de tipos en la expresion");
            return null;
        }
        
        if (context.term().Length > 1)
        {
            for (int i = 1; i < context.term().Length; i++)
            {
                Visit(context.addop(i - 1));
                string tipoLista = (string) Visit(context.term(i));//2
                if (tipoLista != tipo)
                {
                    Console.WriteLine("Error de tipos en la expresion");
                    return null;
                }
            }
        }

        return tipo;
    }

    /*
     * term : factor (mulop factor)*  #TermAST;
     */
    public override object? VisitTermAST(AlphaParser.TermASTContext context)
    {
        string tipo = (string) Visit(context.factor(0)); // tipo del primer factor
        if (context.factor().Length > 1)
        {
            for (int i = 1; i < context.factor().Length; i++)
            {
                Visit(context.mulop(i - 1));
                string tipoLista = (string)  Visit(context.factor(i));
                if (tipo != tipoLista)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: TIPOS DIFERENTES");
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
        string tipo = (string) Visit(context.designator());
        if (context.LEFT_PAREN() != null)
        {
            if (context.actPars() != null)
            {
                Visit(context.actPars());
            }
        }
        if(tipo != null) return tipo;
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
     * | NEW ident #NewFactorAST
     */
    public override object? VisitNewFactorAST(AlphaParser.NewFactorASTContext context)
    {
        IToken ident = (IToken) Visit(context.ident());
        
        Type? tipo = tabla.Buscar(ident.Text); //msg[0] //msg
        
        if(tipo == null)
        {
            Console.WriteLine("Error de tipos en la expresion");
            return null;
        }
        return tipo.getType();
    }

    /*
     * | LEFT_PAREN expr RIGHT_PAREN #ParenFactorAST;
     */
    public override object? VisitParenFactorAST(AlphaParser.ParenFactorASTContext context)
    {
        string tipo = (string) Visit(context.expr());
        if (tipo != null) return tipo;
        return null;
    }
    
    /*
     * designator : ident (DOT ident | LEFT_BRACKET expr RIGHT_BRACKET)*   #DesignatorAST;
     */
    public override object? VisitDesignatorAST(AlphaParser.DesignatorASTContext context)
    {
        if(context.ident().Length > 1) 
        {
            // validar si el tipo es arreglo 
            
            System.Diagnostics.Debug.WriteLine(context.ident(0).GetText() + " DESIGNATOR mas de 1 id");
            CustomType tipo = (CustomType) tabla.Buscar(context.ident(context.ident().Length - 2).GetText());
            if (tipo != null)
            {
                ClassType classType = (ClassType)tabla.Buscar(tipo.TypeOf);
                if (classType != null)
                {
                    // cast to classtype
                    foreach (var data in  classType.attributes)
                    {

                        if (data.token.Text.Equals(context.ident(context.ident().Length - 1).GetText()))
                        {
                            System.Diagnostics.Debug.WriteLine(data.token.Text + "Se encontro en dicha clase");
                            return data.getType();
                        }
                    }
                    System.Diagnostics.Debug.WriteLine(" No se encontro en dicha clase");

                    return null;
                
                }
            }

            System.Diagnostics.Debug.WriteLine(" No se encontro en dicha clase " +
                                               context.ident(context.ident().Length - 2).GetText());
            return null;
        }
        
        if (context.ident().Length == 1) // solo hay un id
        {
            //enterito = 1
            Type tipo = tabla.Buscar(context.ident(0).GetText());
            System.Diagnostics.Debug.WriteLine(context.ident(0).GetText()+ " DESIGNATOR");
            if (tipo != null)
            {
                return tipo.getType();
            }
            System.Diagnostics.Debug.WriteLine( " No se encontro");
            return null;
        }
        if (context.expr() != null)
        {
            //int char //point no
            //msg[0] // msg[0][0] no
            Type tipo = tabla.Buscar(context.ident(0).GetText());
            if (tipo != null )
            {
                return tipo.getType();
            }

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