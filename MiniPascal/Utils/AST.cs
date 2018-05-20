using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MiniPascal.FrontEnd.LexicalAnalysis;

namespace MiniPascal.Utils
{
    public class AST
    {
        public Token token;
        public List<AST> nodes;
        public int nodeID;

        public BuiltType type;

        private static int globalNodeID;

        public AST(List<AST> nodes, Token token)
        {
            this.token = token;
            this.nodes = nodes;
            this.nodeID = Interlocked.Increment(ref globalNodeID);
        }
    }

    public class PascalProgram : AST
    {
        public PascalProgram(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class Block : AST
    {
        public Block(List<AST> nodes) : base(nodes, null) { }
    }

    public class MainBlock : AST
    {
        public MainBlock(List<AST> nodes) : base(nodes, null) { }
    }

    public class Procedure : AST
    {
        public Procedure(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class Function : AST
    {
        public Function(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class VarDeclaration : AST
    {
        public VarDeclaration(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class Assignment : AST
    {
        public Assignment(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class Call : AST
    {
        public Call(List<AST> nodes) : base(nodes, null) { }
    }

    public class Arguments : AST
    {
        public Arguments(List<AST> nodes) : base(nodes,null) { }
    }

    public class Return : AST
    {
        public Return(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class Assert : AST
    {
        public Assert(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class Read : AST
    {
        public Read(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class Write : AST
    {
        public Write(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class While : AST
    {
        public While(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class If : AST
    {
        public If(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class Identifier : AST
    {
        public Identifier(Token token) : base(null, token) { }

    }

    public class Type : AST
    {
        public Type(Token token) : base(null, token) { }
    }

    public class Variable : AST
    {
        public Variable(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class Integer : AST
    {
        public Integer(Token token) : base(null, token) { }
    }

    public class String : AST
    {
        public String(Token token) : base(null, token) { }
    }

    public class Boolean : AST
    {
        public Boolean(Token token) : base(null, token) { }
    }

    public class Array : AST
    {
        public Array(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class Real : AST
    {
        public Real(Token token) : base(null, token) { }
    }

    public class RelationalOp : AST
    {
        public RelationalOp(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class AddingOp : AST
    {
        public AddingOp(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class MultiplyingOp : AST
    {
        public MultiplyingOp(List<AST> nodes, Token token) : base(nodes, token) { }
    }
    public class UnaryOp : AST
    {
        public UnaryOp(List<AST> nodes, Token token) : base(nodes, token) { }
    }

    public class Parameters : AST
    {
        public Parameters(List<AST> nodes) : base(nodes, null) { }
    }

    public class Parameter : AST
    {
        public Parameter(List<AST> nodes) : base(nodes, null) { }
    }

    public class Reference : AST
    {
        public Reference(List<AST> nodes) : base(nodes, null) { }
    }

    public class Error : AST
    {
        public Error() : base(null, null) { }
    }



    
}
