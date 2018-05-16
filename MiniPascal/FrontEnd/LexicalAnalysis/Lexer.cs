using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.Utils.Source;

namespace MiniPascal.FrontEnd.LexicalAnalysis
{
    public class Lexer : ILexer
    {
        private ISource input;

        private int row;
        private int col;

        private Dictionary<char, Token> oneCharTokens;
        private Dictionary<string, Token> keywords;

        public Lexer(ISource input)
        {
            this.input = input;
            this.row = 1;
            this.col = 1;
            this.initializeOneCharTokens();
            this.initializeKeywords();
        }

        private void initializeOneCharTokens()
        {
            this.oneCharTokens = new Dictionary<char, Token>();

            this.oneCharTokens['+'] = new Token(TokenType.PLUS, "+");
            this.oneCharTokens['-'] = new Token(TokenType.MINUS, "-");
            this.oneCharTokens['('] = new Token(TokenType.LEFTBRACKET, "(");
            this.oneCharTokens[')'] = new Token(TokenType.RIGHTBRACKET, ")");
            this.oneCharTokens['*'] = new Token(TokenType.MULT, "*");
            this.oneCharTokens['%'] = new Token(TokenType.MOD, "%");
            this.oneCharTokens['['] = new Token(TokenType.LEFTSQUARE, "[");
            this.oneCharTokens[']'] = new Token(TokenType.RIGHTSQUARE, "]");
            this.oneCharTokens['.'] = new Token(TokenType.DOT, ".");
            this.oneCharTokens[';'] = new Token(TokenType.SEMICOLON, ";");
            this.oneCharTokens[','] = new Token(TokenType.COMMA, ",");
            this.oneCharTokens['/'] = new Token(TokenType.DIV, "/");
        }

        private void initializeKeywords()
        {
            this.keywords = new Dictionary<string, Token>();

            this.keywords["if"] = new Token(TokenType.IF, "if");
            this.keywords["then"] = new Token(TokenType.THEN, "then");
            this.keywords["else"] = new Token(TokenType.ELSE, "else");
            this.keywords["function"] = new Token(TokenType.FUNCTION, "function");
            this.keywords["program"] = new Token(TokenType.PROGRAM, "program");
            this.keywords["return"] = new Token(TokenType.RETURN, "return");
            this.keywords["of"] = new Token(TokenType.OF, "of");
            this.keywords["procedure"] = new Token(TokenType.PROCEDURE, "procedure");
            this.keywords["do"] = new Token(TokenType.DO, "do");
            this.keywords["begin"] = new Token(TokenType.BEGIN, "begin");
            this.keywords["end"] = new Token(TokenType.END, "end");
            this.keywords["assert"] = new Token(TokenType.ASSERT, "assert");
            this.keywords["while"] = new Token(TokenType.WHILE, "while");
            this.keywords["array"] = new Token(TokenType.ARRAY, "array");
            this.keywords["read"] = new Token(TokenType.READ, "read");
            this.keywords["writeln"] = new Token(TokenType.WRITELN, "writeln");
            this.keywords["integer"] = new Token(TokenType.TYPE, "integer");
            this.keywords["Boolean"] = new Token(TokenType.TYPE, "Boolean");
            this.keywords["real"] = new Token(TokenType.TYPE, "real");
            this.keywords["false"] = new Token(TokenType.BOOLEAN, "false");
            this.keywords["true"] = new Token(TokenType.BOOLEAN, "true");
            this.keywords["not"] = new Token(TokenType.NOT, "not");
            this.keywords["var"] = new Token(TokenType.VAR, "var");
        }

        private void advance()
        {
            this.col++;
            this.input.advance();
        }

        private void newLine()
        {
            this.col = 0;
            this.row++;
            this.advance();
        }

        private Token colonOrAssign()
        {
            Position pos = new Position(row, col);
            this.advance();
            if(this.input.currentChar() == '=')
            {
                this.advance();
                return new Token(TokenType.ASSIGN, ":=", pos);
            }
            return new Token(TokenType.COLON, ":", pos);
        }

        private Token lessorequalOrLessOrNotequal()
        {
            Position pos = new Position(row, col);
            this.advance();
            if (this.input.currentChar() == '=')
            {
                this.advance();
                return new Token(TokenType.LESSOREQUAL, "<=", pos);
            } else if (this.input.currentChar() == '>')
            {
                this.advance();
                return new Token(TokenType.NOTEQUAL, "<>", pos);
            }
            return new Token(TokenType.LESS, "<", pos);

        }

        private Token greaterOrGreaterorequal()
        {
            Position pos = new Position(row, col);
            this.advance();
            if(this.input.currentChar() == '=')
            {
                this.advance();
                return new Token(TokenType.GREATEROREQUAL, ">=", pos);
            }
            return new Token(TokenType.GREATER, ">", pos);
        }

        private Token integerOrReal()
        {
            Position pos = new Position(row, col);
            StringBuilder builder = new StringBuilder();
            char cur = (char)this.input.currentChar();
            char? next = input.peekNextChar();
            builder.Append(cur);
            bool error = false;
            if (cur == '0' &&  next!=null && Char.IsDigit((char)next))
            {
                error = true;
            }
            this.advance();
            if(this.input.currentChar() != null)
            {
                cur = (char)this.input.currentChar();
            }
            while (Char.IsLetterOrDigit(cur) || cur == '_')
            {
                if (this.input.currentChar() == null)
                    break;
                if (Char.IsLetter(cur))
                {
                    error = true;
                }
                cur = (char)this.input.currentChar();
                builder.Append(cur);
                this.advance();
            }

            if (error)
            {
                return new Token(TokenType.ERROR, builder.ToString(), pos);
            }
            return new Token(TokenType.INTEGER, builder.ToString(), pos);           
        }

        private Token identifier()
        {
            Position pos = new Position(row, col);
            StringBuilder builder = new StringBuilder();
            char cur = (char)this.input.currentChar();
            builder.Append(cur);
            this.advance();
            while (Char.IsLetterOrDigit(cur) || cur == '_')
            {
                if (this.input.currentChar() == null)
                    break;
                cur = (char)this.input.currentChar();
                if(!Char.IsLetterOrDigit(cur) && cur != '_')
                {
                    break;
                }
                builder.Append(cur);
                this.advance();
            }
            if (this.keywords.ContainsKey(builder.ToString()))
            {
                Token t = this.keywords[builder.ToString()];
                t.pos = pos;
                return t;
            }
            return new Token(TokenType.ID, builder.ToString(), pos);
        }

        private Token stringLiteral()
        {
            Position pos = new Position(row, col);
            StringBuilder builder = new StringBuilder();
            this.advance();
            char? cur = this.input.currentChar();
            while(cur !=null && (char)cur != '"'){
                builder.Append(cur);
                this.advance();
                cur = this.input.currentChar();
            }
            this.advance();
            return new Token(TokenType.STRING, builder.ToString(), pos);
        }


        private bool skipComment()
        {
            bool skipped = false;
            char? cur = this.input.currentChar();
            if(cur!=null && cur == '{')
            {
                char? next = this.input.peekNextChar();
                if(next !=null && next == '*')
                {
                    skipped = true;
                    while (true)
                    {
                        if(cur == null)
                        {
                            break;
                        }
                        if(cur == '*')
                        {
                            next = this.input.peekNextChar();
                            if(next !=null && next == '}')
                            {
                                this.advance();
                                this.advance();
                                break;
                            }
                        }
                        if(cur == '\n')
                        {
                            this.newLine();
                        }
                        else
                        {
                            this.advance();
                        }
                        cur = this.input.currentChar();
                    }
                }
            }
            return skipped;
        }

        private bool skipWhiteSpaceAndNewLines()
        {
            bool skipped = false;
            char? cur = this.input.currentChar();
            while(cur !=null && (cur == ' ' || cur == '\n' || cur == '\r' || cur == '\t'))
            {
                skipped = true;
                if(cur == ' ')
                {
                    this.advance();
                }else if(cur == '\t')
                {
                    this.advance();
                    this.col += 3;
                }
                else
                {
                    this.newLine();
                }
                cur = this.input.currentChar();
            }
            return skipped;
        }


        public Token nextToken()
        {
            bool scanning = true;
            while (scanning)
            {
                scanning = false;
                if (skipComment())
                    scanning = true;
                if (skipWhiteSpaceAndNewLines())
                    scanning = true;
            }

            if(this.input.currentChar() != null)
            {
                char cur = (char)input.currentChar();
                if (this.oneCharTokens.ContainsKey(cur))
                {
                    Token token = this.oneCharTokens[cur];
                    token.pos = new Position(this.row, this.col);
                    this.advance();
                    return token;
                }else if(cur == ':')
                {
                    return colonOrAssign();
                }else if(cur == '<')
                {
                    return lessorequalOrLessOrNotequal();
                }else if(cur == '>')
                {
                    return greaterOrGreaterorequal();
                }else if (Char.IsLetter(cur))
                {
                    return identifier();
                }else if (Char.IsDigit(cur))
                {
                    return integerOrReal();
                }else if (cur == '"')
                {
                    return stringLiteral();
                }
                return new Token(TokenType.ERROR, "");
            }
            return new Token(TokenType.EOF, "eof");
            
        }
    }
}
