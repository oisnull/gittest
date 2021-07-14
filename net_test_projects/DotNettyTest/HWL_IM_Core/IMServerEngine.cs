using DotNetty.Codecs.Protobuf;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using HWL_IM_Core.Common;
using HWL_IM_Core.Protocol;
using HWL_IM_Core.Server;
using HWL_IM_Core.Server.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core
{
    public class IMServerEngine
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        public IMServerEngineOption ServerEngineOption { get; private set; }
        public Dictionary<ImMessageType, IServerMessageExecutor> Receivers { get; private set; }

        private IEventLoopGroup receiveGroup;
        private IEventLoopGroup workGroup;
        private ServerBootstrap bootstrap;
        private IChannel currentChannel;

        public IMServerEngine(string host, int port, Action<IMServerEngineOption> optionAction = null)
        {
            this.Host = host;
            this.Port = port;
            this.ServerEngineOption = new IMServerEngineOption();
            optionAction?.Invoke(this.ServerEngineOption);


            this.Receivers = new Dictionary<ImMessageType, IServerMessageExecutor>();
            this.Receivers.Add(ImMessageType.Validate, new ValidateExecutor());
            this.Receivers.Add(ImMessageType.User, new UserExecutor());
            this.Receivers.Add(ImMessageType.Group, new GroupExecutor());
            this.Receivers.Add(ImMessageType.Ping, new BaseServerMessageExecutor());

            Init();
        }

        private void Init()
        {
            receiveGroup = new MultithreadEventLoopGroup();
            workGroup = new MultithreadEventLoopGroup();

            bootstrap = new ServerBootstrap();
            bootstrap.Group(receiveGroup, workGroup);
            bootstrap.Channel<TcpServerSocketChannel>();
            bootstrap.ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
            {
                IChannelPipeline pipeline = channel.Pipeline;
                pipeline.AddLast(new ProtobufVarint32FrameDecoder());
                pipeline.AddLast(new ProtobufDecoder(ImMessageContext.Parser));
                pipeline.AddLast(new ProtobufVarint32LengthFieldPrepender());
                pipeline.AddLast(new ProtobufEncoder());
                pipeline.AddLast(new IdleStateHandler(IMConstants.READERIDLE_TIMEOUT_SECONDS, IMConstants.WRITERIDLE_TIMEOUT_SECONDS, 0));

                pipeline.AddLast(new ServerChannelHandler(this.ServerEngineOption,this.Receivers));
            }));
        }

        public void Bind()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(this.Host), this.Port);
            currentChannel = bootstrap.BindAsync(endPoint).Result;
        }

        public async Task StopAsync()
        {
            if (currentChannel != null)
            {
                await currentChannel.CloseAsync();
            }
            if (receiveGroup != null)
            {
                await receiveGroup.ShutdownGracefullyAsync();
            }
            if (workGroup != null)
            {
                await workGroup.ShutdownGracefullyAsync();
            }
        }
    }
}
