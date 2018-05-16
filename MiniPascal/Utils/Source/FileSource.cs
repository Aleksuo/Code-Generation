using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MiniPascal.Utils.Source
{
    public class FileSource : ISource
    {
        private string fileAsString;
        private int position;

        public FileSource(string path)
        {
            this.position = 0;
            this.readFile(path);
        }

        private void readFile(string path)
        {
            try
            {
                this.fileAsString = File.ReadAllText(path);
            }catch(Exception error)
            {
                Console.WriteLine(error.ToString());
                throw error;
            }
        }

        public void advance()
        {
            this.position++;
        }

        public char? currentChar()
        {
            if(this.position > this.fileAsString.Length-1)
            {
                return null;
            }
            return this.fileAsString[this.position];
        }

        public char? peekNextChar()
        {
            if((this.position+1) > this.fileAsString.Length-1)
            {
                return null;
            }
            return this.fileAsString[this.position + 1];
        }
    }
}
