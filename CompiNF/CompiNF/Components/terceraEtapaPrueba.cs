using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Antlr.Runtime;
using generated;
using System.Diagnostics;

namespace CompiNF.Components;

public class terceraEtapaPrueba: AlphaParserBaseVisitor<object>
{
    private Type pointType = null;
    private string asmFileName = "test.exe";
    private AssemblyName myAsmName = new AssemblyName();//creo un asembly

    private AppDomain currentDom = Thread.GetDomain();
    private AssemblyBuilder myAsmBldr;//creo un assembly

    private ModuleBuilder myModuleBldr; //carga un modulo en el assembly

    private TypeBuilder myTypeBldr;//carga una clase en el modulo
    private ConstructorInfo objCtor=null;

    private MethodInfo writeMI, writeMS;
    private MethodBuilder pointMainBldr, currentMethodBldr;//define un de entrada en la clase
      
    private List<MethodBuilder> metodosGlobales; 

    private bool isArgument = false;

    public terceraEtapaPrueba()
    {
        metodosGlobales = new List<MethodBuilder>(); 
            
        myAsmName.Name = "TestASM";
        myAsmBldr = currentDom.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.RunAndSave);
        myModuleBldr = myAsmBldr.DefineDynamicModule(asmFileName);
        myTypeBldr = myModuleBldr.DefineType("TestClass");
            
        Type objType = Type.GetType("System.Object");
        objCtor = objType.GetConstructor(new Type[0]);
            
        Type[] ctorParams = new Type[0];
        ConstructorBuilder pointCtor = myTypeBldr.DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.Standard,
            ctorParams);
        ILGenerator ctorIL = pointCtor.GetILGenerator();
        ctorIL.Emit(OpCodes.Ldarg_0);
        ctorIL.Emit(OpCodes.Call, objCtor);
        ctorIL.Emit(OpCodes.Ret);
    }
    
    //program : (using)* CLASS ident LEFT_BRACE (varDecl | classDecl | methodDecl)* RIGHT_BRACE EOF   #ProgramClassAST;
    public override object VisitProgramClassAST(AlphaParser.ProgramClassASTContext context)
    {
        IToken Tok = (IToken)Visit(context.ident());

        foreach (var child in context.children)
        {
            Visit(child);
        }

        // pointType = myTypeBldr.CreateType(); //creo un tipo de la clase para luego ser instanciada
        // myAsmBldr.SetEntryPoint(pointMainBldr);// setEntryPoint cargar el metodo de entrada a la clase
        // myAsmBldr.Save(asmFileName);//
        //     
        // return pointType;
        return null;
    }

    //using : USING ident SEMICOLON    #UsingClassAST;
    public override object VisitUsingClassAST(AlphaParser.UsingClassASTContext context)
    {
        return base.VisitUsingClassAST(context);
    }
    
    //varDecl locals [int indexVar=0, boolean isLocal=false]: type ident (COMMA ident)* SEMICOLON     #VarDeclAST;
    public override object VisitVarDeclAST(AlphaParser.VarDeclASTContext context)
    {
        Debug.WriteLine("VisitVarDeclAST");
        Type type = (Type)Visit(context.type());
        foreach (var child in context.ident())
        {
            bool iL = (bool)context.isLocal;
            Debug.WriteLine("ident declPointer: " + child.declPointer.GetText());
            Debug.WriteLine("Context indexVar: " + context.indexVar);
            Debug.WriteLine("Context isLocal: " + iL);
            Visit(child);
        }
        
        return null;
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

    public override object VisitSemicolonStatementAST(AlphaParser.SemicolonStatementASTContext context)
    {
        return base.VisitSemicolonStatementAST(context);
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
        return base.VisitIdentAST(context);
    }

    public override object VisitArrayAST(AlphaParser.ArrayASTContext context)
    {
        return base.VisitArrayAST(context);
    }
}