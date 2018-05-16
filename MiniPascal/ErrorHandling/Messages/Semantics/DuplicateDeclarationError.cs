using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.FrontEnd.LexicalAnalysis;

namespace MiniPascal.ErrorHandling.Messages
{
    class DuplicateDeclarationError : IErrorMessage
    {
        private Token token;
        public DuplicateDeclarationError(Token token)
        {
            this.token = token;
        }
        public void printMessage()
        {
            Console.WriteLine("Error "+token.pos+": Duplicate identifier '" + token.lexeme+ "' found.");
        }
    }
}
