using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPascal.FrontEnd.LexicalAnalysis
{
    public enum TokenType
    {
        //Value types
        BOOLEAN, INTEGER, STRING, REAL, ARRAY,

        //special symbols 
        PLUS, MINUS, MULT, MOD, EQUALS, NOTEQUAL, LESS, GREATER, LESSOREQUAL, GREATEROREQUAL,
        LEFTBRACKET, RIGHTBRACKET, LEFTSQUARE, RIGHTSQUARE, DOT, COMMA, SEMICOLON, COLON, OR, AND, NOT,
        ASSIGN, DIV,

        //Keywords
        IF, THEN, ELSE, OF, WHILE, DO, BEGIN, END, VAR,
        PROCEDURE, FUNCTION, PROGRAM,  RETURN,

        //Predefined ids
        TYPE, READ, SIZE, WRITELN, ASSERT,

        //Other
        ERROR,EOF,ID,
    }
}
