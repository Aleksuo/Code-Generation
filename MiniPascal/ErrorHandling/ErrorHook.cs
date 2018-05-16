using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.ErrorHandling.Messages;

namespace MiniPascal.ErrorHandling
{
    public class ErrorHook
    {
        private ErrorManager em;

        public void HookTo(ErrorManager em)
        {
            this.em = em;
        }

        public void Unhook()
        {
            this.em = null;
        }

        public void ThrowErrorMessage(IErrorMessage message)
        {
            this.em.catchErrorMessage(message);
        }
    }
}
