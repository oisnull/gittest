using DotNetty.Common.Utilities;
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
        private IMServerEngineOption serverOption;
        public ServerChannelHandler(IMServerEngineOption option)
        {
            this.serverOption = option;
        }

        public ImMessageContext CreateResponseContext(ImMessageType type, ImStatus status, string response = null)
        {
            ImMessageContext responseContext = new ImMessageContext()
            {
                Type = type,
                Response = new ImMessageResponse()
                {
                    Status = status,
                    Output = response,
                    Source = ImMessageSource.Instant,
                }
            };
            return responseContext;
        }

        public void Push(IChannel channel, ImMessageContext messageContext)
        {
            if (channel == null || messageContext == null)
                return;

            channel.WriteAndFlushAsync(messageContext).Wait();
        }

        public void PushOffline(ulong toUserId, IChannel channel)
        {
            while (true)
            {
                if (!channel.IsWritable) break;

                ImMessageContext messageContext = serverOption.OfflineMessageAction.GetFirst(toUserId, true);
                if (messageContext == null) break;

                channel.WriteAndFlushAsync(messageContext).Wait();
            }
        }

        public void ValidateExecute(IChannel channel, ImMessageContext messageContext)
        {
            ulong userId = messageContext.ValidateMessage.UserId;
            string token = messageContext.ValidateMessage.Token;
            if (userId <= 0)
            {
                //serverOption.MessageOperator.PushAsync(channel, CreateResponseContext(messageContext.Type, ImStatus.Failure, "The param of UserId can't be empty")).GetAwaiter();
                Push(channel, CreateResponseContext(messageContext.Type, ImStatus.Failure, "The param of UserId can't be empty"));
                return;
            }
            if (string.IsNullOrEmpty(token))
            {
                //serverOption.MessageOperator.PushAsync(channel, CreateResponseContext(messageContext.Type, ImStatus.Failure, "The param of Token can't be empty")).GetAwaiter();
                Push(channel, CreateResponseContext(messageContext.Type, ImStatus.Failure, "The param of Token can't be empty"));
                return;
            }

            if (token == "123456")
            {
                string newSessionid = Guid.NewGuid().ToString().Replace("-", "");
                serverOption.ChannelManager.SetChannelAndSession(userId, newSessionid, channel);

                Push(channel, CreateResponseContext(messageContext.Type, ImStatus.Success, newSessionid));
                PushOffline(userId, channel);
            }
            else
            {
                Push(channel, CreateResponseContext(messageContext.Type, ImStatus.SessionInvalid, "User session is invalid"));
            }
        }

        public void UserChatExecute(IChannel channel, ImMessageContext messageContext)
        {
            //session validate
            string session = channel.GetAttribute(IMConstants.ATTR_SESSION_ID).Get();
            if (string.IsNullOrEmpty(session) || session != messageContext.Head.Session)
            {
                Push(channel, new ImMessageContext()
                {
                    Type = messageContext.Type,
                    Response = new ImMessageResponse()
                    {
                        Status = ImStatus.SessionInvalid,
                        Source = ImMessageSource.Instant,
                    }
                });
                return;
            }

            ulong from = messageContext.UserChatMessage.FromUserId;
            ulong to = messageContext.UserChatMessage.ToUserId;

            ImMessageContext responseContext = new ImMessageContext()
            {
                Type = messageContext.Type,
                Response = new ImMessageResponse()
                {
                    Status = ImStatus.Success,
                    Source = ImMessageSource.Instant,
                },
                UserChatMessage = messageContext.UserChatMessage
            };

            IChannel toChannel = serverOption.ChannelManager.GetChannel(to);
            if (toChannel == null)
            {
                serverOption.OfflineMessageAction.AddToEnd(to, responseContext);
            }
            else
            {
                Push(toChannel, responseContext);
            }
        }

        public void GroupChatExecute(IChannel channel, ImMessageContext messageContext)
        {
            //session validate
            string session = channel.GetAttribute(IMConstants.ATTR_SESSION_ID).Get();
            if (string.IsNullOrEmpty(session) || session != messageContext.Head.Session)
            {
                Push(channel, new ImMessageContext()
                {
                    Type = messageContext.Type,
                    Response = new ImMessageResponse()
                    {
                        Status = ImStatus.SessionInvalid,
                        Source = ImMessageSource.Instant,
                    }
                });
                return;
            }

            ulong from = messageContext.GroupChatMessage.FromUserId;
            string to = messageContext.GroupChatMessage.ToGroup;

            ImMessageContext responseContext = new ImMessageContext()
            {
                Type = messageContext.Type,
                Response = new ImMessageResponse()
                {
                    Status = ImStatus.Success,
                    Source = ImMessageSource.Instant,
                },
                UserChatMessage = messageContext.UserChatMessage
            };

            List<ulong> userIds = serverOption.UserAction.GetUserIds(to);
            foreach (var item in userIds)
            {
                if (item != from)
                {
                    IChannel toChannel = serverOption.ChannelManager.GetChannel(to);
                    if (toChannel == null)
                    {
                        serverOption.OfflineMessageAction.AddToEnd(item, responseContext);
                    }
                    else
                    {
                        Push(toChannel, responseContext);
                    }
                }
            }
        }

        public void PingExecute(IChannel channel, ImMessageContext messageContext)
        {
            //session validate
            string session = channel.GetAttribute(IMConstants.ATTR_SESSION_ID).Get();
            if (string.IsNullOrEmpty(session) || session != messageContext.Head?.Session)
            {
                Push(channel, new ImMessageContext()
                {
                    Type = messageContext.Type,
                    Response = new ImMessageResponse()
                    {
                        Status = ImStatus.SessionInvalid,
                        Source = ImMessageSource.Instant,
                    }
                });
                return;
            }
            //LogHelper.Debug($"Remote client {channel.RemoteAddress.ToString()} ping: {session}");
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, ImMessageContext context)
        {
            switch (context.Type)
            {
                case ImMessageType.Basic:
                    break;
                case ImMessageType.Validate:
                    ValidateExecute(ctx.Channel, context);
                    break;
                case ImMessageType.User:
                    UserChatExecute(ctx.Channel, context);
                    break;
                case ImMessageType.Group:
                    GroupChatExecute(ctx.Channel, context);
                    break;
                case ImMessageType.Ping:
                    PingExecute(ctx.Channel, context);
                    break;
                case ImMessageType.Pong:
                    break;
                case ImMessageType.ClientAck:
                    break;
                default:
                    break;
            }
        }

        public override void UserEventTriggered(IChannelHandlerContext ctx, object evt)
        {
            if (!(evt is IdleStateEvent)) return;

            IdleStateEvent idleEvent = (IdleStateEvent)evt;
            if (idleEvent.State == IdleState.WriterIdle)
            {
                //Un auth client
                string session = ctx.Channel.GetAttribute(IMConstants.ATTR_SESSION_ID).Get();
                if (string.IsNullOrEmpty(session))
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
            serverOption.ChannelManager.RemoveChannel(context.Channel);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            LogHelper.Error($"Remote client {context.Channel.RemoteAddress.ToString()} execption: {exception.ToString()}");
            //cause.printStackTrace();
            //ctx.close();
        }
    }
}
