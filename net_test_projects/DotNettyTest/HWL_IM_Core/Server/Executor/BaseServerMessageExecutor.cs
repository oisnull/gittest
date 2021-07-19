using DotNetty.Transport.Channels;
using HWL_IM_Core.Common;
using HWL_IM_Core.Extra;
using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Server.Executor
{
    public class BaseServerMessageExecutor : IServerMessageExecutor
    {
        protected OnlineChannelManager ChannelManager;
        protected IServerUserAction UserAction;
        protected IOfflineMessageAction OfflineMessageAction;

        protected IChannel CurrentChannel;
        protected ImMessageType MessageType;
        protected ImMessageHead MessageHead;

        private void CheckParameters()
        {
            if (this.CurrentChannel == null)
            {
                throw new ArgumentNullException("CurrentChannel");
            }
            if (this.MessageHead == null)
            {
                throw new ArgumentNullException("MessageHead");
            }
            if (this.IsCheckSession)
            {
                IMChannelUser user = IMChannelUser.Get(CurrentChannel);
                if (user == null || user.SessionId != this.MessageHead.Session)
                    throw new SessionInvalidException();
            }
        }

        protected virtual bool IsCheckSession
        {
            get { return true; }
        }

        protected virtual void ExecuteCore(ImMessageContext messageContext)
        {

        }

        public void SetOptions(IMServerEngineOption option)
        {
            this.ChannelManager = option.ChannelManager;
            this.UserAction = option.UserAction;
            this.OfflineMessageAction = option.OfflineMessageAction;
        }

        public void Receive(IChannel channel, ImMessageContext messageContext)
        {
            this.CurrentChannel = channel;
            this.MessageType = messageContext.Type;
            this.MessageHead = messageContext.Head;

            try
            {
                this.CheckParameters();
                this.ExecuteCore(messageContext);
            }
            catch (SessionInvalidException)
            {
                Push(channel, CreateResponseContext(ImStatus.SessionInvalid));
            }
            catch (Exception e)
            {
                Push(channel, CreateResponseContext(ImStatus.Failure, e.ToString()));
            }
        }

        public ImMessageContext CreateResponseContext(ImStatus status, string output = null)
        {
            ImMessageContext responseContext = new ImMessageContext()
            {
                Type = this.MessageType,
                Response = new ImMessageResponse()
                {
                    Status = status,
                    Source = ImMessageSource.Instant,
                }
            };
            if (output != null)
            {
                responseContext.Response.Output = output;
            }
            return responseContext;
        }

        public IMServerPushStatus Push(IChannel channel, ImMessageContext messageContext)
        {
            if (channel == null || messageContext == null)
                return IMServerPushStatus.None;

            channel.WriteAndFlushAsync(messageContext).Wait();
            return IMServerPushStatus.ToClient;
        }

        public IMServerPushStatus Push(ulong toUserId, ImMessageContext messageContext)
        {
            IChannel toChannel = this.ChannelManager.GetChannel(toUserId);
            if (toChannel == null || !toChannel.IsWritable)
            {
                this.OfflineMessageAction.AddToEnd(toUserId, messageContext);
                return IMServerPushStatus.ToOffline;
            }
            else
            {
                return Push(toChannel, messageContext);
            }
        }

        public void PushOffline(ulong toUserId, IChannel channel)
        {
            if (this.OfflineMessageAction.GetCount(toUserId) <= 0)
                return;

            while (true)
            {
                if (!channel.IsWritable) break;

                ImMessageContext messageContext = this.OfflineMessageAction.GetFirst(toUserId, true);
                if (messageContext == null) break;

                messageContext.Response.Source = ImMessageSource.Offline;
                channel.WriteAndFlushAsync(messageContext).Wait();
            }
        }
    }
}
