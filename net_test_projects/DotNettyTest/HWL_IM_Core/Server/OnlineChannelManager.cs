using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using HWL_IM_Core.Common;
using HWL_IM_Core.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Server
{
    public class OnlineChannelManager
    {
        /// <summary>
        /// session,channel
        /// </summary>
        private Dictionary<string, IChannel> onlineChannels;
        private IServerSessionAction sessionManager = null;

        public OnlineChannelManager()
        {
            onlineChannels = new Dictionary<string, IChannel>();
        }

        public OnlineChannelManager(IServerSessionAction sessionAction) : this()
        {
            SetSessionAction(sessionAction);
        }

        public void SetSessionAction(IServerSessionAction sessionAction)
        {
            sessionManager = sessionAction == null ? new ServerSessionManager() : sessionAction;
        }

        public void RemoveChannel(IChannel channel)
        {
            IMChannelUser user = IMChannelUser.Get(channel);
            if (user != null)
            {
                sessionManager.RemoveSession(user.UserId);
                onlineChannels.Remove(user.SessionId);
            }

            LogHelper.Info($"Online channel total: {onlineChannels.Count}");
        }

        public void CloseChannel(IChannel channel)
        {
            if (channel != null)
            {
                this.RemoveChannel(channel);

                IMChannelUser.Remove(channel);
                channel.CloseAsync().Wait();
            }
        }

        public bool IsOnline(ulong userid)
        {
            if (userid <= 0)
                return false;

            return sessionManager.GetSession(userid) != null;
        }

        public IChannel GetChannel(ulong userid)
        {
            if (userid <= 0)
                return null;

            string sessid = sessionManager.GetSession(userid);
            if (string.IsNullOrEmpty(sessid))
            {
                return null;
            }

            return onlineChannels[sessid];
        }

        public void SetChannelAndSession(ulong userid, string sessionid, IChannel channel)
        {
            if (userid <= 0 || string.IsNullOrEmpty(sessionid) || channel == null)
            {
                return;
            }

            onlineChannels.Add(sessionid, channel);
            sessionManager.SetSession(userid, sessionid);

            IMChannelUser.Set(channel, userid, sessionid);
            LogHelper.Info($"Online channel total: {onlineChannels.Count}");
        }
    }
}
