using HWL_IM_Core.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Server
{
    public class DefaultServerUserAction : IServerUserAction
    {
        public List<ulong> GetUserIds(string groupGuid)
        {
            return new List<ulong>() { 1, 2, 3 };
        }

        public bool ValidateUser(ulong userId, string token)
        {
            return true;
        }
    }
}
