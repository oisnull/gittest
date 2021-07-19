using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Common
{
    public class IMChannelUser
    {
        public ulong UserId { get; private set; }
        public string SessionId { get; private set; }

        public IMChannelUser(ulong userId, string sessionId)
        {
            if (userId <= 0)
                throw new ArgumentNullException("UserId");

            if (string.IsNullOrEmpty(sessionId))
                throw new ArgumentNullException("SessionId");

            this.UserId = userId;
            this.SessionId = sessionId;
        }

        static readonly AttributeKey<IMChannelUser> ATTR_IM_CLIENT = AttributeKey<IMChannelUser>.ValueOf("im-client-info");

        public static void Set(IChannel channel, ulong userId, string sessionId)
        {
            IMChannelUser userAttr = new IMChannelUser(userId, sessionId);
            channel.GetAttribute(ATTR_IM_CLIENT).Set(userAttr);
        }

        public static IMChannelUser Get(IChannel channel)
        {
            IMChannelUser attr = channel.GetAttribute(ATTR_IM_CLIENT).Get();
            return attr;
        }

        public static void Remove(IChannel channel)
        {
            if (channel.HasAttribute(ATTR_IM_CLIENT))
                channel.GetAttribute(ATTR_IM_CLIENT).Remove();
        }
    }
}
