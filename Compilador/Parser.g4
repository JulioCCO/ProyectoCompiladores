grammar Parser;

options {
    tokenVocab = Scanner;
}

program : usingClause? CLASS identifier LEFT_BRACE (varDecl | classDecl | methodDecl)* RIGHT_BRACE;

usingClause : USING identifier SEMICOLON;

varDecl : type identifier (COMMA identifier)* SEMICOLON;

classDecl : CLASS identifier LEFT_BRACE varDecl* RIGHT_BRACE;

methodDecl : (type | VOID) identifier LEFT_PAREN formPars? RIGHT_PAREN block;

formPars : type identifier (COMMA type identifier)*;

type : identifier array?;

block : LEFT_BRACE (varDecl | statement)* RIGHT_BRACE;

statement : designator (ASSIGN expr | LEFT_PAREN actPars? RIGHT_PAREN | INCREMENT | DECREMENT) SEMICOLON
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

condition : condTerm (OR condTerm)*;

condTerm : condFact (AND condFact)*;

condFact : expr relop expr;

cast : LEFT_PAREN type RIGHT_PAREN;

expr : MINUS? cast? term (addop term)*;

term : factor (mulop factor)*;

factor : designator (LEFT_PAREN actPars? RIGHT_PAREN)?
       | NUMBER
       | CHAR_CONST
       | STRING_CONST
       | TRUE
       | FALSE
       | NEW identifier
       | LEFT_PAREN expr RIGHT_PAREN;

designator : identifier (DOT identifier | LEFT_BRACKET expr RIGHT_BRACKET)*;

relop : EQ | NE | GT | GE | LT | LE;

addop : PLUS | MINUS;

mulop : MULT | DIV | MOD;

identifier : IDENTIFIER;

array : LEFT_BRACKET RIGHT_BRACKET;

// Whitespace and comments
WS : [ \t\r\n]+ -> skip;
COMMENT : '/*' .*? '*/' -> skip;
LINE_COMMENT : '//' .*? ( '\r' | '\n' ) -> skip;
