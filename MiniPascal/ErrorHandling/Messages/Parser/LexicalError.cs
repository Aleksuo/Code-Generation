using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.FrontEnd.LexicalAnalysis;

namespace MiniPascal.ErrorHandling.Messages
{
    class LexicalError : IErrorMessage
    {
        private Token token;

        public LexicalError(Token token)
        {
            this.token = token;
        }
        public void printMessage()
        {
            Console.WriteLine("Error"+token.pos+": Unrecognized token '" + token.lexeme + "'.");
        }
    }
}
