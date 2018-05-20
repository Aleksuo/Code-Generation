using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MiniPascal.FrontEnd.LexicalAnalysis;
using MiniPascal.Utils.Source;

namespace MiniPascal.Test.FrontEnd.LexicalAnalysis
{
    class LexerTests
    {
        public void helper(string input, Token expected)
        {
            ISource source = new StringSource(input);
            Lexer lexer = new Lexer(source);
            Assert.AreEqual(expected, lexer.nextToken());
        }

        [Test]
        public void PlusTokenIsLexed()
        {
            helper("+", new Token(TokenType.PLUS, "+", new Position(1,1)));
        }

        [Test]
        public void MinusTokenIsLexed()
        {
            helper("-", new Token(TokenType.MINUS, "-", new Position(1, 1)));
        }

        [Test]
        public void MultTokenIsLexed()
        {
            helper("*", new Token(TokenType.MULT, "*", new Position(1, 1)));
        }

        [Test]
        public void ModTokenIsLexed()
        {
            helper("%", new Token(TokenType.MOD, "%", new Position(1, 1)));
        }

        [Test]
        public void DivTokenIsLexed()
        {
            helper("/", new Token(TokenType.DIV, "/", new Position(1, 1)));
        }

        [Test]
        public void LBracketTokenIsLexed()
        {
            helper("(", new Token(TokenType.LEFTBRACKET, "(", new Position(1, 1)));
        }

        [Test]
        public void RBracketTokenIsLexed()
        {
            helper(")", new Token(TokenType.RIGHTBRACKET, ")", new Position(1, 1)));
        }

        [Test]
        public void LBlockTokenIsLexed()
        {
            helper("[", new Token(TokenType.LEFTSQUARE, "[", new Position(1, 1)));
        }

        [Test]
        public void RBlockTokenIsLexed()
        {
            helper("]", new Token(TokenType.RIGHTSQUARE, "]", new Position(1, 1)));
        }

        [Test]
        public void DotTokenIsLexed()
        {
            helper(".", new Token(TokenType.DOT, ".", new Position(1, 1)));
        }

        [Test]
        public void SemicolonTokenIsLexed()
        {
            helper(";", new Token(TokenType.SEMICOLON, ";", new Position(1, 1)));
        }

        [Test]
        public void ColonTokenIsLexed()
        {
            helper(":", new Token(TokenType.COLON, ":", new Position(1, 1)));
        }

        [Test]
        public void AssignTokenIsLexed()
        {
            helper(":=", new Token(TokenType.ASSIGN, ":=", new Position(1, 1)));
        }

        [Test]
        public void CommaTokenIsLexed()
        {
            helper(",", new Token(TokenType.COMMA, ",", new Position(1, 1)));
        }

        [Test]
        public void LessTokenIsLexed()
        {
            helper("<", new Token(TokenType.LESS, "<", new Position(1, 1)));
        }

        [Test]
        public void LessOrEqualTokenIsLexed()
        {
            helper("<=", new Token(TokenType.LESSOREQUAL, "<=", new Position(1, 1)));
        }

        [Test]
        public void NotEqualTokenIsLexed()
        {
            helper("<>", new Token(TokenType.NOTEQUAL, "<>", new Position(1, 1)));
        }

        [Test]
        public void GreaterTokenIsLexed()
        {
            helper(">", new Token(TokenType.GREATER, ">", new Position(1, 1)));
        }

        [Test]
        public void GreaterOrEqualTokenIsLexed()
        {
            helper(">=", new Token(TokenType.GREATEROREQUAL, ">=", new Position(1, 1)));
        }
        
        //identifiers

        [Test]
        public void IdentifiersAreRecognized()
        {
            helper("idTest", new Token(TokenType.ID, "idtest", new Position(1, 1)));
        }

        [Test]
        public void IdentifiersAreRecognized2()
        {
            helper("idTest123", new Token(TokenType.ID, "idtest123", new Position(1, 1)));
        }

        [Test]
        public void IdentifiersAreRecognized3()
        {
            helper("id_test", new Token(TokenType.ID, "id_test", new Position(1, 1)));
        }

        //integers

        [Test]
        public void IntegerTokensAreRecognized()
        {
            helper("1", new Token(TokenType.INTEGER, "1", new Position(1, 1)));
        }

        [Test]
        public void IntegerTokensAreRecognized2()
        {
            helper("99911", new Token(TokenType.INTEGER, "99911", new Position(1, 1)));
        }

        /*
        [Test]
        public void IntegersPrefixProducesError()
        {
            helper("999aaa", new Token(TokenType.ERROR, "999aaa", new Position(1, 1)));
        }
        */

        [Test]
        public void ZeroIsLexed()
        {
            helper("0", new Token(TokenType.INTEGER, "0", new Position(1, 1)));
        }

        [Test]
        public void IntegersCannotStartWithZero()
        {
            helper("01", new Token(TokenType.ERROR, "01", new Position(1, 1)));
        }

        //reals

        //strings

        [Test]
        public void StringTokensAreLexed()
        {
            helper("\" This is a string \"", new Token(TokenType.STRING, " This is a string ", new Position(1, 1)));
        }

        //keywords

        [Test]
        public void IfTokenIsLexed()
        {
            helper("if", new Token(TokenType.IF, "if", new Position(1, 1)));
        }

        [Test]
        public void ThenTokenIsLexed()
        {
            helper("then", new Token(TokenType.THEN, "then", new Position(1, 1)));
        }

        [Test]
        public void ElseTokenIsLexed()
        {
            helper("else", new Token(TokenType.ELSE, "else", new Position(1, 1)));
        }

        [Test]
        public void FunctionTokenIsLexed()
        {
            helper("function", new Token(TokenType.FUNCTION, "function", new Position(1, 1)));
        }

        [Test]
        public void ProgramTokenIsLexed()
        {
            helper("program", new Token(TokenType.PROGRAM, "program", new Position(1, 1)));
        }

        [Test]
        public void ReturnTokenIsLexed()
        {
            helper("return", new Token(TokenType.RETURN, "return", new Position(1, 1)));
        }

        [Test]
        public void OfTokenIsLexed()
        {
            helper("of", new Token(TokenType.OF, "of", new Position(1, 1)));
        }

        [Test]
        public void ProcedureTokenIsLexed()
        {
            helper("procedure", new Token(TokenType.PROCEDURE, "procedure", new Position(1, 1)));
        }

        [Test]
        public void DoTokenIsLexed()
        {
            helper("do", new Token(TokenType.DO, "do", new Position(1, 1)));
        }

        [Test]
        public void BeginTokenIsLexed()
        {
            helper("begin", new Token(TokenType.BEGIN, "begin", new Position(1, 1)));
        }

        [Test]
        public void EndTokenIsLexed()
        {
            helper("end", new Token(TokenType.END, "end", new Position(1, 1)));
        }

        [Test]
        public void AssertTokenIsLexed()
        {
            helper("assert", new Token(TokenType.ASSERT, "assert", new Position(1, 1)));
        }

        [Test]
        public void WhileTokenIsLexed()
        {
            helper("while", new Token(TokenType.WHILE, "while", new Position(1, 1)));
        }

        [Test]
        public void ArrayTokenIsLexed()
        {
            helper("array", new Token(TokenType.ARRAY, "array", new Position(1, 1)));
        }

        [Test]
        public void ReadTokenIsLexed()
        {
            helper("read", new Token(TokenType.READ, "read", new Position(1, 1)));
        }

        [Test]
        public void WritelnTokenIsLexed()
        {
            helper("writeln", new Token(TokenType.WRITELN, "writeln", new Position(1, 1)));
        }

        [Test]
        public void IntegerTypeTokenIsLexed()
        {
            helper("integer", new Token(TokenType.TYPE, "integer", new Position(1, 1)));
        }

        [Test]
        public void BooleanTypeTokenIsLexed()
        {
            helper("Boolean", new Token(TokenType.TYPE, "Boolean", new Position(1, 1)));
        }

        [Test]
        public void RealTypeTokenIsLexed()
        {
            helper("real", new Token(TokenType.TYPE, "real", new Position(1, 1)));
        }

        [Test]
        public void BooleanTrueIsLexed()
        {
            helper("true", new Token(TokenType.BOOLEAN, "true", new Position(1, 1)));
        }

        [Test]
        public void BooleanFalseIsLexed()
        {
            helper("false", new Token(TokenType.BOOLEAN, "false", new Position(1, 1)));
        }

        [Test]
        public void NotTokenIsLexed()
        {
            helper("not", new Token(TokenType.NOT, "not", new Position(1, 1)));
        }

        //Scanning

        [Test]
        public void WhiteSpacesAreSkipped()
        {
            helper("            1", new Token(TokenType.INTEGER, "1", new Position(1, 13)));
        }

        [Test]
        public void WhiteSpacesAreSkipped2()
        {
            helper("1              ", new Token(TokenType.INTEGER, "1", new Position(1, 1)));
        }

        [Test]
        public void NewLinesAreSkipped()
        {
            helper("\n\n 1", new Token(TokenType.INTEGER, "1", new Position(3, 2)));
        }

        [Test]
        public void NewLinesAreSkipped2()
        {
            helper("1\n\n", new Token(TokenType.INTEGER, "1", new Position(1, 1)));
        }

        [Test]
        public void SingleLineCommentsAreSkipped()
        {
            helper("{* This is a comment.*} 1", new Token(TokenType.INTEGER, "1",new Position(1,25)));

        }

        [Test]
        public void SingleLineCommentsAreSkipped2()
        {
            helper("1 {* This is a comment. * }", new Token(TokenType.INTEGER, "1", new Position(1, 1)));
        }

        [Test]
        public void MultilineCommentsAreSkipped()
        {
            helper("{* this is \n a multiline comment. *} 1", new Token(TokenType.INTEGER, "1", new Position(2, 26)));
        }

        [Test]
        public void MultilineCommentsAreSkipped2()
        {
            helper("1 {*this is \n \n a multiline comment.\n*}", new Token(TokenType.INTEGER, "1", new Position(1, 1)));
        }

        [Test]
        public void MultipleMultilineCommentsAreSkipped()
        {
            helper("{* a \n comment\n \n*} 1 {* another\n comment\n*}", new Token(TokenType.INTEGER, "1", new Position(4, 4)));
        }

    }
}
