using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.Utils;
using MiniPascal.ErrorHandling;
using MiniPascal.ErrorHandling.Messages;

namespace MiniPascal.FrontEnd.SemanticAnalysis
{
    public class IdentifierUsageCheckingVisitor : NodeVisitor, IHookable{ 

        private SymbolTableManager stm;

        public ErrorHook hook { get; set; }

        public IdentifierUsageCheckingVisitor(SymbolTableManager stm)
        {
            this.stm = stm;
            this.hook = new ErrorHook();
        }

        public void visit_PascalProgram(AST node)
        {
            this.stm.enterScope("global", 1, node.nodeID);
            for(int i = 1; i< node.nodes.Count(); i++)
            {
                this.visit(node.nodes[i]);
            }
            this.stm.exitScope();
        }

        public void visit_MainBlock(AST node)
        {
            this.stm.enterScope("main", stm.currentTable.scopeLevel + 1, node.nodeID);
            this.visitAll(node);
            this.stm.exitScope();
        }

        public void visit_Block(AST node)
        {
            this.stm.enterScope(stm.currentTable.name, stm.currentTable.scopeLevel + 1, node.nodeID);
            this.visitAll(node);
            this.stm.exitScope();
        }

        public void visit_Procedure(AST node)
        {
            this.stm.enterScope(node.nodes[0].token.lexeme, stm.currentTable.scopeLevel, node.nodeID);
            this.visit(node.nodes[2]);
            this.stm.exitScope();
        }

        public void visit_Function(AST node)
        {
            this.stm.enterScope(node.nodes[0].token.lexeme, stm.currentTable.scopeLevel, node.nodeID);
            this.visit(node.nodes[3]);
            this.stm.exitScope();
        }

        public void visit_While(AST node)
        {
            this.stm.enterScope(stm.currentTable.name, stm.currentTable.scopeLevel + 1, node.nodeID);
            this.visitAll(node);
            this.stm.exitScope();
        }

        public void visit_If(AST node)
        {
            this.stm.enterScope(stm.currentTable.name, stm.currentTable.scopeLevel + 1, node.nodeID);
            this.visitAll(node);
            this.stm.exitScope();
        }

        public void visit_VarDeclaration(AST node) { }

        public void visit_Array(AST node) { }

        public void visit_Assignment(AST node)
        {
            this.visitAll(node);
        }

        public void visit_Variable(AST node)
        {
            this.visitAll(node);
        }

        public void visit_Call(AST node)
        {
            this.visitAll(node);
        }

        public void visit_Arguments(AST node)
        {
            this.visitAll(node);
        }

        public void visit_Return(AST node)
        {
            this.visitAll(node);
        }

        public void visit_Assert(AST node)
        {
            this.visitAll(node);
        }

        public void visit_Read(AST node)
        {
            this.visitAll(node);
        }

        public void visit_Write(AST node)
        {
            this.visitAll(node);
        }

        public void visit_RelationalOp(AST node)
        {
            this.visitAll(node);
        }

        public void visit_AddingOp(AST node)
        {
            this.visitAll(node);
        }

        public void visit_MultiplyingOp(AST node)
        {
            this.visitAll(node);
        }

        public void visit_UnaryOp(AST node)
        {
            this.visitAll(node);
        }

        public void visit_Type(AST node) { }
        public void visit_Integer(AST node) { }
        public void visit_Real(AST node) { }
        public void visit_Boolean(AST node) { }
        public void visit_String(AST node) { }



        public void visit_Identifier(AST node)
        {
            string name = node.token.lexeme;
            if(this.stm.currentTable.lookup(name) == null && this.stm.currentTable.lookupInEnclosingProcedureOrFunction(name) == null)
            {
                this.ThrowErrorMessage(new UndeclaredVariableError(node.token));
            }
        }

        public void visit_Error(AST node) { }

        public void visitAll(AST node)
        {
            foreach (AST n in node.nodes)
            {
                this.visit(n);
            }
        }
        
        public void check(AST program)
        {
            this.visit(program);
        }
    }

    
}
