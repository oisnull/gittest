using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core
{
    public interface IClientListener
    {
        void OnConnected();

        void OnDisconnected();

        void OnChannelError(string message);
        void OnConnectError(string message);

        void OnClosed();
    }
}
