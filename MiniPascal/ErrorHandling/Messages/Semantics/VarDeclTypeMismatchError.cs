using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.FrontEnd.LexicalAnalysis;
using MiniPascal.Utils;

namespace MiniPascal.ErrorHandling.Messages
{
    class VarDeclTypeMismatchError : IErrorMessage
    {
        AST var;
        AST expr;

        public VarDeclTypeMismatchError(AST var, AST expr)
        {
            this.var = var;
            this.expr = expr;
        }

        public void printMessage()
        {
            Console.WriteLine("Error " + var.token.pos + ": Cannot assign " + expr.type + " to variable of type " + var.type + ".");
        }
    }
}
