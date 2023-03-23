lexer grammar AlphaScanner;
 

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
STRING_CONST: QUOMARK (LETTER|DIGIT|LETTER|WS)* QUOMARK; // "Hello World"
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


//GET: 'get';
//SET: 'set';
//ADD: 'add';
//REMOVE: 'remove';
//EVENT: 'event';
//AS: 'as';
//IS: 'is';
//NULL: 'null';
// Literals
//LiteralsINT_LITERAL: DIGIT+;
//HEX_LITERAL: '0x' HEX_DIGIT+;

//FLOAT_CONST: (MINUS)? DIGIT+ '.' DIGIT* EXPONENT?;
//FLOAT_LITERAL: DIGIT+ '.' DIGIT* EXPONENT?;
//DOUBLE_LITERAL: DIGIT+ '.' DIGIT* EXPONENT?;
//CHAR_LITERAL: '\'' (ESCAPE_SEQUENCE | ~['\\]) '\'';
//STRING_LITERAL: '"' (ESCAPE_SEQUENCE | ~["\\])* '"';
//
//
//fragment HEX_DIGIT: [0-9a-fA-F];
//fragment EXPONENT: [eE] [+-]? DIGIT+;
//
//fragment ESCAPE_SEQUENCE
//    :   '\\' [btnfr"'\\]
//    |   UNICODE_ESCAPE
//    ;
//
//fragment UNICODE_ESCAPE
//    :   '\\' 'u' HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
//    ;


//PLUS_ASSIGN: '+=';
//MINUS_ASSIGN: '-=';
//MULTIPLY_ASSIGN: '*=';
//DIVIDE_ASSIGN: '/=';
//MODULO_ASSIGN: '%=';
//LOGICAL_NOT: '!';
//BITWISE_AND: '&';
//BITWISE_OR: '|';
//BITWISE_NOT: '~';
//BITWISE_XOR: '^';
//LEFT_SHIFT: '<<';
//RIGHT_SHIFT: '>>';
//CONDITIONAL_OPERATOR: '?';
//NULL_COALESCE_OPERATOR: '??';
//ELLIPSIS: '...';
//COLON: ':';
//ARROW: '->';
//DO: 'do';
//FOREACH: 'foreach';
//IN: 'in';
//SWITCH: 'switch';
//CASE: 'case';
//DEFAULT: 'default';
//TRY: 'try';
//CATCH: 'catch';
//FINALLY: 'finally';
//THROW: 'throw';
//THROWS: 'throws';
//PRIVATE: 'private';
//PROTECTED: 'protected';
//PUBLIC: 'public';
//INTERNAL: 'internal';
//ABSTRACT: 'abstract';
//VIRTUAL: 'virtual';
//OVERRIDE: 'override';
//STATIC: 'static';

//READONLY: 'readonly';
//NAMESPACE: 'namespace';
//INTERFACE: 'interface';
//ENUM: 'enum';
//STRUCT: 'struct';
//THIS: 'this';
//BASE: 'base';














