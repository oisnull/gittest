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
    public class GroupExecutor : IClientMessageExecutor
    {
        public ImGroupMessage GroupMessage { get; set; }

        public void Receive(ImMessageContext message)
        {
            Console.WriteLine("GroupExecutor: " + JsonConvert.SerializeObject(message));
        }

        public ImMessageContext BuildContext()
        {
            return new ImMessageContext()
            {
                Type = ImMessageType.Group,
                GroupMessage = this.GroupMessage,
            };
        }
    }
}
