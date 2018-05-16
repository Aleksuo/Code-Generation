using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.Utils;

namespace MiniPascal.BackEnd
{
    public class CodeGeneratingVisitor : NodeVisitor
    {
        private string name;
        private StringBuilder generatedCode;
        private AST program;
        public CodeGeneratingVisitor(AST program)
        {
            this.program = program;
            this.generatedCode = new StringBuilder();
        }

        public void visit_PascalProgram(AST node)
        {
            //Identifier
            this.name = node.nodes[0].token.lexeme;
            //Last node is always mainblock
            foreach(AST n in node.nodes)
            {
                this.visit(n);
            } 
            
        }

        public void visit_MainBlock(AST node)
        {
            this.generatedCode.AppendLine("int main()");
            this.generatedCode.AppendLine("{");
            foreach(AST n in node.nodes)
            {
                this.visit(n);
            }
            this.generatedCode.AppendLine("}");
        }

        public void visit_Block(AST node)
        {
            this.generatedCode.AppendLine("{");
            foreach(AST n in node.nodes)
            {
                this.visit(n);
            }
            this.generatedCode.AppendLine("}");
        }

        public void visit_VarDeclaration(AST node)
        {
            string type = node.nodes.Last().token.lexeme;
            if (type == "Boolean")
            {
                type = "bool";
            }
            for(int i = 0; i < node.nodes.Count()-1; i++)
            {
                this.generatedCode.AppendLine(type + " " + node.nodes[i].token.lexeme + ";");
            }          
        }

        public CProgram generate()
        {
            this.visit(this.program);
            CProgram cProgram = new CProgram(this.name, generatedCode.ToString());
            return cProgram;
        }
    }
}
