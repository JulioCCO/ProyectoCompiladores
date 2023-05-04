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
            parametros = (LinkedList<Type>)Visit(context.formPars());
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
            else if (tabla.Buscar(context.type().GetText()) != null && tabla.Buscar(context.type().GetText()) is ClassType) // si es tipo compuesto
            {
                MethodType method = new MethodType(tok, TablaSimbolos.nivelActual, parametros.Count,
                    context.type().GetText(), parametros);
                tabla.Insertar(method);
            }
            else // error
            {
                System.Diagnostics.Debug.WriteLine("Error en visit MethodDeclAST, tipo recibido: " + context.type().GetText());
            }
        }
        else if (context.VOID() != null) // si no tiene tipo, es void
        {
            MethodType method = new MethodType(tok, TablaSimbolos.nivelActual, parametros.Count, "void", parametros);
            tabla.Insertar(method);
        }
        else // error
        {
            System.Diagnostics.Debug.WriteLine("Error en visit MethodDeclAST, tipo recibido: " + context.type().GetText());
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
        Visit(context.ident());
        if (context.array() != null)
        {
            Visit(context.array());
        }

        return null;
    }

    /*
        statement : designator (ASSIGN expr | LEFT_PAREN actPars? RIGHT_PAREN | INC | DEC) SEMICOLON    #AssignStatementAST    
     */
    public override object? VisitAssignStatementAST(AlphaParser.AssignStatementASTContext context)
    {
        Visit(context.designator());
        if (context.expr() != null)
        {
            Visit(context.expr());
        }
        else if (context.actPars() != null)
        {
            Visit(context.actPars());
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
        Visit(context.expr());
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
     * block : LEFT_BRACE (varDecl | statement)* RIGHT_BRACE #BlockAST;
     */
    public override object VisitBlockAST(AlphaParser.BlockASTContext context)
    {
        if (context.varDecl().Length > 0)
        {
            foreach (var child in context.varDecl())
            {
                Visit(child);
            }
        }

        if (context.statement().Length > 0)
        {
            foreach (var child in context.statement())
            {
                Visit(child);
            }
        }
        return null;
    }

    /*
     * actPars : expr (COMMA expr)* #ActParsAST;
     */
    public override object? VisitActParsAST(AlphaParser.ActParsASTContext context)
    {
        Visit(context.expr(0));
        if (context.expr().Length > 1)
        {
            for (int i = 1; i < context.expr().Length; i++)
            {
                Visit(context.expr(i));
            }
        }

        return null;
    }

    /*
     * condition : condTerm (LOGICAL_OR condTerm)*  #ConditionAST;
     */
    public override object VisitConditionAST(AlphaParser.ConditionASTContext context)
    {
        Visit(context.condTerm(0));
        if (context.condTerm().Length > 1)
        {
            for (int i = 1; i < context.condTerm().Length; i++)
            {
                Visit(context.condTerm(i));
            }
        }

        return base.VisitConditionAST(context);
    }

    /*
     * condTerm : condFact (LOGICAL_AND condFact)*  #CondTermAST;
     */
    public override object? VisitCondTermAST(AlphaParser.CondTermASTContext context)
    {
        Visit(context.condFact(0));
        if (context.condFact().Length > 1)
        {
            for (int i = 1; i < context.condFact().Length; i++)
            {
                Visit(context.condFact(i));
            }
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
        if (context.cast() != null)
        {
            Visit(context.cast());
        }

        Visit(context.term(0));

        if (context.term().Length > 1)
        {
            for (int i = 1; i < context.term().Length; i++)
            {
                Visit(context.addop(i - 1));
                Visit(context.term(i));
            }
        }

        return null;
    }

    /*
     * term : factor (mulop factor)*  #TermAST;
     */
    public override object? VisitTermAST(AlphaParser.TermASTContext context)
    {
        Visit(context.factor(0));
        if (context.factor().Length > 1)
        {
            for (int i = 1; i < context.factor().Length; i++)
            {
                Visit(context.mulop(i - 1));
                Visit(context.factor(i));
            }
        }

        return null;
    }

    /*
     * factor : designator (LEFT_PAREN actPars? RIGHT_PAREN)? #DesignatorFactorAST
     */
    public override object? VisitDesignatorFactorAST(AlphaParser.DesignatorFactorASTContext context)
    {
        Visit(context.designator());
        if (context.actPars() != null)
        {
            Visit(context.actPars());
        }

        return null;
    }

    /*
     * | CHAR_CONST  #CharFactorAST
     */
    public override object? VisitCharFactorAST(AlphaParser.CharFactorASTContext context)
    {
        return null;
    }

    /*
     * | STRING_CONST #StringFactorAST
     */
    public override object? VisitStringFactorAST(AlphaParser.StringFactorASTContext context)
    {
        return null;
    }

    /*
     * | INT_CONST #IntFactorAST 
     */
    public override object? VisitIntFactorAST(AlphaParser.IntFactorASTContext context)
    {
        return null;
    }

    /*
     * | DOUBLE_CONST #DoubleFactorAST
     */
    public override object? VisitDoubleFactorAST(AlphaParser.DoubleFactorASTContext context)
    {
        return null;
    }

    /*
     * | BOOL_CONST  #BoolFactorAST
     */
    public override object? VisitBoolFactorAST(AlphaParser.BoolFactorASTContext context)
    {
        return null;
    }

    /*
     * | NEW ident #NewFactorAST
     */
    public override object? VisitNewFactorAST(AlphaParser.NewFactorASTContext context)
    {
        Visit(context.ident());
        return null;
    }

    /*
     * | LEFT_PAREN expr RIGHT_PAREN #ParenFactorAST;
     */
    public override object? VisitParenFactorAST(AlphaParser.ParenFactorASTContext context)
    {
        Visit(context.expr());
        return null;
    }

    /*
     * designator : ident (DOT ident | LEFT_BRACKET expr RIGHT_BRACKET)*   #DesignatorAST;
     */
    public override object? VisitDesignatorAST(AlphaParser.DesignatorASTContext context)
    {
        Visit(context.ident(0));
        if (context.ident().Length > 1)
        {
            for (int i = 1; i < context.ident().Length; i++)
            {
                Visit(context.ident(i));
            }
        }
        else if (context.expr().Length > 0)
        {
            foreach (var child in context.expr())
            {
                Visit(child);
            }
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
        return null;
    }

    /*
     * addop : PLUS | MINUSEXP;
     */
    public override object? VisitAddop(AlphaParser.AddopContext context)
    {
        return null;
    }

    /*
     * mulop : MULT | DIV | MOD;
     */
    public override object? VisitMulop(AlphaParser.MulopContext context)
    {
        return null;
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