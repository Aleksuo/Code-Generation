using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPascal.Utils.Source
{
    public interface ISource
    {
        char? peekNextChar();
        void advance();
        char? currentChar();      
    }
}
