using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPascal.FrontEnd.LexicalAnalysis
{
#pragma warning disable CS0659 // 'Position' overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class Position
#pragma warning restore CS0659 // 'Position' overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        public int row;
        public int col;
        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public override bool Equals(object obj)
        {

            if (obj == null || GetType() != obj.GetType())
                return false;

            Position p = (Position)obj;
            return (this.row == p.row) && (this.col == p.col);
        }

        public override string ToString()
        {
            return "(" + row + "," + col + ")";
        }
    }

#pragma warning disable CS0659 // 'Token' overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class Token
#pragma warning restore CS0659 // 'Token' overrides Object.Equals(object o) but does not override Object.GetHashCode()
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

        public override bool Equals(object obj) {

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Token t = (Token)obj;
            bool posEqual = false;
            if(this.pos != null)
            {
                posEqual = this.pos.Equals(t.pos);
            }else if(t.pos == null)
            {
                posEqual = true;
            }
            
            return posEqual && (this.type == t.type) && (this.lexeme == t.lexeme);
        }

        public override string ToString()
        {
            return "(" + type + ", " + lexeme + ")" + pos;
        }
    }
}
