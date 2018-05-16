using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.ErrorHandling.Messages;
using MiniPascal.Utils;

namespace MiniPascal.ErrorHandling.Messages
{
    class UnaryOperandTypeError : IErrorMessage
    {
        AST expr;
        AST op;

        public UnaryOperandTypeError(AST expr, AST op)
        {
            this.expr = expr;
            this.op = op;
        }
        public void printMessage()
        {
            Console.WriteLine("Error "+ op.token.pos+ ": Type " + expr.type + " is not supported for operator '" + op.token.lexeme + "'.");
        }
    }
}
