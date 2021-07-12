using HWL_IM_Core.Extra;
using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Server
{
    public class DefaultOfflineMessageManager : IOfflineMessageAction
    {
        private Dictionary<ulong, List<ImMessageContext>> offlineMessages;

        public DefaultOfflineMessageManager()
        {
            offlineMessages = new Dictionary<ulong, List<ImMessageContext>>();
        }

        public void AddToFirst(ulong userid, ImMessageContext messageContext)
        {
            if (offlineMessages.ContainsKey(userid))
            {
                offlineMessages[userid].Insert(0, messageContext);
            }
            else
            {
                offlineMessages.Add(userid, new List<ImMessageContext>() { messageContext });
            }
        }

        public void AddToEnd(ulong userid, ImMessageContext messageContext)
        {
            if (offlineMessages.ContainsKey(userid))
            {
                offlineMessages[userid].Add(messageContext);
            }
            else
            {
                offlineMessages.Add(userid, new List<ImMessageContext>() { messageContext });
            }
        }

        public void AddEndMessages(ulong userid, List<ImMessageContext> messageContexts)
        {
            if (offlineMessages.ContainsKey(userid))
            {
                offlineMessages[userid].AddRange(messageContexts);
            }
            else
            {
                offlineMessages.Add(userid, messageContexts);
            }
        }

        public List<ImMessageContext> GetAll(ulong userid)
        {
            if (offlineMessages.ContainsKey(userid))
            {
                return offlineMessages[userid];
            }
            else
            {
                return null;
            }
        }

        public ImMessageContext GetFirst(ulong userid, bool remove = false)
        {
            if (offlineMessages.ContainsKey(userid))
            {
                ImMessageContext first = offlineMessages[userid].FirstOrDefault();
                if (remove)
                {
                    offlineMessages[userid].Remove(first);
                }
                return first;
            }
            else
            {
                return null;
            }
        }
    }
}
