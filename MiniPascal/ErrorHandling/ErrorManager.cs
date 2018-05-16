using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniPascal.ErrorHandling.Messages;

namespace MiniPascal.ErrorHandling
{
    public class ErrorManager
    {
        private List<IErrorMessage> messages;
        public ErrorManager()
        {
            this.messages = new List<IErrorMessage>();
        }

        public void catchErrorMessage(IErrorMessage message)
        {
            message.printMessage();
            messages.Add(message);
        }

        public bool areErrors()
        {
            return this.messages.Any();
        }

    }
}
