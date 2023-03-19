lexer grammar Scanner;

// Keywords
CLASS: 'class';
NAMESPACE: 'namespace';
INTERFACE: 'interface';
ENUM: 'enum';
STRUCT: 'struct';
VOID: 'void';
NEW: 'new';
THIS: 'this';
BASE: 'base';
IF: 'if';
ELSE: 'else';
WHILE: 'while';
DO: 'do';
FOR: 'for';
FOREACH: 'foreach';
IN: 'in';
RETURN: 'return';
SWITCH: 'switch';
CASE: 'case';
DEFAULT: 'default';
TRY: 'try';
CATCH: 'catch';
FINALLY: 'finally';
THROW: 'throw';
THROWS: 'throws';
PRIVATE: 'private';
PROTECTED: 'protected';
PUBLIC: 'public';
INTERNAL: 'internal';
ABSTRACT: 'abstract';
VIRTUAL: 'virtual';
OVERRIDE: 'override';
STATIC: 'static';
CONST: 'const';
READONLY: 'readonly';
GET: 'get';
SET: 'set';
ADD: 'add';
REMOVE: 'remove';
EVENT: 'event';
AS: 'as';
IS: 'is';
NULL: 'null';

// Literals
INT_LITERAL: DIGIT+;
HEX_LITERAL: '0x' HEX_DIGIT+;
FLOAT_LITERAL: DIGIT+ '.' DIGIT* EXPONENT?;
DOUBLE_LITERAL: DIGIT+ '.' DIGIT* EXPONENT?;
CHAR_LITERAL: '\'' (ESCAPE_SEQUENCE | ~['\\]) '\'';
STRING_LITERAL: '"' (ESCAPE_SEQUENCE | ~["\\])* '"';

fragment DIGIT: [0-9];
fragment HEX_DIGIT: [0-9a-fA-F];
fragment EXPONENT: [eE] [+-]? DIGIT+;

fragment ESCAPE_SEQUENCE
    :   '\\' [btnfr"'\\]
    |   UNICODE_ESCAPE
    ;

fragment UNICODE_ESCAPE
    :   '\\' 'u' HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
    ;

// Identifiers
IDENTIFIER: LETTER (LETTER | DIGIT | '_')*;

fragment LETTER
    :   [a-zA-Z]
    |   '\u00C0'..'\u00D6'
    |   '\u00D8'..'\u00F6'
    |   '\u00F8'..'\u00FF'
    ;

// Operators
PLUS: '+';
MINUS: '-';
MULTIPLY: '*';
DIVIDE: '/';
MODULO: '%';
ASSIGN: '=';
PLUS_ASSIGN: '+=';
MINUS_ASSIGN: '-=';
MULTIPLY_ASSIGN: '*=';
DIVIDE_ASSIGN: '/=';
MODULO_ASSIGN: '%=';
EQUALS: '==';
NOT_EQUALS: '!=';
LESS_THAN: '<';
GREATER_THAN: '>';
LESS_OR_EQUALS: '<=';
GREATER_OR_EQUALS: '>=';
LOGICAL_AND: '&&';
LOGICAL_OR: '||';
LOGICAL_NOT: '!';
BITWISE_AND: '&';
BITWISE_OR: '|';
BITWISE_NOT: '~';
BITWISE_XOR: '^';
LEFT_SHIFT: '<<';
RIGHT_SHIFT: '>>';
CONDITIONAL_OPERATOR: '?';
NULL_COALESCE_OPERATOR: '??';
INC: '++';
DEC: '--';
DOT: '.';
ELLIPSIS: '...';
COLON: ':';
SEMICOLON: ';';
COMMA: ',';
ARROW: '->';

// Brackets
LEFT_PAREN: '(';
RIGHT_PAREN: ')';
LEFT_BRACE: '{';
RIGHT_BRACE: '}';
LEFT_SQUARE_BRACKET: '[';
RIGHT_SQUARE_BRACKET: ']';
