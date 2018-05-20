using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.FrontEnd.LexicalAnalysis;
using MiniPascal.FrontEnd.Parsing;
using MiniPascal.FrontEnd.SemanticAnalysis;
using MiniPascal.Utils.Source;
using MiniPascal.Utils;
using MiniPascal.ErrorHandling;
using MiniPascal.BackEnd;

namespace MiniPascal
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string path;
                if (args.Length == 0)
                {
                    Console.WriteLine("Please enter a filepath: ");
                    path = Console.ReadLine();
                }
                else
                {
                    path = args[0];
                }

                ErrorManager em = new ErrorManager();
                Lexer lexer = new Lexer(new FileSource(path));

                Parser parser = new Parser(lexer);
                parser.HookTo(em);
                AST program = parser.parse();

                SymbolTableBuildingVisitor tableBuilder = new SymbolTableBuildingVisitor();
                tableBuilder.HookTo(em);
                SymbolTableManager stm = tableBuilder.buildTables(program);

                IdentifierUsageCheckingVisitor idChecker = new IdentifierUsageCheckingVisitor(stm);
                idChecker.HookTo(em);
                idChecker.check(program);

                TypeCheckingVisitor typeChecker = new TypeCheckingVisitor(stm);
                typeChecker.HookTo(em);
                typeChecker.check(program);

                if (!em.areErrors())
                {
                    CodeGeneratingVisitor cgv = new CodeGeneratingVisitor(stm);
                    CProgram cProgram = cgv.generate(program);
                    cProgram.generateFile();
                    Console.WriteLine("Build successful.");
                }
                else
                {
                    Console.WriteLine("Build failed.");
                }

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
        }
    }
}
