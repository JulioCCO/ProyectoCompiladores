using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Antlr.Runtime;
using generated;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace CompiNF.Components;

public class terceraEtapaPrueba : AlphaParserBaseVisitor<object>
{
    // ---------- FOR ----------
    // for exit label
    Label forExitLabel;

    //IlGenerator for
    ILGenerator forIl;

    // for condition label
    bool isFor = false;
    
    // ---------- WHILE ----------
    
    // while exit label
    Label whileExitLabel;

    //IlGenerator while
    ILGenerator whileIl;

    // while condition label
    bool isWhile = false;

    // para saber de donde viene el padre SI ES UNA CLASE O UN METODO
    bool currentFatherIsClass = false;


    // diccionario de variables globales
    Dictionary<string, FieldBuilder> variablesGlobales;

    // diccionario de variables locales
    Dictionary<string, LocalBuilder> variablesLocales;

    // diccionario de clases
    Dictionary<string, TypeBuilder> clases;

    // class builder
    TypeBuilder myClassBldr;

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
        clases = new Dictionary<string, TypeBuilder>();



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

        // Para la escritura en consola de los tipos de datos WRITEMI, WRITEMS, WRITEMB, WRITEMC
        writeMI = typeof(Console).GetMethod(
            "WriteLine",
            new Type[] { typeof(double) }); // se usa para int y double
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
        // visitar los hijos
        foreach (var child in context.children)
        {
            Visit(child);
        }

        pointType = myTypeBldr.CreateType(); //creo un tipo de la clase para luego ser instanciada
        myAsmBldr.SetEntryPoint(pointMainBldr); // setEntryPoint cargar el metodo de entrada a la clase
        myAsmBldr.Save(asmFileName); //

        // imprimiendo las listas de variables globales y locales para ver si se guardaron bien
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
        Debug.WriteLine("\n");
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
        // si el padre es una clase
        if (currentFatherIsClass)
        {
            foreach (var child in context.ident())
            {
                string nameGlobalVar = child.GetText();
                Type typeGlobalVar = (Type)Visit(context.type());

                FieldBuilder fieldB = myClassBldr.DefineField(nameGlobalVar, typeGlobalVar,
                    FieldAttributes.Public | FieldAttributes.Static);
                // Almacenar en la lista de variables globales
                variablesGlobales.Add(nameGlobalVar, fieldB);
            }
        }
        else // Si el padre es un metodo
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
        }

        return null;
    }

    //classDecl : CLASS ident LEFT_BRACE (varDecl)* RIGHT_BRACE                                       #ClassDeclAST;
    public override object VisitClassDeclAST(AlphaParser.ClassDeclASTContext context)
    {
        currentFatherIsClass = true;
        string className = context.ident().GetText();

        // Crear la clase
        myClassBldr = myModuleBldr.DefineType(className, TypeAttributes.Public);

        // Visitar las declaraciones de variables dentro de la clase
        foreach (var child in context.children)
        {
            Visit(child);
        }
        
        // Se crea el tipo de la clase
        myClassBldr.CreateType();
        
        // Se agrega la clase a la lista de clases
        clases.Add(className, myClassBldr);
        currentFatherIsClass = false;
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

        //variablesLocales.Clear(); //se limpia la lista de variables locales para que no se mezclen con las de otros métodos
        return null;
    }

    // formPars : type ident (COMMA type ident)*                                                       #FormParsAST;
    public override object VisitFormParsAST(AlphaParser.FormParsASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();
        isArgument = true;

        //se construye el arreglo de tipos necesario para enviarle a la definición de métodos
        Type[] result = new Type[context.type().Length];

        for (int i = 0; context.type().Length > i; i++)
        {
            result[i] = verificarTipoRetorno((string)Visit((context.type(i))));
            currentIL.Emit(OpCodes.Ldarg, i);
            currentIL.DeclareLocal(result[i]);
            currentIL.Emit(OpCodes.Stloc, i);
            //TODO se debería llevar una lista de argumentos para saber cual es cual cuando se deban llamar
            //currentIL.Emit(OpCodes.Ldloc, 0);//solo para la prueba, el 0 es el que se va a llamar
        }

        isArgument = false;

        return result;
    }
    
    // Se verifica el tipo de retorno de los métodos para saber si es válido
    private Type verificarTipoRetorno(string tipo)
    {
        switch (tipo)
        {
            case "int":
                return typeof(double);
            case "char":
                return typeof(char);
            case "string":
                return typeof(string);
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
        // Se verifica el tipo de retorno
        return verificarTipoRetorno(context.ident().GetText());
    }

    // statement : designator (ASSIGN expr | LEFT_PAREN actPars? RIGHT_PAREN | INC | DEC) SEMICOLON    #AssignStatementAST
    public override object VisitAssignStatementAST(AlphaParser.AssignStatementASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();

        string name = context.designator().GetText();

        if (context.expr() != null) // si es una asignación -----------------------------------------------------------------------------------
        {
            if (variablesLocales.ContainsKey(name)) // si es una variable local
            {
                //se obtiene el localbuilder de la variable local
                LocalBuilder local = variablesLocales[name];
                //se visita la expresión para generar el bytecode correspondiente (QUEDARÁ EN EL TOPE DE LA PILA EL VALOR A ASIGNAR)
                Visit(context.expr());
                //se guarda el valor en la variable local
                currentIL.Emit(OpCodes.Stloc, local.LocalIndex);
            }
            else if (variablesGlobales.ContainsKey(name)) // si es una variable global
            {
                //se obtiene el fieldbuilder de la variable global
                FieldBuilder global = variablesGlobales[name];
                //se visita la expresión para generar el bytecode correspondiente (QUEDARÁ EN EL TOPE DE LA PILA EL VALOR A ASIGNAR)
                Visit(context.expr());
                //se guarda el valor en la variable global
                currentIL.Emit(OpCodes.Stsfld, global);
            }
        }
        else if (context.INC() != null) // si es un incremento -----------------------------------------------------------------------------------
        {
            if (variablesLocales.ContainsKey(name)) // si es una variable local
            {
                //se obtiene el localbuilder de la variable local
                LocalBuilder local = variablesLocales[name];
                //se visita la designator para generar el bytecode correspondiente (QUEDARÁ EN EL TOPE DE LA PILA EL VALOR A ASIGNAR)
                Visit(context.designator());

                //se carga un uno en la pila
                currentIL.Emit(OpCodes.Ldc_R8, 1.0);

                currentIL.Emit(OpCodes.Add);

                //se guarda el valor en la variable local
                currentIL.Emit(OpCodes.Stloc, local.LocalIndex);
            }
            else if (variablesGlobales.ContainsKey(name)) // si es una variable global
            {
                //se obtiene el fieldbuilder de la variable global
                FieldBuilder global = variablesGlobales[name];
                //se visita la designator para generar el bytecode correspondiente (QUEDARÁ EN EL TOPE DE LA PILA EL VALOR A ASIGNAR)
                Visit(context.designator());

                //se carga un uno en la pila
                currentIL.Emit(OpCodes.Ldc_R8, 1.0);

                currentIL.Emit(OpCodes.Add);

                //se guarda el valor en la variable global
                currentIL.Emit(OpCodes.Stsfld, global);
            }
        }

        else if (context.DEC() != null) // si es un decremento -----------------------------------------------------------------------------------
        {
            if (variablesLocales.ContainsKey(name)) // si es una variable local
            {
                //se obtiene el localbuilder de la variable local
                LocalBuilder local = variablesLocales[name];
                //se visita la designator para generar el bytecode correspondiente (QUEDARÁ EN EL TOPE DE LA PILA EL VALOR A ASIGNAR)
                Visit(context.designator());

                //se carga un uno en la pila
                currentIL.Emit(OpCodes.Ldc_R8, 1.0);

                currentIL.Emit(OpCodes.Sub);

                //se guarda el valor en la variable local
                currentIL.Emit(OpCodes.Stloc, local.LocalIndex);
            }
            else if (variablesGlobales.ContainsKey(name)) // si es una variable global
            {
                //se obtiene el fieldbuilder de la variable global
                FieldBuilder global = variablesGlobales[name];
                //se visita la designator para generar el bytecode correspondiente (QUEDARÁ EN EL TOPE DE LA PILA EL VALOR A ASIGNAR)
                Visit(context.designator());

                //se carga un uno en la pila
                currentIL.Emit(OpCodes.Ldc_R8, 1.0);

                currentIL.Emit(OpCodes.Sub);
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

        if (context.ELSE() != null) // si hay un else -----------------------------------------------------------------------------------
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
        isFor = true;
        
        forIl = currentMethodBldr.GetILGenerator(); // Obtener el ILGenerator del método actual

        Label loopLabel = forIl.DefineLabel(); // Definir la etiqueta para el bucle for

        forExitLabel = forIl.DefineLabel(); // Definir la etiqueta para la salida del bucle for

        forIl.MarkLabel(loopLabel); // Marcar la etiqueta para el bucle for

        Visit(context.statement().Last()); // Visitar el statement del for

        if (context.statement().Length == 2) // Si hay un statement dentro del for
        {
            Visit(context.statement().First()); // Visitar el statement del for
        }

        Visit(context.condition()); // Visitar la condición del for

        forIl.Emit(OpCodes.Brfalse, forExitLabel); // Salto a la etiqueta de salida del for

        forIl.Emit(OpCodes.Br, loopLabel); // Salto a la etiqueta del bucle for

        // Marcar la etiqueta de salida del bucle while
        forIl.MarkLabel(forExitLabel);

        isFor = false;
        return null;
    }

    // | WHILE LEFT_PAREN condition RIGHT_PAREN statement      #WhileStatementAST
    public override object VisitWhileStatementAST(AlphaParser.WhileStatementASTContext context)
    {
        isWhile = true;
        // Obtener el ILGenerator del método actual
        whileIl = currentMethodBldr.GetILGenerator();

        // Definir la etiqueta para el bucle while
        Label loopLabel = whileIl.DefineLabel();
        whileIl.MarkLabel(loopLabel);

        // Visitar la condición del bucle while
        Visit(context.condition());

        // Definir la etiqueta de salida del bucle while
        whileExitLabel = whileIl.DefineLabel();
        //globalExitLabel = exitLabel;

        whileIl.Emit(OpCodes.Brfalse, whileExitLabel);

        // Visitar el statement del bucle while
        Visit(context.statement());

        // Volver al inicio del bucle while
        whileIl.Emit(OpCodes.Br, loopLabel);

        // Marcar la etiqueta de salida del bucle while
        whileIl.MarkLabel(whileExitLabel);

        isWhile = false;
        return null;
    }

    // | BREAK SEMICOLON                                                                     #BreakStatementAST
    public override object VisitBreakStatementAST(AlphaParser.BreakStatementASTContext context)
    {
        if (isFor) // Si se está dentro de un bucle for
        {
            forIl.Emit(OpCodes.Br, forExitLabel); // Salto a la etiqueta de salida del bucle for
        }

        if (isWhile) // Si se está dentro de un bucle while
        {
            whileIl.Emit(OpCodes.Br, whileExitLabel); // Salto a la etiqueta de salida del bucle while
        }

        return null;
    }

    // | RETURN expr? SEMICOLON                                           #ReturnStatementAST
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
        //read statement codegeneration
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();

        //Nombre del designator
        string nombre = context.designator().GetText();

        //Verificamos el tipo de la variable ingresada
        Type type = context.tipoDeDato;

        if (type == typeof(int)) // Si es un entero
        {
            string doubleText = context.data + ".0";
            double OutVal;
            double.TryParse(doubleText, out OutVal);
            currentIL.Emit(OpCodes.Ldc_R8, OutVal);
        }
        else if (type == typeof(double)) // Si es un double
        {
            string doubleText = context.data;
            double OutVal;
            double.TryParse(doubleText, out OutVal);
            currentIL.Emit(OpCodes.Ldc_R8, OutVal);
        }
        else if (type == typeof(string)) // Si es un string
        {
            currentIL.Emit(OpCodes.Ldstr, context.data); 
        }
        else if (type == typeof(char)) // Si es un char
        {
            char convertirAChar = char.TryParse(context.data, out convertirAChar) ? convertirAChar : ' ';
            currentIL.Emit(OpCodes.Ldc_I4_S, convertirAChar);
        }
        else if (type == typeof(bool)) // Si es un booleano
        { 
            int num = context.data.ToLower() == "true" ? 1 : 0;
            currentIL.Emit(OpCodes.Ldc_I4, num);
        }
        
        LocalBuilder local = variablesLocales[nombre];
        currentIL.Emit(OpCodes.Stloc, local.LocalIndex);
        
        return null;
    }

    // | WRITE LEFT_PAREN expr (COMMA (INT_CONST|STRING_CONST))? RIGHT_PAREN SEMICOLON       #WriteStatementAST
    public override object VisitWriteStatementAST(AlphaParser.WriteStatementASTContext context)
    {
        // Obtener el generador de código IL actual
        ILGenerator currentIL = currentMethodBldr.GetILGenerator();


        Type exprType = (Type)Visit(context.expr()); // Obtener el tipo de la expresión

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
        Visit(context.block()); // Visitar el bloque
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
        foreach (var child in context.children) // Visitar cada hijo del bloque
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
        Visit(context.condTerm()[0]); // Visitar el primer condTerm
        for (int i = 1; i < context.condTerm().Length; i++)
        {
            Visit(context.condTerm()[i]); // Visitar los demás condTerm
        }

        return null;
    }

    // condTerm : condFact (LOGICAL_AND condFact)*                                                     #CondTermAST;
    public override object VisitCondTermAST(AlphaParser.CondTermASTContext context)
    {
        Visit(context.condFact()[0]); // Visitar el primer condFact
        for (int i = 1; i < context.condFact().Length; i++) 
        {
            Visit(context.condFact()[i]); // Visitar los demás condFact
        }

        return null;
    }

    //condFact : expr relop expr                                                                      #CondFactAST;
    public override object VisitCondFactAST(AlphaParser.CondFactASTContext context)
    {
        Visit(context.expr()[0]); // Visitar el primer expr
        Visit(context.expr()[1]); // Visitar el segundo expr
        Visit(context.relop()); // Visitar el relop
        return null;
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

        if (context.cast() == null) // Si no hay cast
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
        Type typeFact = (Type)Visit(context.factor(0)); // Visitar el primer factor
        if (context.factor().Length > 1)
        {
            for (int i = 1; i < context.factor().Length; i++) // Visitar los demás factores
            {
                Visit(context.factor(i));
                Visit(context.mulop(i - 1));
            }
        }

        return typeFact;
    }

    // factor : designator (LEFT_PAREN actPars? RIGHT_PAREN)?                                          #DesignatorFactorAST
    public override object VisitDesignatorFactorAST(AlphaParser.DesignatorFactorASTContext context)
    {
        //factor
        Type typeDesignator = (Type)Visit(context.designator()); // Visitar el designator

        if (context.actPars() != null) // Si hay actPars
        {
            Visit(context.actPars());
        }

        return typeDesignator;
    }

    // | CHAR_CONST                                                                             #CharFactorAST
    public override object VisitCharFactorAST(AlphaParser.CharFactorASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator(); // Obtener el ILGenerator del método actual

        currentIL.Emit(OpCodes.Ldc_I4_S, context.CHAR_CONST().GetText()[1]); // Cargar el char en la pila

        return typeof(char);
    }

    // | STRING_CONST                                                                           #StringFactorAST
    public override object VisitStringFactorAST(AlphaParser.StringFactorASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator(); // Obtener el ILGenerator del método actual
        currentIL.Emit(OpCodes.Ldstr, context.STRING_CONST().GetText()); // Cargar el string en la pila

        return typeof(string);
    }

    //  | (MINUS)? INT_CONST                                                                     #IntFactorAST   
    public override object VisitIntFactorAST(AlphaParser.IntFactorASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator(); // Obtener el ILGenerator del método actual

        string doubleText = context.INT_CONST().GetText() + ".0";
        double OutVal;
        double.TryParse(doubleText, out OutVal); // Convertir el string a double
        currentIL.Emit(OpCodes.Ldc_R8, OutVal); // Cargar el int en la pila

        return typeof(double);
    }

    //  | (MINUS)? DOUBLE_CONST                                                                  #DoubleFactorAST    
    public override object VisitDoubleFactorAST(AlphaParser.DoubleFactorASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator(); // Obtener el ILGenerator del método actual

        string doubleText = context.DOUBLE_CONST().GetText();
        double OutVal;
        double.TryParse(doubleText, out OutVal); // Convertir el string a double
        currentIL.Emit(OpCodes.Ldc_R8, OutVal); // Cargar el double en la pila

        return typeof(double);
    }

    //  | BOOL_CONST                                                     #BoolFactorAST
    public override object VisitBoolFactorAST(AlphaParser.BoolFactorASTContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator(); // Obtener el ILGenerator del método actual
        int num = context.BOOL_CONST().GetText() == "true" ? 1 : 0; // Convertir el string a int
        currentIL.Emit(OpCodes.Ldc_I4, num); // Cargar el bool en la pila

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
        ILGenerator currentIL = currentMethodBldr.GetILGenerator(); // Obtener el ILGenerator del método actual
        
        string nombre = context.ident(0).GetText(); // Obtener el nombre del identificador
        
        bool estado = false;

        if (context.ident().Length == 1)
        {
            if (variablesLocales.TryGetValue(nombre, out var locale))
            {
                if (locale != null) currentIL.Emit(OpCodes.Ldloc, locale); // Cargar la variable local en la pila
                estado = true;
            }
            else
                currentIL.Emit(OpCodes.Ldsfld, variablesGlobales[nombre]); // Cargar la variable global en la pila
        }

        return estado
            ? variablesLocales[nombre].LocalType // Retornar el tipo de la variable local
            : variablesGlobales[nombre].FieldType; // Retornar el tipo de la variable global
    }

    // relop : EQUALS | NOT_EQUALS | GREATER_THAN | GREATER_OR_EQUALS | LESS_THAN | LESS_OR_EQUALS;
    public override object VisitRelop(AlphaParser.RelopContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator(); // Obtener el ILGenerator del método actual

        if (context.LESS_THAN() != null) // Si es menor que
            currentIL.Emit(OpCodes.Clt); // Comparar si es menor que

        else if (context.LESS_OR_EQUALS() != null) // Si es menor o igual que
        {
            currentIL.Emit(OpCodes.Cgt); // Comparar si es mayor que
            currentIL.Emit(OpCodes.Ldc_I4_0); // Cargar 0 en la pila
            currentIL.Emit(OpCodes.Ceq); // Comparar si es igual
        }

        else if (context.GREATER_THAN() != null) // Si es mayor que
            currentIL.Emit(OpCodes.Cgt); // Comparar si es mayor que

        else if (context.GREATER_OR_EQUALS() != null) // Si es mayor o igual que
        {
            currentIL.Emit(OpCodes.Clt); // Comparar si es menor que
            currentIL.Emit(OpCodes.Ldc_I4_0); // Cargar 0 en la pila
            currentIL.Emit(OpCodes.Ceq); // Comparar si es igual
        }

        else if (context.EQUALS() != null) // Si es igual que
        {
            currentIL.Emit(OpCodes.Ceq); // Comparar si es igual
        }

        else if (context.NOT_EQUALS() != null) // Si es diferente que
        {
            currentIL.Emit(OpCodes.Ceq); // Comparar si es igual
            currentIL.Emit(OpCodes.Ldc_I4_0); // Cargar 0 en la pila
            currentIL.Emit(OpCodes.Ceq);    // Comparar si es igual
        }

        return null;
    }

    // addop : PLUS | MINUSEXP;
    public override object VisitAddop(AlphaParser.AddopContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator(); // Obtener el ILGenerator del método actual
        if (context.PLUS() != null) // Si es suma
            currentIL.Emit(OpCodes.Add); // Sumar
        else if (context.MINUSEXP() != null) // Si es resta
            currentIL.Emit(OpCodes.Sub); // Restar

        return null;
    }

    // mulop : MULT | DIV | MOD;
    public override object VisitMulop(AlphaParser.MulopContext context)
    {
        ILGenerator currentIL = currentMethodBldr.GetILGenerator(); // Obtener el ILGenerator del método actual
        if (context.MULT() != null)
            currentIL.Emit(OpCodes.Mul); // Multiplicar
        else if (context.DIV() != null)
            currentIL.Emit(OpCodes.Div); // Dividir
        else if (context.MOD() != null)
        {
            currentIL.Emit(OpCodes.Rem); // Modulo
        }

        return null;
    }

    // ident locals [ParserRuleContext declPointer = null]: IDENTIFIER                                   #IdentAST;
    public override object VisitIdentAST(AlphaParser.IdentASTContext context)
    {
        return context.IDENTIFIER().GetText(); // Retornar el nombre del identificador
    }

    // array : LEFT_BRACKET INT_CONST? RIGHT_BRACKET                                                     #ArrayAST;
    public override object VisitArrayAST(AlphaParser.ArrayASTContext context)
    {
        return base.VisitArrayAST(context);
    }
}