using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using HWL_IM_Core.Common;
using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Server
{
    public class ServerChannelHandler : SimpleChannelInboundHandler<ImMessageContext>
    {
        private IMServerEngineOption ServerOption;
        private Dictionary<ImMessageType, IServerMessageExecutor> Receivers;
        public ServerChannelHandler(IMServerEngineOption option, Dictionary<ImMessageType, IServerMessageExecutor> receivers)
        {
            this.ServerOption = option;
            this.Receivers = receivers;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, ImMessageContext context)
        {
            if (this.Receivers.ContainsKey(context.Type))
            {
                IServerMessageExecutor executor = this.Receivers[context.Type];
                executor.SetOptions(this.ServerOption);
                executor.Receive(ctx.Channel, context);
            }
        }

        public override void UserEventTriggered(IChannelHandlerContext ctx, object evt)
        {
            if (!(evt is IdleStateEvent)) return;

            IdleStateEvent idleEvent = (IdleStateEvent)evt;
            if (idleEvent.State == IdleState.WriterIdle)
            {
                //Un auth client
                IMChannelUser user = IMChannelUser.Get(ctx.Channel);
                if (user == null)
                {
                    ctx.Channel.CloseAsync();
                    LogHelper.Info($"Remote client {ctx.Channel.RemoteAddress.ToString()} closed.");
                }
            }
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            LogHelper.Info($"Remote client {context.Channel.RemoteAddress.ToString()} connected.");
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            LogHelper.Info($"Remote client {context.Channel.RemoteAddress.ToString()} disconnected.");
            ServerOption.ChannelManager.RemoveChannel(context.Channel);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            LogHelper.Error($"Remote client {context.Channel.RemoteAddress.ToString()} execption: {exception.ToString()}");
            //ctx.close();
        }
    }
}
