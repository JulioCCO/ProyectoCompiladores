parser grammar AlphaParser;


options {
    tokenVocab = AlphaScanner;
}

program : (using)* CLASS ident LEFT_BRACE (varDecl | classDecl | methodDecl)* RIGHT_BRACE EOF   #ProgramClassAST;
    
using : USING ident SEMICOLON                                                                   #UsingClassAST;

varDecl : type ident (COMMA ident)* SEMICOLON                                                   #VarDeclAST;

classDecl : CLASS ident LEFT_BRACE (varDecl)* RIGHT_BRACE                                       #ClassDeclAST;

methodDecl : (type | VOID) ident LEFT_PAREN formPars? RIGHT_PAREN block                         #MethodDeclAST;

formPars : type ident (COMMA type ident)*                                                       #FormParsAST;

type : ident array?                                                                             #TypeAST;                                 

statement : designator (ASSIGN expr | LEFT_PAREN actPars? RIGHT_PAREN | INC | DEC) SEMICOLON    #AssignStatementAST
          | IF LEFT_PAREN condition RIGHT_PAREN statement (ELSE statement)?                     #IfStatementAST
          | FOR LEFT_PAREN expr SEMICOLON condition? SEMICOLON statement? RIGHT_PAREN statement #ForStatementAST
          | WHILE LEFT_PAREN condition RIGHT_PAREN statement                                    #WhileStatementAST
          | BREAK SEMICOLON                                                                     #BreakStatementAST
          | RETURN expr? SEMICOLON                                                              #ReturnStatementAST
          | READ LEFT_PAREN designator RIGHT_PAREN SEMICOLON                                    #ReadStatementAST   
          | WRITE LEFT_PAREN expr (COMMA (INT_CONST|STRING_CONST))? RIGHT_PAREN SEMICOLON       #WriteStatementAST
          | block                                                                               #BlockStatementAST
          | SEMICOLON                                                                           #SemicolonStatementAST;
          
block : LEFT_BRACE (varDecl | statement)* RIGHT_BRACE                                           #BlockAST;

actPars : expr (COMMA expr)*                                                                    #ActParsAST; 

condition : condTerm (LOGICAL_OR condTerm)*                                                     #ConditionAST;

condTerm : condFact (LOGICAL_AND condFact)*                                                     #CondTermAST;

condFact : expr relop expr                                                                      #CondFactAST;

cast : LEFT_PAREN type RIGHT_PAREN                                                              #CastAST;

expr : MINUSEXP? cast? term (addop term)*                                                       #ExprAST;
 
term : factor (mulop factor)*                                                                   #TermAST;

factor : designator (LEFT_PAREN actPars? RIGHT_PAREN)?                                          #DesignatorFactorAST 
       | CHAR_CONST                                                                             #CharFactorAST
       | STRING_CONST                                                                           #StringFactorAST
       | (MINUS)? INT_CONST                                                                     #IntFactorAST   
       | DOUBLE_CONST                                                                           #DoubleFactorAST    
       | BOOL_CONST                                                                             #BoolFactorAST
       | NEW ident                                                                              #NewFactorAST
       | LEFT_PAREN expr RIGHT_PAREN                                                            #ParenFactorAST;

designator : ident (DOT ident | LEFT_BRACKET expr RIGHT_BRACKET)*                               #DesignatorAST;

relop : EQUALS 
        | NOT_EQUALS 
        | GREATER_THAN 
        | GREATER_OR_EQUALS 
        | LESS_THAN 
        | LESS_OR_EQUALS;

addop : PLUS | MINUSEXP;

mulop : MULT | DIV | MOD;

ident : IDENTIFIER                                                                                #IdentAST;

array : LEFT_BRACKET IDENTIFIER? RIGHT_BRACKET                                                    #ArrayAST;



