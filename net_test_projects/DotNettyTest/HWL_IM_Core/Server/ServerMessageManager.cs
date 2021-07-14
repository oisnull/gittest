using DotNetty.Transport.Channels;
using HWL_IM_Core.Extra;
using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Server
{
    public class ServerMessageManager
    {
        private OnlineChannelManager channelManager;
        private IOfflineMessageAction messageAction;

        public ServerMessageManager()
        {
        }

        public void Init(OnlineChannelManager channelManager, IOfflineMessageAction messageAction)
        {
            this.channelManager = channelManager;
            this.messageAction = messageAction;
        }

        public async Task PushAsync(IChannel channel, ImMessageContext messageContext)
        {
            if (channel == null || messageContext == null)
                return;

            await channel.WriteAndFlushAsync(messageContext);
        }

        //isOfflineMessage: used to batch push offline messages when user just online
        public async Task PushAsync(ulong toUserId, ImMessageContext message, bool isOfflineMessage = false)
        {
            if (toUserId <= 0 || message == null)
            {
                return;
            }

            IChannel toChannel = channelManager.GetChannel(toUserId);
            if (toChannel == null)
            {
                //If user is offline and add message to offline store
                if (isOfflineMessage)
                    messageAction.AddToFirst(toUserId, message);
                else
                    messageAction.AddToEnd(toUserId, message);
            }
            else
            {
                await PushAsync(toChannel, message);
            }
        }

        public async Task PushOfflineAsync(ulong toUserId, IChannel channel)
        {
            while (true)
            {
                if (!channel.IsWritable) break;

                ImMessageContext messageContext = messageAction.GetFirst(toUserId, true);
                if (messageContext == null) break;

                await channel.WriteAndFlushAsync(messageContext);
            }
        }

        //public async Task PushOfflineAsync(ulong toUserId)
        //{
        //    IChannel toChannel = channelManager.GetChannel(toUserId);
        //    if (toChannel != null)
        //    {
        //        while (true)
        //        {
        //            if (!toChannel.IsWritable) break;

        //            ImMessageContext messageContext = messageAction.GetFirst(toUserId, true);
        //            if (messageContext == null) break;

        //            await toChannel.WriteAndFlushAsync(messageContext);
        //        }
        //    }
        //}

        //private void loopPushOfflineMessage(ulong toUserId, IChannel channel)
        //{
        //    if (channel == null)
        //    {
        //        channel = OnlineIChannelManager.getInstance().getIChannel(toUserId);
        //    }

        //    if (channel == null || !channel.isOpen() || !channel.isActive())
        //    {
        //        return;
        //    }

        //    // check exists offline message
        //    ImMessageContext messageContext = offlineMessageManager.pollMessage(toUserId);
        //    if (messageContext == null)
        //    {
        //        return;
        //    }

        //    push(toUserId, channel, messageContext, true);
        //    pushMonitor.addCount(toUserId);

        //    loopPushOfflineMessage(toUserId, channel);
        //}

        //public void startPush(ulong userId)
        //{
        //    if (pushMonitor.isRunning(userId))
        //        return;

        //    log.info("Server push offline message to userid({}) start.", userId);
        //    pushMonitor.start(userId);
        //    executorService.execute(()-> {
        //        loopPushOfflineMessage(userId, null);
        //        pushMonitor.removeStatus(userId);
        //        log.info("Server push offline message to userid({}) end.", userId);
        //    });
        //}

        //public void deleteSentMessage(ulong userId, String messageGuid)
        //{
        //    sentMessageManager.removeMessage(userId, messageGuid);
        //}

        //public void moveSentMessageIntoOffline(IChannel channel)
        //{
        //    ulong userId = OnlineIChannelManager.getInstance().getUserId(channel);
        //    if (userId <= 0)
        //        return;

        //    List<ImMessageContext> messages = sentMessageManager.getMessages(userId);
        //    if (messages == null || messages.size() <= 0)
        //        return;

        //    log.info("Server move sent message of userid({}) into offline store.", userId);
        //    synchronized(messages) {
        //        offlineMessageManager.addMessages(userId, messages);
        //        messages.clear();
        //        sentMessageManager.removeUser(userId);
        //    }
        //}
    }
}
