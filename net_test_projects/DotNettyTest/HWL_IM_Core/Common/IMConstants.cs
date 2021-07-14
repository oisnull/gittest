using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Common
{
    public static class IMConstants
    {
        //public static readonly AttributeKey<string> ATTR_USER_ID = AttributeKey<string>.ValueOf("user-id");
        //public static readonly AttributeKey<string> ATTR_SESSION_ID = AttributeKey<string>.ValueOf("user-session-id");
        //public static readonly AttributeKey<string> DEVICE_ID = AttributeKey<string>.ValueOf("device_id");

        public const int READERIDLE_TIMEOUT_SECONDS = 2 * 60;
        public const int WRITERIDLE_TIMEOUT_SECONDS = 30;


        //public static ulong GetChannelUserId(IChannel channel)
        //{
        //    string userIdStr = channel.GetAttribute(ATTR_USER_ID).Get();
        //    ulong userId;
        //    ulong.TryParse(userIdStr, out userId);
        //    return userId;
        //}

        //public static string GetChannelSessionId(IChannel channel)
        //{
        //    return channel.GetAttribute(ATTR_SESSION_ID).Get();
        //}
    }
}
