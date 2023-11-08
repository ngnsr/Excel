grammar Grammar;

/*
 * Parser Rules
 */

compileUnit : expression EOF;

expression :
 LPAREN expression RPAREN #ParenthesizedExpr
 | operatorToken=(PLUS | MINUS) expression #UnaryExpr
 | MMIN LPAREN paramlist=arglist RPAREN #MMinExpr
 | MMAX LPAREN paramlist=arglist RPAREN #MMaxExpr
 | MIN LPAREN expression ',' expression RPAREN #MinExpr
 | MAX LPAREN expression ',' expression RPAREN #MaxExpr
 | expression EXPONENT expression #ExponentialExpr
 | NUMBER #NumberExpr
 | IDENTIFIER #IdentifierExpr
;
/*
 * Lexer Rules
 */

NUMBER : INT ('.' INT)?; 
IDENTIFIER : [A-Z]+[1-9][0-9]*;
INT : ('0'..'9')+;
arglist: expression (',' expression)*;
EXPONENT : '^';
MINUS : '-';
PLUS : '+';
LPAREN : '(';
RPAREN : ')';
MIN : 'min';
MAX : 'max';
MMAX : 'mmax';
MMIN : 'mmin';

// skip spaces, tabs, newlines
WS : [ \t\r\n] -> skip;