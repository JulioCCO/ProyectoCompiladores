lexer grammar Scanner;

// Keywords
CLASS: 'class';
VOID: 'void';
NEW: 'new';
USING: 'using';
SEMICOLON: ';';
COMMA: ',';
ASSIGN: '=';
INC: '++';
DEC: '--';
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
DOT: '.';
CONST: 'const';

// Operators
EQUALS: '==';
NOT_EQUALS: '!=';
LESS_THAN: '<';
LESS_OR_EQUALS: '<=';
GREATER_THAN: '>';
GREATER_OR_EQUALS: '>=';

// Operators
LOGICAL_AND: '&&';
LOGICAL_OR: '||';

// Brackets
LEFT_PAREN: '(';
RIGHT_PAREN: ')';
LEFT_BRACE: '{';
RIGHT_BRACE: '}';
LEFT_SQUARE_BRACKET: '[';
RIGHT_SQUARE_BRACKET: ']';


// Operators
PLUS: '+';
MINUS: '-';
MULT: '*';
DIV: '/';
MOD: '%';

// Literals
NUMBER: DIGIT+;

// CONSTANTS
CHAR_CONST: '\'' (LETTER | DIGIT | '_' | WS)? '\'';
STRING_CONST: QUOMARK (LETTER | NUMBER | LETTER | '_' | WS)* QUOMARK;
INT_CONST: DIGIT+;
FLOAT_CONST: DIGIT+ '.' DIGIT* EXPONENT?;
DOUBLE_CONST: DIGIT+ '.' DIGIT* EXPONENT?;
BOOL_CONST: TRUE | FALSE;

// BASIC TYPES
INT: 'int';
FLOAT: 'float';
DOUBLE: 'double';
CHAR: 'char';
BOOL: 'bool';

// Arrays
ARRAY: 'array';


fragment EXPONENT: [eE] [+-]? DIGIT+;
// Identifiers
IDENTIFIER: LETTER (LETTER | DIGIT | '_')*;

fragment DIGIT: [0-9];
fragment LETTER: [a-zA-Z];
fragment QUOMARK : '"';

// Whitespace and comments
WS : [ \t\r\n]+ -> skip;
COMMENT : '/*' .*? '*/' -> skip;
LINE_COMMENT : '//' .*? ( '\r' | '\n' ) -> skip;


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














