using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Server.Executor
{
    public class GroupChatExecutor : AbstractServerMessageExecutor<ImGroupChatMessage>
    {
        protected override bool IsCheckSession => true;

        protected override void CheckParameters()
        {
            base.CheckParameters();
            if (MessageContent.FromUserId <= 0)
            {
                throw new ArgumentNullException("FromUserId");
            }
            if (string.IsNullOrEmpty(MessageContent.ToGroup))
            {
                throw new ArgumentNullException("ToGroup");
            }
            if (string.IsNullOrEmpty(MessageContent.ContentBody))
            {
                throw new ArgumentNullException("ContentBody");
            }
        }

        public override void Execute(ImGroupChatMessage message)
        {
            ImMessageContext responseContext = new ImMessageContext()
            {
                Type = this.MessageType,
                Response = new ImMessageResponse()
                {
                    Status = ImStatus.Success,
                    Source = ImMessageSource.Instant,
                },
                GroupChatMessage = message
            };

            List<ulong> userIds = UserAction.GetUserIds(message.ToGroup);
            foreach (var uid in userIds)
            {
                if (uid != message.FromUserId)
                {
                    base.Push(uid, responseContext);
                }
            }
        }

        public override ImGroupChatMessage GetMessageContent(ImMessageContext messageContext)
        {
            return messageContext.GroupChatMessage;
        }
    }
}
