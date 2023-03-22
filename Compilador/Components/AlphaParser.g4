parser grammar AlphaParser;


options {
    tokenVocab = AlphaScanner;
}

program : using? CLASS ident LEFT_BRACE (varDecl | classDecl | methodDecl)* RIGHT_BRACE;

using : USING ident SEMICOLON;

varDecl : type ident (COMMA ident)* SEMICOLON;

classDecl : CLASS ident LEFT_BRACE varDecl* RIGHT_BRACE;

methodDecl : (type | VOID) ident LEFT_PAREN formPars? RIGHT_PAREN block;

formPars : type ident (COMMA type ident)*;

type : ident array?;

block : LEFT_BRACE (varDecl | statement)* RIGHT_BRACE;

statement : designator (ASSIGN expr | LEFT_PAREN actPars? RIGHT_PAREN | INC | DEC) SEMICOLON
          | IF LEFT_PAREN condition RIGHT_PAREN statement (ELSE statement)?
          | FOR LEFT_PAREN expr SEMICOLON condition? SEMICOLON statement? RIGHT_PAREN statement
          | WHILE LEFT_PAREN condition RIGHT_PAREN statement
          | BREAK SEMICOLON
          | RETURN expr? SEMICOLON
          | READ LEFT_PAREN designator RIGHT_PAREN SEMICOLON
          | WRITE LEFT_PAREN expr (COMMA NUMBER)? RIGHT_PAREN SEMICOLON
          | block
          | SEMICOLON;
 
actPars : expr (COMMA expr)*; 

condition : condTerm (LOGICAL_OR condTerm)*;

condTerm : condFact (LOGICAL_AND condFact)*;

condFact : expr relop expr;

cast : LEFT_PAREN type RIGHT_PAREN;

expr : MINUS? cast? term (addop term)*;

term : factor (mulop factor)*;

factor : designator (LEFT_PAREN actPars? RIGHT_PAREN)?
       | NUMBER
       | CHAR_CONST
       | STRING_CONST
       | (TRUE | FALSE)
       | NEW ident
       | LEFT_PAREN expr RIGHT_PAREN;

designator : ident (DOT ident | LEFT_BRACKET expr RIGHT_BRACKET)*;

relop : EQUALS | NOT_EQUALS | GREATER_THAN | GREATER_OR_EQUALS | LESS_THAN | LESS_OR_EQUALS;

addop : PLUS | MINUS;

mulop : MULT | DIV | MOD;

ident : IDENTIFIER;

array : LEFT_BRACKET RIGHT_BRACKET;



