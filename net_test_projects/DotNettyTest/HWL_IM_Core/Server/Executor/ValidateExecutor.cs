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
    public class ValidateExecutor : AbstractServerMessageExecutor<ImValidateMessage>
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

        public override void Execute(ImValidateMessage message)
        {
            if (!base.UserAction.ValidateUser(message.UserId, message.Token))
            {
                Push(base.CurrentChannel, CreateResponseContext(ImStatus.Failure, "User token is invalid."));
                base.ChannelManager.CloseChannel(base.CurrentChannel);
                return;
            }

            IMChannelUser client = IMChannelUser.Get(base.CurrentChannel);
            bool isOnline = ChannelManager.IsOnline(message.UserId);
            if (!isOnline && client == null)//case 1: never login from any device
            {
                this.GenerateNewSession();
                return;
            }

            if (!isOnline && client != null)//case 2: the same device, different account login
            {
                Push(base.CurrentChannel, CreateResponseContext(ImStatus.Failure, $"User {client.UserId} is online, Please logout first."));
                return;
            }

            if (isOnline && client == null)//case 3: different device, the same account re-login
            {
                this.ForceOffline();
                this.GenerateNewSession();
                return;
            }

            if (isOnline && client != null)
            {
                if (client.UserId == message.UserId)//case 4: the same device, the same account re-login
                {
                    Push(base.CurrentChannel, CreateResponseContext(ImStatus.Success, client.SessionId));
                    PushOffline(message.UserId, base.CurrentChannel);
                }
                else//case 5: the same device, the diff account re-login
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

                base.ChannelManager.CloseChannel(onlineChannel);
            }
        }

        public override ImValidateMessage GetMessageContent(ImMessageContext messageContext)
        {
            return messageContext.ValidateMessage;
        }
    }
}
