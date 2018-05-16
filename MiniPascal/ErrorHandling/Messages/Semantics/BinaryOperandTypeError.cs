using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.Utils;

namespace MiniPascal.ErrorHandling.Messages
{
    class BinaryOperandTypeError : IErrorMessage
    {
        AST left;
        AST right;
        AST op;

        public BinaryOperandTypeError(AST left, AST right, AST op)
        {
            this.left = left;
            this.right = right;
            this.op = op;
        }
        public void printMessage()
        {
            Console.WriteLine("Error "+this.op.token.pos+": Operator '" + op.token.lexeme + "' is not supported for types " + left.type + " and " + right.type + ".");
        }
    }
}
