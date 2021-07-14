using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Extra
{
    public interface IServerSessionAction
    {
        string GetSession(ulong userid);

        ulong GetUserId(string sessionid);

        void SetSession(ulong userid, string sessionid);

        void RemoveSession(string sessionid);

        void RemoveSession(ulong userid);
    }
}
