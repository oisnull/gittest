using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Server.Executor
{
    public abstract class AbstractServerMessageExecutor<T> : BaseServerMessageExecutor
    {
        protected T MessageContent;

        protected virtual void CheckParameters()
        {
            if (MessageContent == null)
                throw new ArgumentNullException("MessageContent");
        }

        protected sealed override void ExecuteCore(ImMessageContext messageContext)
        {
            this.MessageContent = this.GetMessageContent(messageContext);
            this.CheckParameters();
            this.Execute(this.MessageContent);
        }

        public abstract T GetMessageContent(ImMessageContext messageContext);

        public abstract void Execute(T message);
    }
}
