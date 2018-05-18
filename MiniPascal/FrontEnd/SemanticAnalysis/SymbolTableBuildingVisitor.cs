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
    using SymbolTables = System.Collections.Generic.Dictionary<Tuple<string, int, int>, SymbolTable>;
    using Key = System.Tuple<string, int, int>;

    public class SymbolTableBuildingVisitor : NodeVisitor, IHookable
    {
        public SymbolTables symbolTables;
        SymbolTable currentTable;

        public ErrorHook hook { get; set; }

        public SymbolTableBuildingVisitor()
        {
            this.symbolTables = new SymbolTables();
            this.currentTable = null;
            this.hook = new ErrorHook();
        }


        public void visit_PascalProgram(AST node)
        {
            //Console.WriteLine("Start global scope");
            this.currentTable = new SymbolTable("global", 1, null);
            this.currentTable.baseScope = this.currentTable;
            Key key = new Key("global", 1, node.nodeID);
            symbolTables[key]=this.currentTable;
            for(int i = 1; i<node.nodes.Count(); i++)
            {
                this.visit(node.nodes[i]);
            }
            this.exitCurrentScope();
            //Console.WriteLine("End global scope");
        }

        public void visit_MainBlock(AST node)
        {
            //Console.WriteLine("Enter main scope");
            string scopeName = "main";
            int scopeLevel = this.currentTable.scopeLevel + 1;
            this.enterNewScope(scopeName, scopeLevel, node.nodeID);
            this.visitAll(node);
            this.currentTable = this.currentTable.baseScope;
            this.exitCurrentScope();
        }

        public void visit_Block(AST node)
        {
            string scopeName = this.currentTable.name;
            int scopeLevel = this.currentTable.scopeLevel + 1;
            this.enterNewScope(scopeName, scopeLevel, node.nodeID);
            this.visitAll(node);
            this.currentTable = this.currentTable.baseScope;
        }

        public void visit_Procedure(AST node)
        {
            string name = node.nodes[0].token.lexeme;
            int scopeLevel = this.currentTable.scopeLevel + 1;
            if(this.currentTable.lookup(name) == null)
            {
                this.currentTable.define(new ProcedureSymbol(name, BuiltType.NONE, Category.PROCEDURE, this.parameterListGenerator(node)));
            }
            else
            {
                this.ThrowErrorMessage(new DuplicateDeclarationError(node.nodes[0].token));
            }

            //link parameters to procedure symbol here
            this.enterNewScope(name, scopeLevel, node.nodeID);
            this.visit(node.nodes[2]);
            this.exitCurrentScope();
        }

        public void visit_Function(AST node)
        {
            string name = node.nodes[0].token.lexeme;
            BuiltType type = this.currentTable.lookType(node.nodes[2].token.lexeme);
            if(this.currentTable.lookup(name) == null)
            {   
                this.currentTable.define(new FunctionSymbol(name, type, Category.FUNCTION, this.parameterListGenerator(node)));
            }
            else
            {
                this.ThrowErrorMessage(new DuplicateDeclarationError(node.nodes[0].token));
            }
            //parameters here
            this.enterNewScope(name, this.currentTable.scopeLevel + 1, node.nodeID);
            this.visit(node.nodes[3]);
        }

        private List<Symbol> parameterListGenerator(AST node)
        {
            List<Symbol> paramList = new List<Symbol>();
            AST parameters = node.nodes[1];
            if (parameters != null)
            {
                foreach (AST n in parameters.nodes)
                {
                    string parName = n.nodes[0].token.lexeme;
                    BuiltType parType = this.currentTable.lookType(n.nodes[1].token.lexeme);
                    if (n is Parameter)
                    {
                        paramList.Add(new Symbol(parName, parType, Category.PARAMETER));
                    }else  if(n is Reference)
                    {
                        paramList.Add(new Symbol(parName, parType, Category.REFERENCE));
                    }
                }
            }
            return paramList;
        }
        public void visit_While(AST node)
        {
            this.enterNewScope(this.currentTable.name, this.currentTable.scopeLevel + 1, node.nodeID);
            foreach(AST n in node.nodes)
            {
                this.visit(n);
            }
            this.exitCurrentScope();
        }

        public void visit_If(AST node)
        {
            this.enterNewScope(this.currentTable.name, this.currentTable.scopeLevel + 1, node.nodeID);
            foreach(AST n in node.nodes)
            {
                this.visit(n);
            }
            this.exitCurrentScope();
        }


        public void visit_VarDeclaration(AST node)
        {
            List<AST> nodes = node.nodes;
            BuiltType type = this.currentTable.lookType(nodes.Last().token.lexeme);
            for(int i = 0; i<nodes.Count-1; i++)
            {
                string name = nodes[i].token.lexeme;
                //check if current scope has duplicates or if enclosing function/procedure has clashing parameters
                if(this.currentTable.lookupOnlyThisScope(name) == null && this.currentTable.lookupInEnclosingProcedureOrFunction(name) == null)
                {
                    this.currentTable.define(new Symbol(name, type, Category.VARIABLE));
                }
                else
                {
                    this.ThrowErrorMessage(new DuplicateDeclarationError(nodes[i].token));
                }
            }
        }

        public void visit_Assignment(AST node)
        {
            visitAll(node);
        }

        public void visit_Identifier(AST node)
        {
            /*
            string name = node.token.lexeme;
            if(this.currentTable.lookup(name) == null)
            {
                this.ThrowErrorMessage(new UndeclaredVariableError(node.token));
            }
            */
        }

        public void visit_RelationalOp(AST node)
        {
            visitAll(node);
        }

        public void visit_AddingOp(AST node)
        {
            visitAll(node);
        }

        public void visit_MultiplyingOp(AST node)
        {
            visitAll(node);
        }

        public void visit_UnaryOp(AST node)
        {
            visitAll(node);
        }

        public void visit_Return(AST node)
        {
            visitAll(node);
        }

        public void visit_Write(AST node)
        {
            visitAll(node);
        }

        public void visit_Read(AST node)
        {
            visitAll(node);
        }

        public void visit_Assert(AST node)
        {
            visitAll(node);
        }

        public void visit_Variable(AST node)
        {}

        private void visitAll(AST node)
        {
            foreach (AST n in node.nodes)
            {
                this.visit(n);
            }
        }
        private void enterNewScope(string scopeName, int scopeLevel, int id)
        {
            Key key = new Key(scopeName, scopeLevel, id);
            SymbolTable newTable = new SymbolTable(scopeName, scopeLevel, this.currentTable);
            symbolTables[key] = newTable;
            this.currentTable = newTable;
        }

        private void exitCurrentScope()
        {
            this.currentTable = this.currentTable.baseScope;
        }
        public SymbolTableManager buildTables(AST program)
        {
            this.visit(program);
            return new SymbolTableManager(this.symbolTables);
        }
    }
}
