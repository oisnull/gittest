using DotNetty.Codecs.Protobuf;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using HWL_IM_Core.Client;
using HWL_IM_Core.Common;
using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core
{
    public class IMClientEngine
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        public int TimeoutSeconds { get; set; }
        public IMClientStatus Status { get; private set; }
        public IChannel CurrentChannel { get; private set; }
        public ImMessageHead GlobalMessageHead { get; set; }

        private IEventLoopGroup workGroup;
        private Bootstrap bootstrap = null;
        private Dictionary<ImMessageType, IClientMessageExecutor> messageExecutors;
        private IClientListener clientListener;

        public IMClientEngine(string host, int port, int timeoutSeconds = 60, IClientListener clientListener = null)
        {
            this.Host = host;
            this.Port = port;
            this.TimeoutSeconds = timeoutSeconds;
            this.messageExecutors = new Dictionary<ImMessageType, IClientMessageExecutor>();
            this.clientListener = clientListener;

            Init();
        }

        public bool IsConnected
        {
            get
            {
                return this.Status == IMClientStatus.Connect && this.CurrentChannel.Open;
            }
        }

        public string LocalAddress
        {
            get
            {
                return this.CurrentChannel?.LocalAddress.ToString();
            }
        }

        public void Register(IClientMessageExecutor executor)
        {
            if (executor == null) return;

            ImMessageType type = executor.BuildContext().Type;
            if (this.messageExecutors.ContainsKey(type))
            {
                this.messageExecutors.Remove(type);
            }
            this.messageExecutors.Add(type, executor);
        }

        public IClientMessageExecutor GetExecutor(ImMessageType type)
        {
            if (this.messageExecutors.ContainsKey(type))
            {
                return this.messageExecutors[type];
            }
            else
            {
                return null;
            }
        }

        public bool Send(IClientMessageExecutor executor, bool register = true)
        {
            if (executor == null) return false;

            if (!this.IsConnected)
            {
                throw new Exception("The status of im client is disconnect.");
            }

            if (register)
            {
                this.Register(executor);
            }

            ImMessageContext context = executor.BuildContext();
            if (context == null)
            {
                throw new Exception("The content sent through the im client cannot be empty.");
            }

            if (context.Head == null)
            {
                context.Head = this.GlobalMessageHead;
            }

            this.CurrentChannel.WriteAndFlushAsync(context).Wait();
            return true;
        }

        public bool Send(ImMessageType type)
        {
            if (this.messageExecutors.ContainsKey(type))
            {
                return this.Send(this.messageExecutors[type], false);
            }
            else
            {
                throw new Exception($"Not found message type of {type.ToString()}, Please register it first.");
            }
        }

        private void Init()
        {
            workGroup = new MultithreadEventLoopGroup();
            bootstrap = new Bootstrap();
            bootstrap
                .Group(workGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    channel.Pipeline.AddLast(new ProtobufVarint32FrameDecoder());
                    channel.Pipeline.AddLast(new ProtobufDecoder(ImMessageContext.Parser));
                    channel.Pipeline.AddLast(new ProtobufVarint32LengthFieldPrepender());
                    channel.Pipeline.AddLast(new ProtobufEncoder());
                    channel.Pipeline.AddLast(new IdleStateHandler(0, 0, this.TimeoutSeconds));

                    channel.Pipeline.AddLast(new ClientChannelHandler(this.messageExecutors, this.clientListener, () => this.Send(ImMessageType.Ping)));
                }));
        }

        public void Connect()
        {
            if (this.IsConnected)
                return;

            try
            {
                this.Status = IMClientStatus.Connecting;
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(this.Host), this.Port);
                this.CurrentChannel = bootstrap.ConnectAsync(endPoint).Result;
                this.Status = IMClientStatus.Connect;
                this.clientListener?.OnConnected();
            }
            catch (Exception e)
            {
                this.clientListener?.OnConnectError(e.ToString());
                this.Status = IMClientStatus.DisConnect;
            }
        }

        public void Stop()
        {
            if (this.CurrentChannel != null)
            {
                this.CurrentChannel.CloseAsync();
            }
            if (workGroup != null)
            {
                workGroup.ShutdownGracefullyAsync();
            }
            this.clientListener?.OnClosed();
        }
    }
}
