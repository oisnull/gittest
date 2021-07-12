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
    public class ChatGroupExecutor : IClientMessageExecutor
    {
        public ImMessageType MessageType => ImMessageType.Group;
        public ImGroupChatMessage ChatMessage { get; set; }

        public void Receive(ImMessageContext message)
        {
            //ImUserValidateMessage validateMessage = message.ValidateMessage;
            Console.WriteLine("ChatGroupExecutor: " + JsonConvert.SerializeObject(message));
        }

        public ImMessageContext SendContent()
        {
            return new ImMessageContext()
            {
                Type = ImMessageType.Validate,
                GroupChatMessage = this.ChatMessage,
            };
        }
    }
}
