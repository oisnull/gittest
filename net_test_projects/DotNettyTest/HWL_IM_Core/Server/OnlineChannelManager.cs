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

        public IChannel GetChannel(string sessionid)
        {
            if (string.IsNullOrEmpty(sessionid) || !onlineChannels.ContainsKey(sessionid)) return null;
            return onlineChannels[sessionid];
        }

        public void AddChannel(IChannel channel)
        {
            string key = channel.GetAttribute(IMConstants.ATTR_SESSION_ID).Get();
            if (!string.IsNullOrEmpty(key))
                onlineChannels.Add(key, channel);
        }

        public void RemoveChannel(IChannel channel)
        {
            string key = channel.GetAttribute(IMConstants.ATTR_SESSION_ID).Get();
            if (!string.IsNullOrEmpty(key))
            {
                sessionManager.RemoveSession(key);
                onlineChannels.Remove(key);
            }
        }

        public string GetSession(ulong userid)
        {
            if (userid <= 0) return null;
            return sessionManager.GetSession(userid);
        }

        public ulong GetUserId(IChannel channel)
        {
            string key = channel.GetAttribute(IMConstants.ATTR_SESSION_ID).Get();
            return sessionManager.GetUserId(key);
        }

        public bool IsOnline(ulong userid)
        {
            if (userid <= 0)
                return false;

            return sessionManager.GetSession(userid) != null;
        }

        public bool IsOnline(string sessionid)
        {
            if (string.IsNullOrEmpty(sessionid)) return false;
            return onlineChannels.ContainsKey(sessionid);
        }

        public bool isOnline(IChannel channel)
        {
            ulong userId = GetUserId(channel);
            if (userId <= 0) return false;

            IChannel c = GetChannel(userId);
            return c != null && c.Active;
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

            channel.GetAttribute(IMConstants.ATTR_USER_ID).Set(userid.ToString());
            channel.GetAttribute(IMConstants.ATTR_SESSION_ID).Set(sessionid);
            onlineChannels.Add(sessionid, channel);
            sessionManager.SetSession(userid, sessionid);

            LogHelper.Info($"Online channel total: {onlineChannels.Count}");
        }
    }
}
