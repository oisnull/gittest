using DotNetty.Transport.Channels;
using HWL_IM_Core.Common;
using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Server.Executor
{
    public class ValidateExecutor : AbstractServerMessageExecutor<ImUserValidateMessage>
    {
        protected override bool IsCheckSession => false;

        protected override void CheckParameters()
        {
            base.CheckParameters();
            if (MessageContent.UserId <= 0)
            {
                throw new ArgumentNullException("UserId");
            }
            if (string.IsNullOrEmpty(MessageContent.Token))
            {
                throw new ArgumentNullException("Token");
            }
        }

        public override void Execute(ImUserValidateMessage message)
        {
            if (!base.UserAction.ValidateUser(message.UserId, message.Token))
            {
                Push(base.CurrentChannel, CreateResponseContext(ImStatus.Failure, "User token is invalid."));
                return;
            }

            IMChannelUser client = IMChannelUser.Get(base.CurrentChannel);
            bool isOnline = ChannelManager.IsOnline(message.UserId);
            if (isOnline)
            {
                if (client == null)
                {
                    this.ForceOffline();
                    this.GenerateNewSession();
                }
                else
                {
                    //already session online
                    Push(base.CurrentChannel, CreateResponseContext(ImStatus.Success, client.SessionId));
                    PushOffline(message.UserId, base.CurrentChannel);
                }
            }
            else
            {
                if (client == null)
                {
                    this.GenerateNewSession();
                }
                else if (client.UserId != message.UserId)
                {
                    Push(base.CurrentChannel, CreateResponseContext(ImStatus.Failure, $"User {client.UserId} is online, Please logout first."));
                }
            }
        }

        private void GenerateNewSession()
        {
            string newSessionid = Guid.NewGuid().ToString().Replace("-", "");
            base.ChannelManager.SetChannelAndSession(MessageContent.UserId, newSessionid, base.CurrentChannel);

            Push(base.CurrentChannel, CreateResponseContext(ImStatus.Success, newSessionid));
            PushOffline(MessageContent.UserId, base.CurrentChannel);
        }

        private void ForceOffline()
        {
            IChannel onlineChannel = base.ChannelManager.GetChannel(base.MessageContent.UserId);
            if (onlineChannel != null)
            {
                IMChannelUser client = IMChannelUser.Get(onlineChannel);
                Push(onlineChannel, CreateResponseContext(ImStatus.ForceOffline));

                LogHelper.Debug($"Remote client {onlineChannel.RemoteAddress.ToString()} force offline, session: {client.SessionId}");

                base.ChannelManager.RemoveChannel(onlineChannel);
                onlineChannel.CloseAsync().Wait();
            }
        }

        public override ImUserValidateMessage GetMessageContent(ImMessageContext messageContext)
        {
            return messageContext.ValidateMessage;
        }
    }
}
