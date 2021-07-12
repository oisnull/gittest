using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using HWL_IM_Core.Common;
using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Client
{

    public class ClientChannelHandler : SimpleChannelInboundHandler<ImMessageContext>
    {
        private Dictionary<ImMessageType, IClientMessageExecutor> messageExecutors;
        private IClientListener channelListener;
        private Action pingCall;

        public ClientChannelHandler(Dictionary<ImMessageType, IClientMessageExecutor> executors, IClientListener clientListener, Action pingCall)
        {
            this.messageExecutors = executors;
            this.channelListener = clientListener;
            this.pingCall = pingCall;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, ImMessageContext context)
        {
            if (messageExecutors.ContainsKey(context.Type))
            {
                messageExecutors[context.Type].Receive(context);
            }
        }

        public override void UserEventTriggered(IChannelHandlerContext ctx, object evt)
        {
            if (evt is IdleStateEvent)
            {
                IdleStateEvent idleEvent = (IdleStateEvent)evt;
                if (idleEvent.State == IdleState.AllIdle)
                {
                    pingCall?.Invoke();
                    LogHelper.Info("Client send ping info to server.");
                }
            }
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            this.channelListener?.OnDisconnected();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            this.channelListener?.OnChannelError(exception.ToString());
        }
    }
}
