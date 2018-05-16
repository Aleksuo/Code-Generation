using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.Utils;

namespace MiniPascal.ErrorHandling.Messages
{
    class TypeNotSupportedError : IErrorMessage
    {
        AST node;
        AST expr;

        public TypeNotSupportedError(AST node, AST expr)
        {
            this.node = node;
            this.expr = expr;
        }

        public void printMessage()
        {
            Console.WriteLine("Error "+expr.token.pos+": Type " + expr.type + " is not supported for '" + node.token.lexeme + "'.");
        }
    }
}
