using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.FrontEnd.LexicalAnalysis;
using MiniPascal.Utils;

namespace MiniPascal.BackEnd
{
    public class CodeGeneratingVisitor : NodeVisitor
    {
        private string name;
        private StringBuilder generatedCode;
        private AST program;

        private SymbolTableManager stm;
        public CodeGeneratingVisitor(SymbolTableManager stm)
        {
            this.stm = stm;
            this.generatedCode = new StringBuilder();
        }

        public void visit_PascalProgram(AST node)
        {
            this.stm.enterScope("global", 1, node.nodeID);
            //Identifier
            this.name = node.nodes[0].token.lexeme;
            //Last node is always mainblock
            foreach(AST n in node.nodes)
            {
                this.visit(n);
            }
            this.stm.exitScope();            
        }

        public void visit_MainBlock(AST node)
        {
            this.stm.enterScope("main", this.stm.currentTable.scopeLevel + 1, node.nodeID);
            this.generatedCode.AppendLine("int main()");
            this.generatedCode.AppendLine("{");
            foreach(AST n in node.nodes)
            {
                if (n is Call)
                {
                    this.generatedCode.AppendLine((string)this.visit(n) + ";");
                }
                else
                {
                    this.visit(n);
                }
            }
            this.generatedCode.AppendLine("}");
            this.stm.exitScope();
        }

        public void visit_Block(AST node)
        {
            string indent = this.indent(this.stm.currentTable.scopeLevel);
            this.stm.enterScope(this.stm.currentTable.name, this.stm.currentTable.scopeLevel + 1, node.nodeID);
            this.generatedCode.AppendLine(indent+"{");
            foreach(AST n in node.nodes)
            {
                if(n is Call)
                {
                    this.generatedCode.AppendLine((string)this.visit(n) + ";");
                }
                else
                {
                    this.visit(n);
                }
            }
            this.generatedCode.AppendLine(indent+"}");
            this.stm.exitScope();
        }

        public void visit_VarDeclaration(AST node)
        {
            BuiltType bType = node.type;
            string type = "";
            if (bType == BuiltType.BOOLEAN)
            {
                type = "bool";
            }else if(bType == BuiltType.INTEGER)
            {
                type = "int";
            }else if(bType == BuiltType.REAL)
            {
                type = "double";
            }else if(bType == BuiltType.STRING)
            {
                type = "char";
            }
            for(int i = 0; i < node.nodes.Count()-1; i++)
            {
                if(bType == BuiltType.STRING)
                {
                    this.generatedCode.AppendLine(this.indent(stm.currentTable.scopeLevel) + type + " " + node.nodes[i].token.lexeme +"[255]"+";");
                }
                else
                {
                    this.generatedCode.AppendLine(this.indent(stm.currentTable.scopeLevel) + type + " " + node.nodes[i].token.lexeme + ";");
                }          
            }          
        }

        public void visit_Procedure(AST node)
        {
            AST id = node.nodes[0];
            AST parameters = node.nodes[1];
            string procedure = "void " + id.token.lexeme + "(";
            for(int i = 0; i<parameters.nodes.Count; i++)
            {
                procedure += this.visit(parameters.nodes[i]);
                if(i != parameters.nodes.Count - 1)
                {
                    procedure += ", ";
                }
            }
            procedure += ")";
            this.generatedCode.AppendLine(procedure);
            this.stm.enterScope(id.token.lexeme, stm.currentTable.scopeLevel, node.nodeID);
            this.visit(node.nodes[2]);
            this.stm.exitScope();
        }

        public void visit_Function(AST node)
        {
            AST id = node.nodes[0];
            AST parameters = node.nodes[1];
            BuiltType bType = this.stm.currentTable.lookup(id.token.lexeme).type;
            AST block = node.nodes[3];
            string type = "";
            if (bType == BuiltType.INTEGER)
            {
                type = "int";
            }
            else if (bType == BuiltType.BOOLEAN)
            {
                type = "bool";
            }
            else if (bType == BuiltType.REAL)
            {
                type = "double";
            }
            else if (bType == BuiltType.STRING)
            {
                type = "char";
            }
            string function =  type+" " + id.token.lexeme + "(";
            for (int i = 0; i < parameters.nodes.Count; i++)
            {
                function += this.visit(parameters.nodes[i]);
                if (i != parameters.nodes.Count - 1)
                {
                    function += ", ";
                }
            }
            function += ")";
            this.generatedCode.AppendLine(function);
            this.stm.enterScope(id.token.lexeme, stm.currentTable.scopeLevel, node.nodeID);
            this.visit(node.nodes[3]);
            this.stm.exitScope();
        }

        public void visit_Assignment(AST node)
        {
            AST var = node.nodes[0];
            AST expr = node.nodes[1];
            if(var.type == BuiltType.STRING)
            {
                this.generatedCode.AppendLine(this.indent(stm.currentTable.scopeLevel)+"strcpy("+var.token.lexeme+","+ (string)this.visit(expr) + ");");

            }
            else
            {
                this.generatedCode.AppendLine(this.indent(stm.currentTable.scopeLevel) + var.token.lexeme + "=" + (string)this.visit(expr) + ";");
            }        
        }

        public string visit_Call(AST node)
        {
            string statement = "";
            AST id = node.nodes[0];
            AST args = node.nodes[1];
            statement += id.token.lexeme + "(";
            for(int i = 0; i<args.nodes.Count; i++)
            {
                statement += (string)this.visit(args.nodes[i]);
                if(i < args.nodes.Count - 1)
                {
                    statement += ", ";
                }
            }
            statement += ")";
            return statement;

        }

        public void visit_Arguments(AST node)
        {

        }

        public void visit_Return(AST node)
        {
            string indent = this.indent(this.stm.currentTable.scopeLevel);
            this.generatedCode.AppendLine(indent+"return " + (string)this.visit(node.nodes[0]) +";");
        }

        public void visit_Assert(AST node)
        {
            this.generatedCode.AppendLine(this.indent(stm.currentTable.scopeLevel)+"assert(" + (string)this.visit(node.nodes[0]) + ");");
        }

        public void visit_Read(AST node)
        {
            string statement = "";
            string indent = this.indent(stm.currentTable.scopeLevel);
            statement += indent;
            statement += "scanf(\"";
            foreach(AST n in node.nodes){
                BuiltType type = n.type;
                if(type == BuiltType.INTEGER)
                {
                    statement += "%d";
                }else if(type == BuiltType.REAL)
                {
                    statement += "%e";
                }else if(type == BuiltType.STRING)
                {
                    statement += "%s";
                }
            }
            statement += "\"";
            foreach(AST n in node.nodes)
            {
                string id = n.nodes[0].token.lexeme;
                BuiltType type = n.type;
                statement += ",";
                if (n.type == BuiltType.STRING)
                {
                    statement += id;
                }
                else
                {
                    statement += "&" + id;
                }

            }
            statement += ");";
            this.generatedCode.AppendLine(statement);
        }

        public void visit_Write(AST node)
        {
            string indent = this.indent(this.stm.currentTable.scopeLevel);
            string statement = indent+"printf(\"";
            AST args = node.nodes[0];
            foreach(AST arg in args.nodes)
            {
                BuiltType type = arg.type;
                if (type == BuiltType.INTEGER)
                {
                    statement += "%d";
                }
                else if (type == BuiltType.REAL)
                {
                    statement += "%e";
                }
                else if (type == BuiltType.STRING)
                {
                    statement += "%s";
                }
            }
            statement += "\"";
            foreach(AST arg in args.nodes)
            {
                statement += ",";
                statement += (string)this.visit(arg);
            }
            statement += ");";
            this.generatedCode.AppendLine(statement);
        }

        public void visit_While(AST node) { }

        public void visit_If(AST node)
        {         
        }

        public string visit_Identifier(AST node)
        {
            string name = node.token.lexeme;
            if (this.stm.currentTable.lookupInEnclosingProcedureOrFunction(name) !=null)
            {
                if(this.stm.currentTable.lookupInEnclosingProcedureOrFunction(name).category == Category.REFERENCE)
                {
                    name = "*" + name;
                }
            }
            return name;
        }

        public void visit_Type(AST node) { }

        public string visit_Variable(AST node)
        {
            return (string)this.visit(node.nodes[0]);
        }

        public string visit_Integer(AST node)
        {
            return node.token.lexeme;
        }

        public string visit_Boolean(AST node)
        {
            return node.token.lexeme;
        }

        public string visit_String(AST node)
        {
            return "\""+node.token.lexeme+"\"";
        }

        public void visit_Array(AST node) { }

        public string visit_Real(AST node)
        {
            return node.token.lexeme;
        }

        public string visit_RelationalOp(AST node)
        {
            TokenType type = node.token.type;
            string typeString = "";
            if(type == TokenType.EQUALS)
            {
                typeString = "==";
            }else if(type == TokenType.NOTEQUAL)
            {
                typeString = "!=";
            }
            else
            {
                typeString = node.token.lexeme;
            }
            return "(" + (string)this.visit(node.nodes[0]) + " " + typeString + " " + (string)this.visit(node.nodes[1])+")";
        }

        public string visit_AddingOp(AST node)
        {
            TokenType type = node.token.type;
            string typeString = "";
            if (type == TokenType.OR)
            {
                typeString = "||";
            }
            else
            {
                typeString = node.token.lexeme;
            }

            return "(" + (string)this.visit(node.nodes[0]) + " " + typeString + " " + (string)this.visit(node.nodes[1]) + ")";
        }

        public string visit_MultiplyingOp(AST node)
        {
            TokenType type = node.token.type;
            string typeString = "";
            if (type == TokenType.AND)
            {
                typeString = "&&";
            }
            else
            {
                typeString = node.token.lexeme;
            }

            return "(" + (string)this.visit(node.nodes[0]) + " " + typeString + " " + (string)this.visit(node.nodes[1]) + ")";
        }

        public string visit_UnaryOp(AST node)
        {
            TokenType type = node.token.type;
            string typeString = "";
            if (type == TokenType.NOT)
            {
                typeString = "!";
            }
            else
            {
                typeString = node.token.lexeme;
            }
            return typeString + (string)this.visit(node.nodes[0]);
        }

        public void visit_Parameters(AST node) { }

        public string visit_Parameter(AST node)
        {
            BuiltType bType = node.type;
            string type = "";
            if(bType == BuiltType.INTEGER)
            {
                type = "int";
            }else if(bType == BuiltType.BOOLEAN)
            {
                type = "bool";
            }else if(bType == BuiltType.REAL)
            {
                type = "double";
            }else if(bType == BuiltType.STRING)
            {
                type = "char*";
            }
            return type + " " + node.nodes[0].token.lexeme;
        }

        public string visit_Reference(AST node)
        {
            BuiltType bType = node.type;
            string type = "";
            if (bType == BuiltType.INTEGER)
            {
                type = "int*";
            }
            else if (bType == BuiltType.BOOLEAN)
            {
                type = "bool*";
            }
            else if (bType == BuiltType.REAL)
            {
                type = "double*";
            }
            else if (bType == BuiltType.STRING)
            {
                type = "char*";
            }
            return type + " " + node.nodes[0].token.lexeme;
        }

        private string indent(int level)
        {
            string indentation = "";
            for(int i = 1; i < level; i++)
            {
                indentation += '\t';
            }
            return indentation;
        }

        public CProgram generate(AST program)
        {
            generatedCode.AppendLine("#include <stdio.h>");
            generatedCode.AppendLine("#include <stdbool.h>");
            generatedCode.AppendLine("#include <assert.h>");
            generatedCode.AppendLine("#include <string.h>");
            generatedCode.AppendLine();
            this.visit(program);
            CProgram cProgram = new CProgram(this.name, generatedCode.ToString());
            return cProgram;
        }
    }
}
