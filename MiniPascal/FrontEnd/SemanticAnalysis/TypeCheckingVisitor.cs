using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.Utils;
using MiniPascal.ErrorHandling;
using MiniPascal.ErrorHandling.Messages;
using MiniPascal.FrontEnd.LexicalAnalysis;

namespace MiniPascal.FrontEnd.SemanticAnalysis
{
    public class TypeCheckingVisitor : NodeVisitor, IHookable
    {
        public ErrorHook hook { get; set; }

        private SymbolTableManager stm;

        public TypeCheckingVisitor(SymbolTableManager stm)
        {
            this.stm = stm;
            hook = new ErrorHook();
        }

        public void visit_PascalProgram(AST node)
        {
            this.stm.enterScope("global", 1, node.nodeID);
            for (int i = 1; i < node.nodes.Count(); i++)
            {
                this.visit(node.nodes[i]);
            }
            this.stm.exitScope();
        }

        public void visit_Block(AST node)
        {
            this.stm.enterScope(stm.currentTable.name, stm.currentTable.scopeLevel + 1, node.nodeID);
            this.visitAll(node);
            this.stm.exitScope();
        }

        public void visit_MainBlock(AST node)
        {
            this.stm.enterScope("main", stm.currentTable.scopeLevel + 1, node.nodeID);
            this.visitAll(node);
            this.stm.exitScope();
        }

        public void visit_Procedure(AST node)
        {
            this.stm.enterScope(node.nodes[0].token.lexeme, stm.currentTable.scopeLevel, node.nodeID);
            this.visit(node.nodes[1]);
            this.visit(node.nodes[2]);
            this.stm.exitScope();
        }

        public void visit_Function(AST node)
        {
            this.stm.enterScope(node.nodes[0].token.lexeme, stm.currentTable.scopeLevel, node.nodeID);
            this.visit(node.nodes[1]);
            this.visit(node.nodes[3]);
            this.stm.exitScope();
        }

        public void visit_VarDeclaration(AST node)
        {
            this.visitAll(node);
            node.type = node.nodes.Last().type;
        }

        public void visit_Assignment(AST node)
        {
            this.visitAll(node);
            AST left = node.nodes[0];
            AST right = node.nodes[1];
            if (left.type != right.type && left.type != BuiltType.ERROR && right.type != BuiltType.ERROR)
            {
                this.ThrowErrorMessage(new VarDeclTypeMismatchError(left, right));
            }
        }

        public void visit_Call(AST node)
        {
            this.visitAll(node);
            node.type = node.nodes[0].type;
            //Check that call argument types match function parameter types
            Symbol procOrFunc = this.stm.currentTable.lookup(node.nodes[0].token.lexeme);
            List<Symbol> parameters = null;
            if (procOrFunc != null)
            {
                if(procOrFunc.category == Category.FUNCTION)
                {
                    FunctionSymbol func = (FunctionSymbol)procOrFunc;
                    parameters = func.parameters;
                }
                else
                {
                    ProcedureSymbol proc = (ProcedureSymbol)procOrFunc;
                    parameters = proc.parameters;
                }

            }
            AST arguments = null;
            if (node.nodes.Count == 2)
            {
                arguments = node.nodes[1];
            }
            if (parameters != null && arguments !=null)
            {
                if(parameters.Count != arguments.nodes.Count)
                {
                    //count mismatch
                    return;
                }
                for(int i = 0; i<parameters.Count; i++)
                {
                    if(parameters[i].type != arguments.nodes[i].type)
                    {
                        //type mismatch
                    }
                }
            }else if(parameters == null && arguments != null)
            {
                //count mismatch
            }else if(parameters !=null && arguments == null)
            {
                //count mismatch
            }

        }

        public void visit_Arguments(AST node)
        {
            this.visitAll(node);
        }

        public void visit_Return(AST node)
        {

        }

        public void visit_Assert(AST node)
        {
            this.visitAll(node);
            AST expr = node.nodes[0];
            if(expr.type != BuiltType.BOOLEAN)
            {
                this.ThrowErrorMessage(new StatementTypeError(node, expr.type, BuiltType.BOOLEAN));
            }
        }

        public void visit_Read(AST node)
        {
            this.visitAll(node);
        }

        public void visit_Write(AST node)
        {
            this.visitAll(node);
            if(node.nodes[0].type == BuiltType.ERROR)
            {
                this.ThrowErrorMessage(new TypeNotSupportedError(node, node.nodes[0]));
            }
        }

        public void visit_While(AST node)
        {
            //Check that the expression has a boolean value
            this.visitAll(node);
            AST expr = node.nodes[0];
            if(expr.type != BuiltType.BOOLEAN)
            {
                this.ThrowErrorMessage(new StatementTypeError(node, expr.type, BuiltType.BOOLEAN));
            }
        }

        public void visit_If(AST node)
        {
            //Check that the expression has a boolean value
            this.visitAll(node);
            AST expr = node.nodes[0];
            if(expr.type != BuiltType.BOOLEAN)
            {
                this.ThrowErrorMessage(new StatementTypeError(node, expr.type, BuiltType.BOOLEAN));
            }
            
        }

        public void visit_Identifier(AST node)
        {
            string id = node.token.lexeme;
            if(this.stm.currentTable.lookup(id) != null)
            {
                node.type = this.stm.currentTable.lookup(node.token.lexeme).type;
            }
            else if(this.stm.currentTable.lookupInEnclosingProcedureOrFunction(id) != null)
            {
                node.type = this.stm.currentTable.lookupInEnclosingProcedureOrFunction(id).type;
            }
            else
            {
                node.type = BuiltType.ERROR;
            }
            
        }

        public void visit_Type(AST node)
        {
            node.type = this.stm.currentTable.lookType(node.token.lexeme);
        }

        public void visit_Variable(AST node)
        {
            this.visitAll(node);
            node.type = node.nodes[0].type;
        }

        public void visit_Integer(AST node)
        {
            node.type = BuiltType.INTEGER;
        }

        public void visit_Boolean(AST node)
        {
            node.type = BuiltType.BOOLEAN;
        }

        public void visit_String(AST node)
        {
            node.type = BuiltType.STRING;
        }

        public void visit_Array(AST node) { }

        public void visit_Real(AST node)
        {
            node.type = BuiltType.REAL;
        }

        public void visit_RelationalOp(AST node)
        {
            this.visitAll(node);
            AST left = node.nodes[0];
            AST right = node.nodes[1];
            if(left.type != right.type && left.type != BuiltType.ERROR)
            {
                this.ThrowErrorMessage(new BinaryOperandTypeError(left, right, node));
                node.type = BuiltType.ERROR;
            }
            else
            {
                node.type = BuiltType.BOOLEAN;
            }

        }

        public void visit_AddingOp(AST node)
        {
            this.visitAll(node);
            TokenType type = node.token.type;
            AST left = node.nodes[0];
            AST right = node.nodes[1];
            if(type == TokenType.PLUS)
            {
                if((left.type == BuiltType.INTEGER || left.type == BuiltType.REAL || left.type == BuiltType.STRING) && left.type == right.type)
                {
                    node.type = left.type;
                    return;
                }
                else
                {
                    node.type = BuiltType.ERROR;
                    this.ThrowErrorMessage(new BinaryOperandTypeError(left, right, node));
                }

            }else if(type == TokenType.MINUS)
            {
                if ((left.type == BuiltType.INTEGER || left.type == BuiltType.REAL) && left.type == right.type)
                {
                    node.type = left.type;
                    return;
                }
                else
                {
                    node.type = BuiltType.ERROR;
                    this.ThrowErrorMessage(new BinaryOperandTypeError(left, right, node));
                }
            }
            else if(type == TokenType.OR)
            {
                if(left.type == BuiltType.BOOLEAN && right.type == BuiltType.BOOLEAN)
                {
                    node.type = left.type;
                    return;
                }
                else
                {
                    node.type = BuiltType.ERROR;
                    this.ThrowErrorMessage(new BinaryOperandTypeError(left, right, node));
                }
            }
        }

        public void visit_MultiplyingOp(AST node)
        {
            this.visitAll(node);
            TokenType type = node.token.type;
            AST left = node.nodes[0];
            AST right = node.nodes[1];

            if(type == TokenType.DIV || type == TokenType.MULT)
            {
                if((left.type == BuiltType.INTEGER || left.type == BuiltType.REAL) && left.type == right.type)
                {
                    node.type = left.type;
                }
                else
                {
                    node.type = BuiltType.ERROR;
                    this.ThrowErrorMessage(new BinaryOperandTypeError(left, right, node));
                }
            }else if(type == TokenType.MOD)
            {
                if(left.type == BuiltType.INTEGER && right.type == BuiltType.INTEGER)
                {
                    node.type = left.type;
                }
                else
                {
                    node.type = BuiltType.ERROR;
                    this.ThrowErrorMessage(new BinaryOperandTypeError(left, right, node));
                }
            }else if(type == TokenType.AND)
            {
                if(left.type == BuiltType.BOOLEAN && right.type == BuiltType.BOOLEAN)
                {
                    node.type = left.type;
                }
                else
                {
                    node.type = BuiltType.ERROR;
                    this.ThrowErrorMessage(new BinaryOperandTypeError(left, right, node));
                }
            }
        }

        public void visit_UnaryOp(AST node)
        {
            this.visitAll(node);
            TokenType type = node.token.type;
            AST expr = node.nodes[0];
            if(type == TokenType.PLUS || type == TokenType.MINUS)
            {
                if (expr.type == BuiltType.INTEGER ||expr.type == BuiltType.REAL)
                {
                    node.type = BuiltType.ERROR;
                    node.type = expr.type;
                }
                else
                {
                    this.ThrowErrorMessage(new UnaryOperandTypeError(expr,node));
                }
            }else if(type == TokenType.NOT)
            {
                if(expr.type == BuiltType.BOOLEAN)
                {
                    node.type = expr.type;
                }
                else
                {
                    node.type = BuiltType.ERROR;
                    this.ThrowErrorMessage(new UnaryOperandTypeError(expr,node));
                }
            }
        }

        public void visit_Parameters(AST node)
        {
            this.visitAll(node);
        }

        public void visit_Parameter(AST node)
        {
            this.visitAll(node);
            node.type = node.nodes[1].type;
        }

        public void visit_Reference(AST node)
        {
            this.visitAll(node);
            node.type = node.nodes[1].type;
        }

        public void visitAll(AST node)
        {
            foreach(AST n in node.nodes)
            {
                this.visit(n);
            }
        }

        public void visit_Error(AST node) { }


        public void check(AST program)
        {
            this.visit(program);
        }
    }
}
