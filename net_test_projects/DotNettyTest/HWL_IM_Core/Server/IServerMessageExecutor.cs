using DotNetty.Transport.Channels;
using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Server
{
    public interface IServerMessageExecutor
    {
        void SetOptions(IMServerEngineOption option);
        void Receive(IChannel channel, ImMessageContext messageContext);
    }
}
