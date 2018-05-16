using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPascal.FrontEnd.LexicalAnalysis
{
    class MockLexer : ILexer
    {
        Queue<Token> tokenStream;

        public MockLexer(params Token[] tokens)
        {
            this.tokenStream = new Queue<Token>();
            for(int i = 0; i < tokens.Length; i++)
            {
                this.tokenStream.Enqueue(tokens[i]);
            }
        }

        public Token nextToken()
        {
            return this.tokenStream.Dequeue();
        }
    }
}
