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

        [Test]
        public void TokensAreEqualWithoutPosition()
        {
            Token t1 = new Token(TokenType.PLUS, "+");
            Token t2 = new Token(TokenType.PLUS, "+");
            Assert.AreEqual(t1, t2);
        }

        [Test]
        public void TokensAreEqualWithPosition()
        {
            Token t1 = new Token(TokenType.PLUS, "+", new Position(1,1));
            Token t2 = new Token(TokenType.PLUS, "+", new Position(1,1));
            Assert.AreEqual(t1, t2);
        }

        [Test]
        public void TokensAreNotEqualWithDifferentPositions()
        {
            Token t1 = new Token(TokenType.PLUS, "+", new Position(1, 1));
            Token t2 = new Token(TokenType.PLUS, "+", new Position(1, 2));
            Assert.AreNotEqual(t1, t2);
        }

        [Test]
        public void TokenWithPositionIsNotEqualToTokenWithoutPosition()
        {
            Token t1 = new Token(TokenType.PLUS, "+", new Position(1, 1));
            Token t2 = new Token(TokenType.PLUS, "+");
            Assert.AreNotEqual(t1, t2);
        }

        [Test]
        public void TokensAreNotEqualWithDifferentType(){
            Token t = new Token(TokenType.PLUS, "+", new Position(1, 1));
            int a = 0;
            Assert.AreNotEqual(t,a);
        }
    }
    
}
