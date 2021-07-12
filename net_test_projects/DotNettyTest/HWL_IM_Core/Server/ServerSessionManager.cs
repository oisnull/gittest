using HWL_IM_Core.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Server
{
    public class ServerSessionManager : IServerSessionAction
    {
        protected Dictionary<ulong, string> onlineSessions;

        public ServerSessionManager()
        {
            onlineSessions = new Dictionary<ulong, string>();
        }

        public string GetSession(ulong userid)
        {
            if (userid <= 0 || !onlineSessions.ContainsKey(userid))
            {
                return null;
            }
            return onlineSessions[userid];
        }

        public ulong GetUserId(string sessionid)
        {
            if (string.IsNullOrEmpty(sessionid))
            {
                return 0;
            }
            return onlineSessions.FirstOrDefault(x => x.Value == sessionid).Key;
        }

        public void RemoveSession(string sessionid)
        {
            if (string.IsNullOrEmpty(sessionid)) return;

            ulong userid = GetUserId(sessionid);
            RemoveSession(userid);
        }

        public void RemoveSession(ulong userid)
        {
            if (userid <= 0 || !onlineSessions.ContainsKey(userid)) return;

            onlineSessions.Remove(userid);
        }

        public void SetSession(ulong userid, string sessionid)
        {
            if (userid <= 0 || string.IsNullOrEmpty(sessionid)) return;

            onlineSessions.Add(userid, sessionid);
        }
    }
}
