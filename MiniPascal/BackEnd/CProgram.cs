using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPascal.BackEnd
{
    public class CProgram
    {
        private string name;
        private string program;

        public CProgram(string name , string program)
        {
            this.name = name;
            this.program = program;
        }

        public void generateFile()
        {
            System.IO.File.WriteAllText("Output/" + this.name + ".c", program);
        }
    }
}
