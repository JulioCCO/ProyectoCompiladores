using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Antlr.Runtime;
using generated;
using System.Diagnostics;
using System.Windows;

namespace CompiNF.Components;

public class terceraEtapaPrueba : AlphaParserBaseVisitor<object>
{
    // diccionario de variables globales
    Dictionary<string, FieldBuilder> variablesGlobales;

    // diccionario de variables locales
    Dictionary<string, LocalBuilder> variablesLocales;

    private Type pointType = null;
    private string asmFileName = "test.exe";
    private AssemblyName myAsmName = new AssemblyName(); //creo un asembly

    private AppDomain currentDom = Thread.GetDomain();
    private AssemblyBuilder myAsmBldr; //creo un assembly

    private ModuleBuilder myModuleBldr; //carga un modulo en el assembly

    private TypeBuilder myTypeBldr; //carga una clase en el modulo
    private ConstructorInfo objCtor = null;

    private MethodInfo writeMI, writeMS, writeMB, writeMC;
    private MethodBuilder pointMainBldr, currentMethodBldr; //define un de entrada en la clase

    private List<MethodBuilder> metodosGlobales;

    private bool isArgument = false;

    public terceraEtapaPrueba()
    {
        metodosGlobales = new List<MethodBuilder>();
        variablesGlobales = new Dictionary<string, FieldBuilder>();
        variablesLocales = new Dictionary<string, LocalBuilder>();

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
            new Type[] { typeof(double) });
        writeMS = typeof(Console).GetMethod(
            "WriteLine",
            new Type[] { typeof(string) });
        writeMC = typeof(Console).GetMethod(
            "WriteLine",
            new Type[] { typeof(char) });

        writeMB = typeof(Console).GetMethod(
            "WriteLine",
            new Type[] { typeof(bool) });
    }

    //program : (using)* CLASS ident LEFT_BRACE (varDecl | classDecl | methodDecl)* RIGHT_BRACE EOF   #ProgramClassAST;
    public override object VisitProgramClassAST(AlphaParser.ProgramClassASTContext context)
    {
        foreach (var child in context.children)
        {
            Visit(child);
        }

        pointType = myTypeBldr.CreateType(); //creo un tipo de la clase para luego ser instanciada
        myAsmBldr.SetEntryPoint(pointMainBldr); // setEntryPoint cargar el metodo de entrada a la clase
        myAsmBldr.Save(asmFileName); //

        // imprimiendo las listas
        Debug.WriteLine("Variables globales");
        foreach (var item in variablesGlobales)
        {
            Debug.WriteLine("Nombre: " + item.Key + " tipo: " + item.Value.FieldType);
        }

        Debug.WriteLine("Variables locales");
        foreach (var item in variablesLocales)
        {
            Debug.WriteLine("Nombre: " + item.Key + " tipo: " + item.Value.LocalType);
        }

        return pointType;
    }

    //using : USING ident SEMICOLON    #UsingClassAST;
    public override object VisitUsingClassAST(AlphaParser.UsingClassASTContext context)
    {
        return base.VisitUsingClassAST(context);
    }

    //varDecl locals [int indexVar=0, boolean isLocal=false]: type ident (COMMA ident)* SEMICOLON     #VarDeclAST;
    public override object VisitVarDeclAST(AlphaParser.VarDeclASTContext context)
    {
        bool iL = context.isLocal;

        if (iL) // Si es local
        {
            ILGenerator currentIl = currentMethodBldr.GetILGenerator();
            foreach (var child in context.ident())
            {
                Type typeLocalVar = (Type)Visit(context.type());
                LocalBuilder localVar = currentIl.DeclareLocal(typeLocalVar);
                variablesLocales.Add(child.GetText(), localVar);
            }
        }
        else // Si es global
        {
            foreach (var child in context.ident())
            {
                string nameGlobalVar = child.GetText();
                Type typeGlobalVar = (Type)Visit(context.type());
                FieldBuilder globalVar = myTypeBldr.DefineField(nameGlobalVar, typeGlobalVar,
                    FieldAttributes.Public | FieldAttributes.Static);
                // Almacenar en la lista de variables globales
                variablesGlobales.Add(nameGlobalVar, globalVar);
            }
        }

        return null;
    }

    //classDecl : CLASS ident LEFT_BRACE (varDecl)* RIGHT_BRACE                                       #ClassDeclAST;
    public override object VisitClassDeclAST(AlphaParser.ClassDeclASTContext context)
    {
        foreach (var child in context.children)
        {
            Visit(child);
        }

        return null;
    }

    // methodDecl : (type | VOID) ident LEFT_PAREN formPars? RIGHT_PAREN block                         #MethodDeclAST;
    public override object VisitMethodDeclAST(AlphaParser.MethodDeclASTContext context)
    {
        Type typeMethod = null;
        if (context.type() != null)
            typeMethod = verificarTipoRetorno((string)Visit(context.type()));
        else if (context.VOID() != null)
            typeMethod = typeof(void);

        currentMethodBldr = myTypeBldr.DefineMethod(context.ident().GetText(),
            MethodAttributes.Public |
            MethodAttributes.Static,
            typeMethod,
            null); //los parámetros son null porque se tiene que visitar despues de declarar el método... se cambiará luego

        //se visitan los parámetros para definir el arreglo de tipos de cada uno de los parámetros formales... si es que hay (not null)
        Type[] parameters = null;
        if (context.formPars() != null)
            parameters = (Type[])Visit(context.formPars());

        //después de visitar los parámetros, se cambia el signatura que requiere la definición del método
        currentMethodBldr.SetParameters(parameters);

        //se visita el cuerpo del método para generar el código que llevará el "currentMethodBldr" de turno
        Visit(context.block());

        ILGenerator currentIL = currentMethodBldr.GetILGenerator();
        currentIL.Emit(OpCodes.Ret);

        //Se agrega el método recién creado a la lista de mpetodos globales para no perder su referencia cuando se creen más métodos
        metodosGlobales.Add(currentMethodBldr);
        if (context.ident().GetText().Equals("Main"))
        {
            //el puntero al metodo principal se setea cuando es el Main quien se declara
            pointMainBldr = currentMethodBldr;
        }

        variablesLocales
            .Clear(); //se limpia la lista de variables locales para que no se mezclen con las de otros métodos
        return null;
    }

    // formPars : type ident (COMMA type ident)*                                                       #FormParsAST;
    public override object VisitFormParsAST(AlphaParser.FormParsASTContext context)
    {
        return base.VisitFormParsAST(context);
    }

    private Type verificarTipoRetorno(string tipo)
    {
        switch (tipo)
        {
            case "int":
                return typeof(double);
            case "char":
                return typeof(char);
            case "boolean":
                return typeof(bool);
            case "double":
                return typeof(double);
        }

        return null;
    }

    // type : ident array?  #TypeAST;
    public override object VisitTypeAST(AlphaParser.TypeASTContext context)
    {
        return verificarTipoRetorno(context.ident().GetText());
    }

    // statement : designator (ASSIGN expr | LEFT_PAREN actPars? RIGHT_PAREN | INC | DEC) SEMICOLON    #AssignStatementAST
    public override object VisitAssignStatementAST(AlphaParser.AssignStatementASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();

        if (context.expr() != null)
        {
            string name = context.designator().GetText();


            if (variablesLocales.ContainsKey(name))
            {
                //se obtiene el localbuilder de la variable local
                LocalBuilder local = variablesLocales[name];
                //se visita la expresión para generar el bytecode correspondiente (QUEDARÁ EN EL TOPE DE LA PILA EL VALOR A ASIGNAR)
                Visit(context.expr());
                //se guarda el valor en la variable local
                currentIL.Emit(OpCodes.Stloc, local.LocalIndex);
            }
            else if (variablesGlobales.ContainsKey(name))
            {
                //se obtiene el fieldbuilder de la variable global
                FieldBuilder global = variablesGlobales[name];
                //se visita la expresión para generar el bytecode correspondiente (QUEDARÁ EN EL TOPE DE LA PILA EL VALOR A ASIGNAR)
                Visit(context.expr());
                //se guarda el valor en la variable global
                currentIL.Emit(OpCodes.Stsfld, global);
            }
        }

        return null;
    }

    // | IF LEFT_PAREN condition RIGHT_PAREN statement (ELSE statement)?                     #IfStatementAST
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

    // | FOR LEFT_PAREN expr SEMICOLON condition? SEMICOLON statement? RIGHT_PAREN statement #ForStatementAST
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

    // | WHILE LEFT_PAREN condition RIGHT_PAREN statement                                    #WhileStatementAST
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

    // | BREAK SEMICOLON                                                                     #BreakStatementAST
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

    // | RETURN expr? SEMICOLON                                                              #ReturnStatementAST
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

    //  | READ LEFT_PAREN designator RIGHT_PAREN SEMICOLON                                    #ReadStatementAST   
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

    // | WRITE LEFT_PAREN expr (COMMA (INT_CONST|STRING_CONST))? RIGHT_PAREN SEMICOLON       #WriteStatementAST
    public override object VisitWriteStatementAST(AlphaParser.WriteStatementASTContext context)
    {
        // Obtener el generador de código IL actual
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();


        Type exprType = (Type)Visit(context.expr());

        Debug.WriteLine(exprType);

        if (exprType == typeof(double)) // Tanto para int como para double
        {
            currentIL.EmitCall(OpCodes.Call, writeMI, null);
        }
        else if (exprType == typeof(string)) // Para string
        {
            currentIL.EmitCall(OpCodes.Call, writeMS, null);
        }
        else if (exprType == typeof(char)) // Para char
        {
            currentIL.EmitCall(OpCodes.Call, writeMC, null);
        }
        else if (exprType == typeof(bool)) // Para bool
        {
            currentIL.EmitCall(OpCodes.Call, writeMB, null);
        }

        return null;
    }

    // | block                                                                               #BlockStatementAST
    public override object VisitBlockStatementAST(AlphaParser.BlockStatementASTContext context)
    {
        Visit(context.block());
        return null;
    }

    // | SEMICOLON                                                                           #SemicolonStatementAST;
    public override object VisitSemicolonStatementAST(AlphaParser.SemicolonStatementASTContext context)
    {
        return base.VisitSemicolonStatementAST(context);
    }

    // block : LEFT_BRACE (varDecl | statement)* RIGHT_BRACE      #BlockAST;
    public override object VisitBlockAST(AlphaParser.BlockASTContext context)
    {
        foreach (var child in context.children)
        {
            Visit(child);
        }

        return null;
    }

    // actPars : expr (COMMA expr)*                                                                    #ActParsAST; 
    public override object VisitActParsAST(AlphaParser.ActParsASTContext context)
    {
        return base.VisitActParsAST(context);
    }

    // condition : condTerm (LOGICAL_OR condTerm)*                                                     #ConditionAST;
    public override object VisitConditionAST(AlphaParser.ConditionASTContext context)
    {
        return base.VisitConditionAST(context);
    }

    // condTerm : condFact (LOGICAL_AND condFact)*                                                     #CondTermAST;
    public override object VisitCondTermAST(AlphaParser.CondTermASTContext context)
    {
        return base.VisitCondTermAST(context);
    }

    //condFact : expr relop expr                                                                      #CondFactAST;
    public override object VisitCondFactAST(AlphaParser.CondFactASTContext context)
    {
        return base.VisitCondFactAST(context);
    }

    // cast : LEFT_PAREN type RIGHT_PAREN                                                              #CastAST;
    public override object VisitCastAST(AlphaParser.CastASTContext context)
    {
        return base.VisitCastAST(context);
    }

    // expr : MINUSEXP? cast? term (addop term)*                                                       #ExprAST;
    public override object VisitExprAST(AlphaParser.ExprASTContext context)
    {
        Type typeTerm = null;

        if (context.cast() == null)
        {
            typeTerm = (Type)Visit(context.term(0));

            if (context.term().Length > 1)
            {
                //ILGenerator currentIL = currentMethodBldr.GetILGenerator();
                for (int i = 1; context.term().Length > i; i++)
                {
                    Visit(context.term(i));
                    Visit(context.addop(i - 1));
                }
            }
        }

        return typeTerm;
    }

    // term : factor (mulop factor)*                                                                   #TermAST;
    public override object VisitTermAST(AlphaParser.TermASTContext context)
    {
        Type typeFact = (Type)Visit(context.factor(0));
        if (context.factor().Length>1)
        {
            for (int i = 1; i < context.factor().Length; i++)
            {
                Visit(context.factor(i));
                Visit(context.mulop(i-1));
            }
        }
        
        return typeFact;
        
    }

    // factor : designator (LEFT_PAREN actPars? RIGHT_PAREN)?                                          #DesignatorFactorAST
    public override object VisitDesignatorFactorAST(AlphaParser.DesignatorFactorASTContext context)
    {
        //factor
        Type typeDesignator = (Type)Visit(context.designator());

        if (context.actPars() != null)
        {
            Visit(context.actPars());
        }

        return typeDesignator;
    }

    // | CHAR_CONST                                                                             #CharFactorAST
    public override object VisitCharFactorAST(AlphaParser.CharFactorASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();
        
        currentIL.Emit(OpCodes.Ldc_I4_S, context.CHAR_CONST().GetText()[1]);
        
        return typeof(char);
    }

    // | STRING_CONST                                                                           #StringFactorAST
    public override object VisitStringFactorAST(AlphaParser.StringFactorASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();
        currentIL.Emit(OpCodes.Ldstr, context.STRING_CONST().GetText());

        return typeof(string);
    }

    //  | (MINUS)? INT_CONST                                                                     #IntFactorAST   
    public override object VisitIntFactorAST(AlphaParser.IntFactorASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();

        string doubleText = context.INT_CONST().GetText() + ".0";
        double OutVal;
        double.TryParse(doubleText, out OutVal);
        currentIL.Emit(OpCodes.Ldc_R8, OutVal);

        return typeof(double);
    }

    //  | (MINUS)? DOUBLE_CONST                                                                  #DoubleFactorAST    
    public override object VisitDoubleFactorAST(AlphaParser.DoubleFactorASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();

        string doubleText = context.DOUBLE_CONST().GetText();
        double OutVal;
        double.TryParse(doubleText, out OutVal);
        currentIL.Emit(OpCodes.Ldc_R8, OutVal);

        return typeof(double);
    }

    //  | BOOL_CONST                                                                             #BoolFactorAST
    public override object VisitBoolFactorAST(AlphaParser.BoolFactorASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();
        int num = context.BOOL_CONST().GetText() == "true" ? 1 : 0;
        currentIL.Emit(OpCodes.Ldc_I4, num);

        return typeof(bool);
    }

    // | NEW type                                                                               #NewFactorAST
    public override object VisitNewFactorAST(AlphaParser.NewFactorASTContext context)
    {
        return base.VisitNewFactorAST(context);
    }

    //  | LEFT_PAREN expr RIGHT_PAREN                                                            #ParenFactorAST;
    public override object VisitParenFactorAST(AlphaParser.ParenFactorASTContext context)
    {
        return base.VisitParenFactorAST(context);
    }

    // designator : ident (DOT ident | LEFT_BRACKET expr RIGHT_BRACKET)*                               #DesignatorAST;
    public override object VisitDesignatorAST(AlphaParser.DesignatorASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();

        bool flag = false;

        if (context.ident().Length == 1)
        {
            if (variablesLocales.ContainsKey(context.ident(0).GetText()))
            {
                currentIL.Emit(OpCodes.Ldloc, variablesLocales[context.ident(0).GetText()]);
                flag = true;
            }
            else
                currentIL.Emit(OpCodes.Ldsfld, variablesGlobales[context.ident(0).GetText()]);
        }

        return flag
            ? variablesLocales[context.ident(0).GetText()].LocalType
            : variablesGlobales[context.ident(0).GetText()].FieldType;
    }

    // relop : EQUALS | NOT_EQUALS | GREATER_THAN | GREATER_OR_EQUALS | LESS_THAN | LESS_OR_EQUALS;
    public override object VisitRelop(AlphaParser.RelopContext context)
    {
        return base.VisitRelop(context);
    }

    // addop : PLUS | MINUSEXP;
    public override object VisitAddop(AlphaParser.AddopContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();
        if (context.PLUS() != null)
            currentIL.Emit(OpCodes.Add);
        else if (context.MINUSEXP() != null)
            currentIL.Emit(OpCodes.Sub);

        return null;
    }

    // mulop : MULT | DIV | MOD;
    public override object VisitMulop(AlphaParser.MulopContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();
        if (context.MULT() != null)
            currentIL.Emit(OpCodes.Mul);
        else if (context.DIV() != null)
            currentIL.Emit(OpCodes.Div);
        else if (context.MOD() != null)
        {
            currentIL.Emit(OpCodes.Rem);
        }
        
        return null;
    }

    // ident locals [ParserRuleContext declPointer = null]: IDENTIFIER                                   #IdentAST;
    public override object VisitIdentAST(AlphaParser.IdentASTContext context)
    {
        return context.IDENTIFIER().GetText();
    }

    // array : LEFT_BRACKET INT_CONST? RIGHT_BRACKET                                                     #ArrayAST;
    public override object VisitArrayAST(AlphaParser.ArrayASTContext context)
    {
        return base.VisitArrayAST(context);
    }
}