using System;
using Antlr4.Runtime;
using generated;

namespace Compilador.Components;

public class AContextual : AlphaParserBaseVisitor<object>
{
    private TablaSimbolos tabla;

    public AContextual()
    {
        tabla = new TablaSimbolos();
    }

    private string showType(int type)
    {
        return type switch
        {
            0 => "int",
            1 => "double",
            2 => "string",
            3 => "bool",
            4 => "char",
            5 => "void",
            6 => "class",
            _ => "error"
        };
    }

    private string showToken(IToken token)
    {
        return token.Text + "Fila, columna: (" + token.Line + "," + token.Column + ")";
    }

    private bool isMethod(IToken token)
    {
        return tabla.Buscar(token).isMethod;
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

    public override object VisitProgramClassAST(AlphaParser.ProgramClassASTContext context)
    {
        tabla.OpenScope();
        try
        {
            // IToken Tok = (IToken) Visit(context.ident());
            // tabla.Insertar(Tok, 6, false);
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
                    System.Diagnostics.Debug.WriteLine("visita varDecl" + i + ": " + child.GetText());
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
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("Error en visit ProgramClassAST" + e.Message);
            Console.WriteLine(e);
            throw;
        }

        tabla.CloseScope();
        return base.VisitProgramClassAST(context);
    }

    public override object VisitUsingClassAST(AlphaParser.UsingClassASTContext context)
    {
        return base.VisitUsingClassAST(context);
    }

    public override object VisitVarDeclAST(AlphaParser.VarDeclASTContext context)
    {
        return base.VisitVarDeclAST(context);
    }

    public override object VisitClassDeclAST(AlphaParser.ClassDeclASTContext context)
    {
        return base.VisitClassDeclAST(context);
    }

    public override object VisitMethodDeclAST(AlphaParser.MethodDeclASTContext context)
    {
        return base.VisitMethodDeclAST(context);
    }

    public override object VisitFormParsAST(AlphaParser.FormParsASTContext context)
    {
        return base.VisitFormParsAST(context);
    }

    public override object VisitTypeAST(AlphaParser.TypeASTContext context)
    {
        return base.VisitTypeAST(context);
    }

    public override object VisitAssignStatementAST(AlphaParser.AssignStatementASTContext context)
    {
        return base.VisitAssignStatementAST(context);
    }

    public override object VisitIfStatementAST(AlphaParser.IfStatementASTContext context)
    {
        return base.VisitIfStatementAST(context);
    }

    public override object VisitForStatementAST(AlphaParser.ForStatementASTContext context)
    {
        return base.VisitForStatementAST(context);
    }

    public override object VisitWhileStatementAST(AlphaParser.WhileStatementASTContext context)
    {
        return base.VisitWhileStatementAST(context);
    }

    public override object VisitBreakStatementAST(AlphaParser.BreakStatementASTContext context)
    {
        return base.VisitBreakStatementAST(context);
    }

    public override object VisitReturnStatementAST(AlphaParser.ReturnStatementASTContext context)
    {
        return base.VisitReturnStatementAST(context);
    }

    public override object VisitReadStatementAST(AlphaParser.ReadStatementASTContext context)
    {
        return base.VisitReadStatementAST(context);
    }

    public override object VisitWriteStatementAST(AlphaParser.WriteStatementASTContext context)
    {
        return base.VisitWriteStatementAST(context);
    }

    public override object VisitBlockStatementAST(AlphaParser.BlockStatementASTContext context)
    {
        return base.VisitBlockStatementAST(context);
    }

    public override object VisitBlockAST(AlphaParser.BlockASTContext context)
    {
        return base.VisitBlockAST(context);
    }

    public override object VisitActParsAST(AlphaParser.ActParsASTContext context)
    {
        return base.VisitActParsAST(context);
    }

    public override object VisitConditionAST(AlphaParser.ConditionASTContext context)
    {
        return base.VisitConditionAST(context);
    }

    public override object VisitCondTermAST(AlphaParser.CondTermASTContext context)
    {
        return base.VisitCondTermAST(context);
    }

    public override object VisitCondFactAST(AlphaParser.CondFactASTContext context)
    {
        return base.VisitCondFactAST(context);
    }

    public override object VisitCastAST(AlphaParser.CastASTContext context)
    {
        return base.VisitCastAST(context);
    }

    public override object VisitExprAST(AlphaParser.ExprASTContext context)
    {
        return base.VisitExprAST(context);
    }

    public override object VisitTermAST(AlphaParser.TermASTContext context)
    {
        return base.VisitTermAST(context);
    }

    public override object VisitDesignatorFactorAST(AlphaParser.DesignatorFactorASTContext context)
    {
        return base.VisitDesignatorFactorAST(context);
    }

    public override object VisitCharFactorAST(AlphaParser.CharFactorASTContext context)
    {
        return base.VisitCharFactorAST(context);
    }

    public override object VisitStringFactorAST(AlphaParser.StringFactorASTContext context)
    {
        return base.VisitStringFactorAST(context);
    }

    public override object VisitIntFactorAST(AlphaParser.IntFactorASTContext context)
    {
        return base.VisitIntFactorAST(context);
    }

    public override object VisitDoubleFactorAST(AlphaParser.DoubleFactorASTContext context)
    {
        return base.VisitDoubleFactorAST(context);
    }

    public override object VisitBoolFactorAST(AlphaParser.BoolFactorASTContext context)
    {
        return base.VisitBoolFactorAST(context);
    }

    public override object VisitNewFactorAST(AlphaParser.NewFactorASTContext context)
    {
        return base.VisitNewFactorAST(context);
    }

    public override object VisitParenFactorAST(AlphaParser.ParenFactorASTContext context)
    {
        return base.VisitParenFactorAST(context);
    }

    public override object VisitDesignatorAST(AlphaParser.DesignatorASTContext context)
    {
        return base.VisitDesignatorAST(context);
    }

    public override object VisitRelop(AlphaParser.RelopContext context)
    {
        return base.VisitRelop(context);
    }

    public override object VisitAddop(AlphaParser.AddopContext context)
    {
        return base.VisitAddop(context);
    }

    public override object VisitMulop(AlphaParser.MulopContext context)
    {
        return base.VisitMulop(context);
    }

    public override object VisitIdentAST(AlphaParser.IdentASTContext context)
    {
        System.Diagnostics.Debug.WriteLine("visit IdentAST :" + context.IDENTIFIER().GetText());
        return context.IDENTIFIER();
    }

    public override object VisitArrayAST(AlphaParser.ArrayASTContext context)
    {
        return base.VisitArrayAST(context);
    }

    public override object VisitNumberFactorAST(AlphaParser.NumberFactorASTContext context)
    {
        return base.VisitNumberFactorAST(context);
    }
}