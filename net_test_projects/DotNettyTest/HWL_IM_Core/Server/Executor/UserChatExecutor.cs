using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Server.Executor
{
    public class UserChatExecutor : AbstractServerMessageExecutor<ImUserChatMessage>
    {
        protected override bool IsCheckSession => true;

        protected override void CheckParameters()
        {
            base.CheckParameters();
            if (MessageContent.FromUserId <= 0)
            {
                throw new ArgumentNullException("FromUserId");
            }
            if (MessageContent.ToUserId <= 0)
            {
                throw new ArgumentNullException("ToUserId");
            }
            if (string.IsNullOrEmpty(MessageContent.ContentBody))
            {
                throw new ArgumentNullException("ContentBody");
            }
        }

        public override void Execute(ImUserChatMessage message)
        {
            ImMessageContext responseContext = new ImMessageContext()
            {
                Type = this.MessageType,
                Response = new ImMessageResponse()
                {
                    Status = ImStatus.Success,
                    Source = ImMessageSource.Instant,
                },
                UserChatMessage = message
            };

            base.Push(message.ToUserId, responseContext);
        }

        public override ImUserChatMessage GetMessageContent(ImMessageContext messageContext)
        {
            return messageContext.UserChatMessage;
        }
    }
}
