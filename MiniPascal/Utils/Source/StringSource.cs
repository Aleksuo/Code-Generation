using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPascal.Utils.Source
{ 
    public class StringSource : ISource
    {
        private string text;
        private int position;

        public StringSource(string input)
        {
            this.text = input;
        }

        public void advance()
        {
            this.position++;
        }

        public char? currentChar()
        {
            if (this.position > this.text.Length-1)
            {
                return null;
            }
            return this.text[this.position];
        }

        public char? peekNextChar()
        {
            if ((this.position + 1) > this.text.Length-1)
            {
                return null;
            }
            return this.text[this.position + 1];
        }
    }
}
