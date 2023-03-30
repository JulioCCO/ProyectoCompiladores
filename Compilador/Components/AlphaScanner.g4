lexer grammar AlphaScanner;
 
 @lexer::members {
                 public override void NotifyListeners(LexerNoViableAltException e){
                 this.ErrorListenerDispatch.SyntaxError(this.ErrorOutput, (IRecognizer) this, 0, TokenStartLine, this.TokenStartColumn, "reconocimiento de token : '" + this.GetErrorDisplay(this.EmitEOF().InputStream.GetText(Interval.Of(this.TokenStartCharIndex, this.InputStream.Index)))  + "'", (RecognitionException) e);
                }
 }
 

// Keywords
USING: 'using';
CLASS: 'class';
VOID: 'void';
NEW: 'new';
IF: 'if';
ELSE: 'else';
FOR: 'for';
WHILE: 'while';
BREAK: 'break';
RETURN: 'return';
READ: 'read';
WRITE: 'write';
TRUE: 'true';
FALSE: 'false';
CONST: 'const';
ASSIGN: '=';
INC: '++';
DEC: '--';
DOT: '.';
COMMA: ',';
SEMICOLON: ';';

// Operators
EQUALS: '==';
NOT_EQUALS: '!=';
LESS_THAN: '<';
LESS_OR_EQUALS: '<=';
GREATER_THAN: '>';
GREATER_OR_EQUALS: '>=';
LOGICAL_AND: '&&';
LOGICAL_OR: '||';
PLUS: '+';
MINUS: '-';
MULT: '*';
DIV: '/';
MOD: '%';
VAR: 'var';

// Brackets
LEFT_PAREN: '(';
RIGHT_PAREN: ')';

LEFT_BRACE: '{';
RIGHT_BRACE: '}';

LEFT_BRACKET: '[';
RIGHT_BRACKET: ']';


// Literals
NUMBER: DIGIT+; 

// CONSTANTS
CHAR_CONST: MINIQUOMARK CHAR MINIQUOMARK;  // 'a'
STRING_CONST: QUOMARK (LETTER|DIGIT|LETTER|WS|SPECIAL)* QUOMARK; // "Hello World"
INT_CONST: (MINUS)? DIGIT+; // 123 O -123
DOUBLE_CONST: (MINUS)? DIGIT+ ( '.' DIGIT)* ; // 123.123 O -123.123 O 123.
BOOL_CONST: TRUE | FALSE;  // true | false

// BASIC TYPES
INT_IDENT: 'int';
CHAR_IDENT: 'char';
DOUBLE_IDENT: 'double';
BOOL_IDENT: 'bool';
STRING_IDENT: 'string';

// Arrays
ARRAY: 'array'; 

// Identifiers
IDENTIFIER: LETTER (LETTER|DIGIT)*; // a O a1 O a1a

fragment CHAR: [a-zA-Z0-9];
fragment DIGIT: [0-9];
fragment LETTER: [a-zA-Z_];
fragment QUOMARK : '"';
fragment MINIQUOMARK: '\'';


// Whitespace and comments
WS : [ \t\r\n]+ -> skip; // skip spaces, tabs, newlines
COMMENT : '/*' .*? '*/' -> skip; // skip comments
LINE_COMMENT : '//' .*? ( '\r' | '\n' ) -> skip;  // skip line comments













