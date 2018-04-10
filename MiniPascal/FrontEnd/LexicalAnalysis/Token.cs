using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPascal.FrontEnd.LexicalAnalysis
{
    public class Position
    {
        public int row;
        public int col;
        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }

    public class Token
    {
        public TokenType type;
        public string lexeme;

        public Position pos;


        public Token(TokenType type, string lexeme, Position pos)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.pos = pos;
        }

        public Token(TokenType type, string lexeme)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.pos = null;
        }

        public string toString()
        {
            return "( " + this.type.ToString() + ", " + this.lexeme + " )";
        }
    }
}
