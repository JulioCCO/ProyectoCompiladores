using System;
using Antlr4.Runtime;
using generated;

namespace Compilador.Components;

public class AContextual : AlphaParserBaseVisitor<object>
{
    public TablaSimbolos tabla; // tabla de simbolos global
    public int nivelPropio = 0;

    public AContextual()
    {
        tabla = new TablaSimbolos();
    }

    private TablaSimbolos.BasicType showType(string type)
    {
        return type switch
        {
            "int" => TablaSimbolos.BasicType.Int,
            "double" => TablaSimbolos.BasicType.Double,
            "string" => TablaSimbolos.BasicType.String,
            "boolean" => TablaSimbolos.BasicType.Boolean,
            "char" => TablaSimbolos.BasicType.Char,
            "void" => TablaSimbolos.BasicType.Void,
            "null" => TablaSimbolos.BasicType.Null,
            "int[]" => TablaSimbolos.BasicType.Int,
            "char[]" => TablaSimbolos.BasicType.Char,
            _ => TablaSimbolos.BasicType.Error
        };
    }


    private string showToken(IToken token)
    {
        return token.Text + "Fila, columna: (" + token.Line + "," + token.Column + ")";
    }

    private TablaSimbolos.DataType isMethod(IToken token)
    {
        return tabla.Buscar(token).tipoDato;
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
            tabla.Insertar(Tok, TablaSimbolos.BasicType.Null, TablaSimbolos.DataType.Class, nivelPropio);
            System.Diagnostics.Debug.WriteLine("visit ProgramClassAST");
            if (context.@using().Length > 0)
            {
                foreach (var child in context.@using())
                {
                    Visit(child);
                    System.Diagnostics.Debug.WriteLine("visita using: " + child.GetText());
                }
            }

            if (context.varDecl().Length > 0)
            {
                int i = 0;
                foreach (var child in context.varDecl())
                {
                    Visit(child);
                    System.Diagnostics.Debug.WriteLine("visita varDecl " + i + ": " + child.GetText());
                    i++;
                }
            }

            if (context.classDecl().Length > 0)
            {
                int i = 0;
                foreach (var child in context.classDecl())
                {
                    Visit(child);
                    System.Diagnostics.Debug.WriteLine("visita classDecl " + i + ": " + child.GetText());
                    i++;
                }
            }

            if (context.methodDecl().Length > 0)
            {
                int i = 0;
                foreach (var child in context.methodDecl())
                {
                    Visit(child);
                    System.Diagnostics.Debug.WriteLine("visita methodDecl " + i + ": " + child.GetText());
                    i++;
                }
            }

            tabla.CloseScope();
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("Error en visit ProgramClassAST" + e.Message);
            throw;
        }

        tabla.Imprimir();
        //return base.VisitProgramClassAST(context);
        return null;
    }

    /*
    // using : USING ident SEMICOLON    #UsingClassAST;
    */
    public override object? VisitUsingClassAST(AlphaParser.UsingClassASTContext context)
    {
        IToken tok = (IToken)Visit(context.ident());
        tabla.Insertar(tok, TablaSimbolos.BasicType.Null, TablaSimbolos.DataType.Using, nivelPropio);
        return null;
    }

    /*
    // varDecl : type ident (COMMA ident)* SEMICOLON    #VarDeclAST;
        int  carro, moto, avion;
        int[] x, y, z;
        int x;
    */
    public override object? VisitVarDeclAST(AlphaParser.VarDeclASTContext context)
    {
        try
        {
            foreach (var child in context.ident())
            {
                IToken tok = (IToken)Visit(child);
                var ultimasDosPosiciones =
                    context.type().GetText()
                        .Substring(context.type().GetText().Length - 2); // [] para saber si es arreglo

                TablaSimbolos.BasicType varTipo = showType(context.type().GetText());
                if (varTipo is TablaSimbolos.BasicType.Error)
                {
                    System.Diagnostics.Debug.WriteLine("Error en visit VarDeclAST, tipo recibido: " +
                                                       context.type().GetText());
                }
                else if (ultimasDosPosiciones.Equals("[]"))
                {
                    if (varTipo is TablaSimbolos.BasicType.Int or TablaSimbolos.BasicType.Char)
                    {
                        tabla.Insertar(tok, varTipo, TablaSimbolos.DataType.Array,
                            nivelPropio);
                    }
                    else
                    {
                        var textoSinUltimosDosCaracteres =
                            context.type().GetText().Substring(0, context.type().GetText().Length - 2);
                        System.Diagnostics.Debug.WriteLine(
                            "Error en visit VarDeclAST, los arreglos solo pueden ser de tipo int o char, tipo encontrado: " +
                            textoSinUltimosDosCaracteres);
                    }
                }
                else
                {
                    tabla.Insertar(tok, varTipo, TablaSimbolos.DataType.Variable, nivelPropio);
                }
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
        nivelPropio++;
        System.Diagnostics.Debug.WriteLine("visit ClassDeclAST: " + context.ident().GetText());
        tabla.OpenScope();
        IToken tok = (IToken)Visit(context.ident());
        tabla.Insertar(tok, TablaSimbolos.BasicType.Null, TablaSimbolos.DataType.Class, nivelPropio);
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
        nivelPropio++;
        System.Diagnostics.Debug.WriteLine("visit MethodDeclAST: " + context.ident().GetText());
        tabla.OpenScope();
        IToken tok = (IToken)Visit(context.ident());
        if (context.type() != null)
        {
            var tipo = showType(context.type().GetText());
            if (tipo == TablaSimbolos.BasicType.Error)
            {
                System.Diagnostics.Debug.WriteLine("Error en visit MethodDeclAST, tipo recibido: " +
                                                   context.type().GetText());
                return null;
            }

            // TODO: Se debe verificar que el metodo sea un tipo valido para el lenguaje
            tabla.Insertar(tok, tipo, TablaSimbolos.DataType.Method, nivelPropio);
        }
        else
        {
            tabla.Insertar(tok, TablaSimbolos.BasicType.Void, TablaSimbolos.DataType.Method, nivelPropio);
        }

        if (context.formPars() != null)
        {
            Visit(context.formPars());
        }

        Visit(context.block());
        tabla.CloseScope();
        return null;
    }

    /*
     * formPars : type ident (COMMA type ident)*    #FormParsAST;
     *  int x, char y, int z  
     */
    public override object? VisitFormParsAST(AlphaParser.FormParsASTContext context)
    {
        System.Diagnostics.Debug.WriteLine("visit FormParsAST: " + context.GetText());
        if (context.type().Length > 0)
        {
            for (int i = 0; i < context.type().Length; i++)
            {
                var tipo = showType(context.type(i).GetText());
                IToken tok = (IToken)Visit(context.ident(i));
                // TODO: Se debe verificar que el tipo exista en el lenguaje, ademas de saber a que funcion pertenecen los parametros
                tabla.Insertar(tok, tipo, TablaSimbolos.DataType.Variable, nivelPropio);
            }
        }

        return null;
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
        nivelPropio++;
        tabla.OpenScope();
        IToken tok = context.IF().Symbol;
        tabla.Insertar(tok, TablaSimbolos.BasicType.Null, TablaSimbolos.DataType.If, nivelPropio);
        Visit(context.condition());
        Visit(context.statement(0));
        tabla.CloseScope();
        
        if (context.statement(1) != null)
        {
            nivelPropio++;
            IToken tok2 = context.ELSE().Symbol;
            tabla.Insertar(tok2, TablaSimbolos.BasicType.Null, TablaSimbolos.DataType.Else, nivelPropio);
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
        nivelPropio++;
        tabla.OpenScope();
        IToken tok = context.FOR().Symbol;
        tabla.Insertar(tok, TablaSimbolos.BasicType.Null, TablaSimbolos.DataType.For, nivelPropio);
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
        nivelPropio++;
        IToken tok = context.WHILE().Symbol;
        tabla.Insertar(tok, TablaSimbolos.BasicType.Null, TablaSimbolos.DataType.While, nivelPropio);
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
     *  | NUMBER    #NumberFactorAST
     */
    public override object? VisitNumberFactorAST(AlphaParser.NumberFactorASTContext context)
    {
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