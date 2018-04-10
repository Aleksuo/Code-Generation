using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MiniPascal.FrontEnd.LexicalAnalysis;

namespace MiniPascal.Test.FrontEnd.LexicalAnalysis
{
    class TokenTests
    {
        [Test]
        public void TwoParameterConstructorCreatesAToken()
        {
            Token token = new Token(TokenType.PLUS, "+");
            Assert.AreEqual(TokenType.PLUS, token.type);
            Assert.AreEqual("+", token.lexeme);
            Assert.IsNull(token.pos);
        }

        [Test]
        public void ThreeParameterConstructorCreatesAToken()
        {
            Token token = new Token(TokenType.PLUS, "+", new Position(1, 1));
            Assert.AreEqual(TokenType.PLUS, token.type);
            Assert.AreEqual("+", token.lexeme);
            Assert.AreEqual(1, token.pos.row);
            Assert.AreEqual(1, token.pos.col);
        }
    }
}
