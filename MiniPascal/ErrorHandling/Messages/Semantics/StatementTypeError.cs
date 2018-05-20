using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.Utils;


namespace MiniPascal.ErrorHandling.Messages
{
    public class StatementTypeError : IErrorMessage
    {

        AST statement;
        BuiltType found;
        BuiltType expected;
            

            public StatementTypeError(AST statement, BuiltType found, BuiltType expected)
            {
            this.statement = statement;
            this.found = found;
            this.expected = expected;
            }
            public void printMessage()
            {
                Console.WriteLine(this.statement.token.pos +"Error: Statement '" + statement.token.lexeme+ "' expects expression of type " + expected + ", found: " + found+ ".");
            }
        
    }
}
