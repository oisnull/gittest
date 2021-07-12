using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Extra
{
    public interface IOfflineMessageAction
    {
        void AddToFirst(ulong userid, ImMessageContext messageContext);

        void AddToEnd(ulong userid, ImMessageContext messageContext);

        void AddEndMessages(ulong userid, List<ImMessageContext> messageContexts);

        List<ImMessageContext> GetAll(ulong userid);

        ImMessageContext GetFirst(ulong userid, bool remove = false);
    }
}
