using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Extra
{
    public interface IServerUserAction
    {
        bool ValidateUser(ulong userId, string token);

        List<ulong> GetUserIds(string groupGuid);
    }
}
