using HWL_IM_Core.Client;
using HWL_IM_Core.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Client.Executor
{
    public class ClientPingExecutor : IClientMessageExecutor
    {
        public ImMessageType MessageType => ImMessageType.Ping;

        public void Receive(ImMessageContext message)
        {
            ClientConfig.WriteLine("ClientPingExecutor: " + JsonConvert.SerializeObject(message));
        }

        public ImMessageContext SendContent()
        {
            return new ImMessageContext()
            {
                Type = ImMessageType.Ping,
            };
        }
    }
}
