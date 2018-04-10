using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPascal.FrontEnd.LexicalAnalysis
{
    public enum TokenType
    {
        //special symbols 
        PLUS, MINUS, MULT, MOD, EQUALS, NOTEQUALS, LESS, GREATER, LESSOREQUAL, GREATEROREQUAL,
        LEFTCURLY, RIGHTCURLY, LEFTSQUARE, RIGHTSQUARE, DOT, COMMA, SEMICOLON, COLON, 

        //Keywords
        OR, AND, NOT, IF, THEN, ELSE, OF, WHILE, DO, BEGIN, END, VAR,
        ARRAY, PROCEDURE, FUNCTION, PROGRAM, ASSERT, RETURN,

        //Predefined ids
        TYPE, FALSE, TRUE, READ, SIZE, WRITELN,

    }
}
