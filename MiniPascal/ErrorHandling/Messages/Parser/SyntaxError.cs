using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.FrontEnd.LexicalAnalysis;

namespace MiniPascal.ErrorHandling.Messages
{
    class SyntaxError : IErrorMessage
    {
        private Token token;

        public SyntaxError(Token token)
        {
            this.token = token;
        }

        public void printMessage()
        {
            Console.WriteLine("Syntax error "+token.pos+": Unexpected symbol '" + token.lexeme + "'.");
        }
    }
}
