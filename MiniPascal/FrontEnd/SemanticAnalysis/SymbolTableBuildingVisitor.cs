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
    using SymbolTables = System.Collections.Generic.Dictionary<Tuple<string, int>, SymbolTable>;
    using Key = System.Tuple<string, int>;

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
            Console.WriteLine("Start global scope");
            this.currentTable = new SymbolTable("global", 1, null);
            this.currentTable.baseScope = this.currentTable;
            Key key = new Key("global", 1);
            symbolTables[key]=this.currentTable;
            foreach(AST n in node.nodes)
            {
                this.visit(n);
            }

            Console.WriteLine("End global scope");
        }

        public void visit_MainBlock(AST node)
        {
            Console.WriteLine("Enter main scope");
            string scopeName = "main";
            int scopeLevel = this.currentTable.scopeLevel + 1;
            this.EnterNewScope(scopeName, scopeLevel);
            foreach(AST n in node.nodes)
            {
                this.visit(n);
            }
            this.currentTable = this.currentTable.baseScope;
        }

        public void visit_Block(AST node)
        {
            string scopeName = this.currentTable.name;
            int scopeLevel = this.currentTable.scopeLevel + 1;
            this.EnterNewScope(scopeName, scopeLevel);
            foreach(AST n in node.nodes)
            {
                this.visit(n);
            }
            this.currentTable = this.currentTable.baseScope;
        }

        public void visit_VarDeclaration(AST node)
        {
            List<AST> nodes = node.nodes;
            BuiltType type = this.currentTable.lookType(nodes.Last().token.lexeme);
            for(int i = 0; i<nodes.Count-1; i++)
            {
                string name = nodes[i].token.lexeme;
                if(this.currentTable.lookup(nodes[i].token.lexeme) == null)
                {
                    this.currentTable.define(new Symbol(name, type));
                }
                else
                {
                    this.ThrowErrorMessage(new DuplicateDeclarationError(nodes[i].token));
                }
            }
        }

        private void EnterNewScope(string scopeName, int scopeLevel)
        {
            Key key = new Key(scopeName, scopeLevel);
            SymbolTable newTable = new SymbolTable(scopeName, scopeLevel, this.currentTable);
            symbolTables[key] = newTable;
            this.currentTable = newTable;
        }
        public SymbolTables buildTables(AST program)
        {
            this.visit(program);
            return this.symbolTables;
        }
    }
}
