using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using generated;
using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;


namespace Compilador.Components;

public class TerceraEtapa: AlphaParserBaseVisitor<object>
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

    public TerceraEtapa()
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
            
        //inicializar writeline para string
            
        writeMI = typeof(Console).GetMethod(
            "WriteLine",
            new Type[] { typeof(int) });
        writeMS = typeof(Console).GetMethod(
            "WriteLine",
            new Type[] { typeof(string) });
    }

    public override object VisitProgramClassAST(AlphaParser.ProgramClassASTContext context)
    {
       
        IToken Tok = (IToken)Visit(context.ident());

        foreach (var child in context.children)
        {
            Visit(child);
        }

        pointType = myTypeBldr.CreateType(); //creo un tipo de la clase para luego ser instanciada
        myAsmBldr.SetEntryPoint(pointMainBldr);// setEntryPoint cargar el metodo de entrada a la clase
        myAsmBldr.Save(asmFileName);//
            
        return pointType;
        
        
    }

    public override object VisitUsingClassAST(AlphaParser.UsingClassASTContext context)
    {
        // Obtener el nombre del espacio de nombres a utilizar
        string namespaceName = context.ident().GetText();
    
        // Agregar una directiva de using al código generado
        usingDirectives.Add(namespaceName);
        return null;
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
        // Obtener el generador de código IL actual
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();
    
        // Obtener el contexto de la designación y la expresión
        AlphaParser.DesignatorASTContext designatorContext = (AlphaParser.DesignatorASTContext) Visit(context.designator());
        AlphaParser.ExprASTContext exprContext =(AlphaParser.ExprASTContext) Visit(context.expr());
    
        // Generar el código para evaluar la expresión
        Visit(exprContext);
    
        // Generar el código para la designación
        Visit(designatorContext);
    
        // Obtener el tipo de la expresión y la designación
        Type exprType = GetTypeFromExpression(exprContext);
        Type designatorType = GetTypeFromDesignator(designatorContext);
    
        // Comprobar si la asignación es válida
        if (!IsAssignable(exprType, designatorType))
        {
            // La asignación no es válida, puedes lanzar una excepción, mostrar un mensaje de error, etc.
        }
    
        // Emitir la instrucción de asignación correspondiente según el tipo de designador
        if (designatorContext.array() != null)
        {
            // Asignación a un elemento de un arreglo
            currentIL.Emit(OpCodes.Stelem, designatorType);
        }
        else
        {
            // Asignación a una variable
            LocalBuilder variable = GetVariableFromDesignator(designatorContext);
            currentIL.Emit(OpCodes.Stloc, variable);
        }

    }
    
    //| IF LEFT_PAREN condition RIGHT_PAREN statement (ELSE statement)?                     #IfStatementAST
    public override object VisitIfStatementAST(AlphaParser.IfStatementASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();
    
        // Visitar la condición del if
        Visit(context.condition());
    
        // Definir la etiqueta para el salto al bloque de código del if
        Label ifLabel = currentIL.DefineLabel();
        currentIL.Emit(OpCodes.Brfalse, ifLabel);
    
        // Visitar el statement del if
        Visit(context.statement(0));
    
        if (context.ELSE() != null)
        {
            // Si hay un bloque de código else, definir la etiqueta para el salto al bloque de código else
            Label elseLabel = currentIL.DefineLabel();
            currentIL.Emit(OpCodes.Br, elseLabel);
        
            // Marcar la etiqueta para el bloque de código del if
            currentIL.MarkLabel(ifLabel);
        
            // Visitar el statement del else
            Visit(context.statement(1));
        
            // Marcar la etiqueta para el final del bloque de código else
            currentIL.MarkLabel(elseLabel);
        }
        else
        {
            // Marcar la etiqueta para el final del bloque de código del if
            currentIL.MarkLabel(ifLabel);
        }
    
        return null;
    }
    
    //| FOR LEFT_PAREN expr SEMICOLON condition? SEMICOLON statement? RIGHT_PAREN statement #ForStatementAST
    public override object VisitForStatementAST(AlphaParser.ForStatementASTContext context)
    {
        // Visitar la expresión de inicialización del for
        Visit(context.expr());

        // Obtener el ILGenerator del método actual
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();

        // Definir la etiqueta de inicio del for
        Label loopStartLabel = currentIL.DefineLabel();
        currentIL.MarkLabel(loopStartLabel);

        // Visitar la condición del for
        Visit(context.condition());

        // Definir la etiqueta de salida del for
        Label loopExitLabel = currentIL.DefineLabel();
        currentIL.Emit(OpCodes.Brfalse, loopExitLabel);

        // Visitar el statement del for
        Visit(context.statement(0));

        // Visitar la expresión de iteración del for
        Visit(context.expr());

        // Salto al inicio del for
        currentIL.Emit(OpCodes.Br, loopStartLabel);

        // Marcar la etiqueta de salida del for
        currentIL.MarkLabel(loopExitLabel);

        return null;
    }

    public override object VisitWhileStatementAST(AlphaParser.WhileStatementASTContext context)
    {
        // Obtener el ILGenerator del método actual
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();
    
        // Definir la etiqueta para el bucle while
        Label loopLabel = currentIL.DefineLabel();
        currentIL.MarkLabel(loopLabel);
    
        // Visitar la condición del bucle while
        Visit(context.condition());
    
        // Definir la etiqueta de salida del bucle while
        Label exitLabel = currentIL.DefineLabel();
        currentIL.Emit(OpCodes.Brfalse, exitLabel);
    
        // Visitar el statement del bucle while
        Visit(context.statement());
    
        // Volver al inicio del bucle while
        currentIL.Emit(OpCodes.Br, loopLabel);
    
        // Marcar la etiqueta de salida del bucle while
        currentIL.MarkLabel(exitLabel);
    
        return null;
    }

    public override object VisitBreakStatementAST(AlphaParser.BreakStatementASTContext context)
    {
        // Obtener el generador de código IL actual
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();
    
        // Definir una etiqueta para el punto de salida del bucle
        Label loopExitLabel = currentIL.DefineLabel();
    
        // Emitir una instrucción de salto incondicional a la etiqueta de salida del bucle
        currentIL.Emit(OpCodes.Br, loopExitLabel);
    
        // Marcar la etiqueta de salida del bucle
        currentIL.MarkLabel(loopExitLabel);
    
        return null;

    }

    public override object VisitReturnStatementAST(AlphaParser.ReturnStatementASTContext context)
    {
        // Verificar si hay una expresión de retorno
        if (context.expr() != null)
        {
            // Visitar la expresión de retorno
            Visit(context.expr());
        }
    
        // Obtener el generador de código IL actual
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();
    
        // Emitir la instrucción de retorno
        currentIL.Emit(OpCodes.Ret);
    
        return null;
    }

    public override object VisitReadStatementAST(AlphaParser.ReadStatementASTContext context)
    {
        // Obtener el generador de código IL actual
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();
    
        // Obtener el nombre de la designación
        string designatorName = context.designator().GetText();
    
        // Almacenar el valor leído en la designación
        if (designatorName == "myInt")
        {
            // Llamar al método Console.ReadLine()
            MethodInfo readLineMethod = typeof(Console).GetMethod("ReadLine", Type.EmptyTypes);
            currentIL.EmitCall(OpCodes.Call, readLineMethod, null);
        
            // Convertir el valor de cadena leído a entero
            MethodInfo parseIntMethod = typeof(int).GetMethod("Parse", new[] { typeof(string) });
            currentIL.EmitCall(OpCodes.Call, parseIntMethod, null);
        
            // Declarar una variable local 'myInt'
            LocalBuilder myIntLocal = currentIL.DeclareLocal(typeof(int));
        
            // Almacenar el valor leído en la variable local 'myInt'
            currentIL.Emit(OpCodes.Stloc, myIntLocal);
        }
        else if (designatorName == "myString")
        {
            // Llamar al método Console.ReadLine()
            MethodInfo readLineMethod = typeof(Console).GetMethod("ReadLine", Type.EmptyTypes);
            currentIL.EmitCall(OpCodes.Call, readLineMethod, null);
        
            // Declarar una variable local 'myString'
            LocalBuilder myStringLocal = currentIL.DeclareLocal(typeof(string));
        
            // Almacenar el valor leído en la variable local 'myString'
            currentIL.Emit(OpCodes.Stloc, myStringLocal);
        }
        else
        {
            // Designación no válida
            // Puedes lanzar una excepción, mostrar un mensaje de error, etc.
        }
    
        return null;
    }

    public override object VisitWriteStatementAST(AlphaParser.WriteStatementASTContext context)
    {
        // Obtener el generador de código IL actual
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();
    
        // Obtener la expresión a escribir
        AlphaParser.ExprASTContext exprContext = (AlphaParser.ExprASTContext) context.expr();
    
        // Visitar la expresión para generar su código
        Visit(exprContext);
    
        // Obtener el tipo de la expresión
        
        // Type exprType = exprContext.GetType();
        Type exprType = GetTypeFromExpression(exprContext.GetType());
    
        if (exprType == typeof(int))
        {
            // Llamar al método Console.Write(int value)
            MethodInfo writeMethod = typeof(Console).GetMethod("Write", new[] { typeof(int) });
            currentIL.EmitCall(OpCodes.Call, writeMethod, null);
        }
        else if (exprType == typeof(string))
        {
            // Llamar al método Console.Write(string value)
            MethodInfo writeMethod = typeof(Console).GetMethod("Write", new[] { typeof(string) });
            currentIL.EmitCall(OpCodes.Call, writeMethod, null);
        }
        else
        {
            // Tipo de expresión no válido para escribir
            // Puedes lanzar una excepción, mostrar un mensaje de error, etc.
        }
    
        return null;
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
        return (IToken)context.IDENTIFIER().Symbol;
    }

    public override object VisitArrayAST(AlphaParser.ArrayASTContext context)
    {
        return base.VisitArrayAST(context);
    }
}

