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
    public class ClientValidateExecutor : IClientMessageExecutor
    {
        public ulong UserId { get; set; }
        public string Token { get; set; }

        Action<ImMessageResponse> successCallback;
        public ClientValidateExecutor(Action<ImMessageResponse> callback)
        {
            successCallback = callback;
        }

        public void Receive(ImMessageContext message)
        {
            if (message.Response.Status == ImStatus.Success)
            {
                successCallback?.Invoke(message.Response);
                ClientConfig.WriteLine($"ClientValidateExecutor: im client login success, session: {message.Response.Output}", ConsoleColor.Green);
            }
            else
            {
                //callReceive?.Invoke(false, message.Response);
                ClientConfig.WriteLine($"ClientValidateExecutor: im client login {message.Response.Status.ToString()}, {message.Response.Output}", ConsoleColor.Red);
            }
        }

        public ImMessageContext BuildContext()
        {
            return new ImMessageContext()
            {
                Type = ImMessageType.Validate,
                ValidateMessage = new ImValidateMessage()
                {
                    UserId = UserId,
                    Token = Token
                },
            };
        }
    }
}
