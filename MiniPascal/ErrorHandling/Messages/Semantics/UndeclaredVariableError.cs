using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.FrontEnd.LexicalAnalysis;

namespace MiniPascal.ErrorHandling.Messages
{
    class UndeclaredVariableError : IErrorMessage
    {
        private Token var;
        public UndeclaredVariableError(Token var)
        {
            this.var = var;
        }

        public void printMessage()
        {
            Console.WriteLine("Error "+var.pos+": Variable '" + this.var.lexeme+ "' is undeclared.");
        }
    }
}
