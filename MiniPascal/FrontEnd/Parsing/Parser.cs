using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.FrontEnd.LexicalAnalysis;
using MiniPascal.Utils;
using MiniPascal.ErrorHandling;
using MiniPascal.ErrorHandling.Messages;

namespace MiniPascal.FrontEnd.Parsing
{
    public class Parser: IHookable
    {
        private ILexer lexer;
        private Token currentToken;

        public ErrorHook hook { get; set; }

        public Parser(ILexer lexer)
        {
            this.lexer = lexer;
            this.currentToken = this.lexer.nextToken();
            this.hook = new ErrorHook();
        }

        private void eatToken(TokenType expected)
        {
            TokenType curType = this.currentToken.type;
            Console.WriteLine(this.currentToken);
            if(curType == expected)
            {
                Console.WriteLine("Parsed: " + this.currentToken);
                this.currentToken = this.lexer.nextToken();
            }
            else
            {
                this.ThrowErrorMessage(new SyntaxError(this.currentToken));
                this.currentToken = this.lexer.nextToken();
            }
        }

        private AST program()
        {
            //<program> ::= "program" <id> ";" {<procedure> | <function> } <main-block> "."
            List<AST> nodes = new List<AST>();
            Token token = this.currentToken;
            this.eatToken(TokenType.PROGRAM);
            nodes.Add(this.id());
            this.eatToken(TokenType.SEMICOLON);
            while(true)
            {
                if(this.currentToken.type == TokenType.PROCEDURE)
                {
                    nodes.Add(this.procedure());
                }else if(this.currentToken.type == TokenType.FUNCTION)
                {
                    nodes.Add(this.function());
                }
                else
                {
                    break;
                }
            }
            nodes.Add(this.block(true));
            this.eatToken(TokenType.DOT);
            return new Utils.PascalProgram(nodes, token);
        }

        private AST procedure()
        {
            //"procedure" <id> "(" <parameters> ")" ";" <block> ";"
            List<AST> nodes = new List<AST>();
            Token cur = this.currentToken;

            this.eatToken(TokenType.PROCEDURE);
            nodes.Add(this.id());
            this.eatToken(TokenType.LEFTBRACKET);
            nodes.Add(this.parameters());
            this.eatToken(TokenType.RIGHTBRACKET);
            this.eatToken(TokenType.SEMICOLON);
            nodes.Add(this.block(false));
            this.eatToken(TokenType.SEMICOLON);
            return new Procedure(nodes,cur);
        }

        private AST function()
        {
            //"function" <id> "(" <parameters> ")" ":" <type> ";" <block> ";"
            List<AST> nodes = new List<AST>();
            Token cur = this.currentToken;

            this.eatToken(TokenType.FUNCTION);
            nodes.Add(this.id());
            this.eatToken(TokenType.LEFTBRACKET);
            nodes.Add(this.parameters());
            this.eatToken(TokenType.RIGHTBRACKET);
            this.eatToken(TokenType.COLON);
            nodes.Add(this.type());
            this.eatToken(TokenType.SEMICOLON);
            nodes.Add(this.block(false));
            this.eatToken(TokenType.SEMICOLON);
            return new Function(nodes,cur);
        }

        private AST block(bool main)
        {
            //"begin" <statement> {";" <statement>} [";"] "end"
            List<AST> nodes = new List<AST>();
            this.eatToken(TokenType.BEGIN);
            while (true)
            {
                nodes.Add(this.statement());
                if(this.currentToken.type == TokenType.SEMICOLON)
                {
                    this.eatToken(TokenType.SEMICOLON);
                    if(this.currentToken.type == TokenType.END)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            this.eatToken(TokenType.END);
            if (main)
            {
                return new MainBlock(nodes);
            }
            return new Block(nodes);
        }

        private AST statement()
        {
            TokenType type = this.currentToken.type;
            Token cur = this.currentToken;
            List<AST> nodes = new List<AST>();
            if(type == TokenType.ID || type == TokenType.LEFTBRACKET || type == TokenType.RETURN 
                || type == TokenType.READ || type == TokenType.WRITELN || type == TokenType.ASSERT)
            {
                //<statement>  ::= <simple statement>
                return this.simple_statement();
            }
            else if(type == TokenType.BEGIN || type == TokenType.IF || type == TokenType.WHILE)
            {
                //<statement> ::= <structured statement>
                return this.structured_statement();
            }
            //<statement> ::= <var declaration>
            return this.var_declaration();
        }

        private AST simple_statement()
        {
            TokenType type = this.currentToken.type;
            Token cur = this.currentToken;
            List<AST> nodes = new List<AST>();
            if (type == TokenType.ID)
            {
                nodes.Add(this.id());
                type = this.currentToken.type;
                //<call> ::= <id> "(" <arguments> ")"
                if (type == TokenType.LEFTBRACKET)
                {
                    this.eatToken(TokenType.LEFTBRACKET);
                    nodes.Add(this.arguments());
                    this.eatToken(TokenType.RIGHTBRACKET);
                    return new Call(nodes);
                }
                //<assignment statement> ::= <variable> ":=" <expr> 
                cur = this.currentToken;
                this.eatToken(TokenType.ASSIGN);
                nodes.Add(this.expr());
                this.eatToken(TokenType.SEMICOLON);
                return new Assignment(nodes, cur);
            }
            else if (type == TokenType.RETURN)
            {
                //todo: conditional return
                //<return statement> ::= "return" [<expr>] 
                this.eatToken(TokenType.RETURN);
                if(this.currentToken.type != TokenType.SEMICOLON || this.currentToken.type != TokenType.END)
                {
                    nodes.Add(this.expr());
                }              
                return new Return(nodes, cur);
            }
            else if (type == TokenType.READ)
            {
                //<read statement> ::= "read" "(" <variable> {"," <variable>} ")"
                this.eatToken(TokenType.READ);
                this.eatToken(TokenType.LEFTBRACKET);
                while (true)
                {
                    nodes.Add(this.variable());
                    if (this.currentToken.type == TokenType.COMMA)
                    {
                        this.eatToken(TokenType.COMMA);
                    }
                    else
                    {
                        break;
                    }
                }
                this.eatToken(TokenType.LEFTBRACKET);
                return new Read(nodes, cur);
            }
            else if (type == TokenType.WRITELN)
            {
                //<write statement> ::= "writeln" "(" <arguments> ")"
                this.eatToken(TokenType.WRITELN);
                this.eatToken(TokenType.LEFTBRACKET);
                nodes.Add(this.arguments());
                this.eatToken(TokenType.LEFTBRACKET);
                return new Write(nodes, cur);
            }
            else
            {
                //<assert statement> ::= "assert" "(" <Boolean expr> ")"
                this.eatToken(TokenType.ASSERT);
                this.eatToken(TokenType.LEFTBRACKET);
                nodes.Add(this.expr());
                this.eatToken(TokenType.RIGHTBRACKET);
                return new Assert(nodes, cur);
            }
        }
        private AST structured_statement()
        {
            TokenType type = this.currentToken.type;
            Token cur = this.currentToken;
            if (type == TokenType.BEGIN)
            {
                //<structure statement> ::= <block>
                return this.block(false);
            }
            else if (type == TokenType.IF)
            {
                //<structured statement> ::= <if statement>
                return this.if_statement();
            }
            else
            {
                //<structured statement> ::= <while statement>
                return this.while_statement();
            }
        }

        private AST var_declaration()
        {
            List<AST> nodes = new List<AST>();
            Token cur = this.currentToken;
            this.eatToken(TokenType.VAR);
            while (true)
            {
                nodes.Add(this.id());
                if(this.currentToken.type == TokenType.COMMA)
                {
                    this.eatToken(TokenType.COMMA);
                }
                else
                {
                    break;
                }
            }
            this.eatToken(TokenType.COLON);
            nodes.Add(this.type());
            return new VarDeclaration(nodes, cur);
        }

        private AST type()
        {
            //<type> ::= <simple type>
            TokenType type = this.currentToken.type;
            if(type == TokenType.TYPE)
            {
                return this.simple_type();
            }
            //<type> ::= <array type>
            return this.array_type();
        }

        private AST simple_type()
        {
            TokenType type = this.currentToken.type;
            Token cur = this.currentToken;
            this.eatToken(TokenType.TYPE);
            return new Utils.Type(cur);
        }

        private AST array_type()
        {
            TokenType type = this.currentToken.type;
            Token cur = this.currentToken;
            List<AST> nodes = new List<AST>();
            this.eatToken(TokenType.ARRAY);
            this.eatToken(TokenType.LEFTSQUARE);
            if (this.currentToken.type != TokenType.RIGHTSQUARE)
            {
                nodes.Add(this.expr());
            }
            this.eatToken(TokenType.RIGHTSQUARE);
            this.eatToken(TokenType.OF);
            nodes.Add(this.simple_type());
            return new Utils.Array(nodes, cur);
        }



        private AST if_statement()
        {
            TokenType type = this.currentToken.type;
            Token cur = this.currentToken;
            List<AST> nodes = new List<AST>();
            this.eatToken(TokenType.IF);
            nodes.Add(this.expr());
            this.eatToken(TokenType.THEN);
            nodes.Add(this.statement());
            if (this.currentToken.type == TokenType.ELSE) {
                this.eatToken(TokenType.ELSE);
                nodes.Add(this.statement());
            }
            return new If(nodes, cur);
        }

        private AST while_statement()
        {
            TokenType type = this.currentToken.type;
            Token cur = this.currentToken;
            List<AST> nodes = new List<AST>();

            this.eatToken(TokenType.WHILE);
            nodes.Add(this.expr());
            this.eatToken(TokenType.DO);
            nodes.Add(this.statement());
            return new While(nodes,cur);
        }

        private AST expr()
        {
            List<AST> nodes = new List<AST>();
            AST simple = this.simple_expr();
            nodes.Add(simple);
            TokenType type = this.currentToken.type;
            if(type == TokenType.EQUALS || type == TokenType.GREATER || type == TokenType.GREATEROREQUAL 
                || type == TokenType.LESS || type == TokenType.LESSOREQUAL || type == TokenType.NOTEQUAL)
            {
                type = this.currentToken.type;
                Token cur = this.currentToken;
                if(type == TokenType.EQUALS)
                {
                    this.eatToken(TokenType.EQUALS);
                }else if (type == TokenType.GREATER)
                {
                    this.eatToken(TokenType.GREATER);
                }else if(type == TokenType.GREATEROREQUAL)
                {
                    this.eatToken(TokenType.GREATEROREQUAL);
                }else if(type == TokenType.LESS)
                {
                    this.eatToken(TokenType.LESS);
                }else if(type == TokenType.LESSOREQUAL)
                {
                    this.eatToken(TokenType.LESSOREQUAL);
                }
                else
                {
                    this.eatToken(TokenType.NOTEQUAL);
                }
                nodes.Add(this.simple_expr());
                return new RelationalOp(nodes, cur);
            }
            return simple;
        }

        private AST simple_expr() { 

        
            //<simple expr> ::= [sign]<term> {<adding operator> <term>}
            TokenType type = this.currentToken.type;
            Token cur = this.currentToken;
            List<AST> nodes = new List<AST>();
            if (type == TokenType.PLUS || type == TokenType.MINUS)
            {
                // <sign><term>...
                if(type == TokenType.PLUS)
                {
                    this.eatToken(TokenType.PLUS);
                }
                this.eatToken(TokenType.MINUS);
                List<AST> unaryNodes = new List<AST>();
                unaryNodes.Add(this.term());
                nodes.Add(new UnaryOp(unaryNodes, cur));
            }
            else
            {
                // <term>...
                nodes.Add(this.term());
            }
            type = this.currentToken.type;
            cur = this.currentToken;
            if (type != TokenType.PLUS && type != TokenType.MINUS && type != TokenType.OR)
            {
                return nodes[0];
            }
            AST node = null;
            while (true)
            {
                type = this.currentToken.type;
                cur = this.currentToken;
                if (type == TokenType.PLUS || type == TokenType.MINUS || type == TokenType.OR)
                {
                   
                    if (type == TokenType.PLUS)
                    {
                        this.eatToken(TokenType.PLUS);
                    } else if (type == TokenType.MINUS)
                    {
                        this.eatToken(TokenType.MINUS);
                    }
                    else
                    {
                        this.eatToken(TokenType.OR);
                    }
                    nodes.Add(this.term());
                    node = new AddingOp(nodes, cur);
                    nodes = new List<AST>();
                    nodes.Add(node);
                }
                else
                {
                    break;
                }              
            }
            return node;
        }

        private AST term()
        {
            //<term> ::= <factor>{<multiplying operator> <factor>>
            
            List<AST> nodes = new List<AST>();
            AST node = this.factor();
            TokenType type = this.currentToken.type;
            Token cur = this.currentToken;
            nodes.Add(node);
            while (true)
            {
                type = this.currentToken.type;
                cur = this.currentToken;
                if (type == TokenType.MULT || type == TokenType.DIV || type == TokenType.MOD ||type == TokenType.AND)
                {
                    this.eatToken(type);
                }
                else
                {
                    break;
                }
                nodes.Add(this.factor());
                node = new MultiplyingOp(nodes, cur);
                nodes = new List<AST>();
                nodes.Add(node);
           }
            
            return node;
        }

        private AST factor()
        {
            TokenType type = this.currentToken.type;
            Token cur = this.currentToken;
            List<AST> nodes = new List<AST>();

            if(type == TokenType.ID)
            {
                nodes.Add(this.id());
                //this.eatToken(TokenType.ID);
                //<call> ::= <id> "(" <arguments> ")"
                type = this.currentToken.type;
                if (type == TokenType.LEFTBRACKET)
                {
                    this.eatToken(TokenType.LEFTBRACKET);
                    nodes.Add(this.arguments());
                    this.eatToken(TokenType.RIGHTBRACKET);
                    return new Call(nodes);
                }
                //<variable> ::= <variable id> ["["<integer expr>"]"]
              
                if(type == TokenType.LEFTSQUARE)
                {
                    this.eatToken(TokenType.LEFTSQUARE);
                    nodes.Add(this.expr());
                    this.eatToken(TokenType.RIGHTSQUARE);
                }
                return new Variable(nodes, null);
            }else if(type == TokenType.LEFTBRACKET)
            {
                //<factor> ::= "(" <expr> ")"
                this.eatToken(TokenType.LEFTBRACKET);
                AST node = this.expr();
                this.eatToken(TokenType.RIGHTBRACKET);
                return node;
            }else if(type == TokenType.INTEGER)
            {
                AST node = new Integer(cur);
                this.eatToken(TokenType.INTEGER);
                return node;
            }else if(type == TokenType.STRING)
            {
                AST node = new Utils.String(cur);
                this.eatToken(TokenType.STRING);
                return node;
            }else if(type == TokenType.REAL)
            {
                AST node = new Real(cur);
                this.eatToken(TokenType.REAL);
                return node;
            }else if(type == TokenType.NOT)
            {
                this.eatToken(TokenType.NOT);
                nodes.Add(this.factor());
                return new UnaryOp(nodes, cur);
            }else if(type == TokenType.BOOLEAN)
            {
                AST node = new Utils.Boolean(cur);
                this.eatToken(TokenType.BOOLEAN);
                return node;
            }
            return null;
        }

            
        private AST variable()
        {
            TokenType type = this.currentToken.type;
            Token cur = this.currentToken;
            List<AST> nodes = new List<AST>();
            nodes.Add(this.id());
            this.eatToken(TokenType.ID);
            type = this.currentToken.type;
            if (type == TokenType.LEFTSQUARE)
            {
                this.eatToken(TokenType.LEFTSQUARE);
                nodes.Add(this.expr());
                this.eatToken(TokenType.RIGHTSQUARE);
            }
            return new Variable(nodes, null);
        }
        

        private AST id()
        {

            Token cur = this.currentToken;
            this.eatToken(TokenType.ID);
            AST id = new Identifier(cur);
            return id;
        }

       private AST arguments()
       {
            TokenType type = this.currentToken.type;
            Token cur = this.currentToken;
            List<AST> nodes = new List<AST>();

            if(type == TokenType.RIGHTBRACKET)
            {
                return null;
            }
            nodes.Add(this.expr());
            while (true)
            {
                type = this.currentToken.type;
                cur = this.currentToken;

                if(type != TokenType.COMMA)
                {
                    break;
                }
                this.eatToken(TokenType.COMMA);
                nodes.Add(this.expr());
            }
            return new Arguments(nodes);
       }

        private AST parameters()
        {
            TokenType type = this.currentToken.type;
            Token cur = this.currentToken;
            List<AST> nodes = new List<AST>();
            if(type == TokenType.RIGHTBRACKET)
            {
                return null;
            }

            while (true)
            {
                
                if(type == TokenType.VAR)
                {
                    List<AST> refNodes = new List<AST>();
                    this.eatToken(TokenType.VAR);
                    refNodes.Add(this.id());
                    this.eatToken(TokenType.COLON);
                    refNodes.Add(this.type());
                    nodes.Add(new Reference(refNodes));
                }
                else
                {
                    List<AST> paramNodes = new List<AST>();
                    paramNodes.Add(this.id());
                    this.eatToken(TokenType.COLON);
                    paramNodes.Add(this.type());
                    nodes.Add(new Parameter(paramNodes));
                }
                if(this.currentToken.type == TokenType.COMMA)
                {
                    this.eatToken(TokenType.COMMA);
                    type = this.currentToken.type;
                }
                else
                {
                    break;
                }

            }
            return new Parameters(nodes);
        }

        

        public AST parse()
        {
            return program();
        }
    }
}
