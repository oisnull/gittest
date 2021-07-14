using HWL_IM_Client.Executor;
using HWL_IM_Core;
using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HWL_IM_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            LogHelper.InitConfigure();

            IMClientEngine client = new IMClientEngine("127.0.0.1", 8050, clientListener: new IMClientLisenter());
            client.Register(new ClientPingExecutor());
            client.Register(new UserExecutor());
            client.Register(new GroupExecutor());
            client.GlobalMessageHead = new ImMessageHead()
            {
                Client = "windows",
                Language = "c#",
                //Session = null,
                Version = "1.0.0",
                Timestamp = 0,
            };
            Console.WriteLine($"Connecting to server({client.Host}:{client.Port}) ...");
            client.Connect();
            ClientConfig.WriteLine($"Current client endpoint: {client.LocalAddress}", ConsoleColor.Green);

            Exec(client);
        }

        static void Exec(IMClientEngine client)
        {
            Print();

            ulong currentUserId = 0;
            while (true)
            {
                try
                {
                    string key = Console.ReadLine();
                    if (key?.ToLower() == "q")
                    {
                        break;
                    }
                    else if (key.StartsWith("login"))
                    {
                        string[] parameters = key.Split(' ');
                        currentUserId = ulong.Parse(parameters[1]);
                        UserLogin(client, currentUserId, parameters[2]);
                    }
                    else if (key.StartsWith("connect"))
                    {
                        client.Connect();
                        ClientConfig.WriteLine($"Current client endpoint: {client.LocalAddress}", ConsoleColor.Green);
                    }
                    else if (key.StartsWith("logout"))
                    {
                        client.Stop();
                        break;
                    }
                    else if (key.StartsWith("user"))
                    {
                        string[] parameters = key.Split(' ');
                        User(client, currentUserId, ulong.Parse(parameters[1]), parameters[2]);
                    }
                    else if (key.StartsWith("group"))
                    {
                        string[] parameters = key.Split(' ');
                        Group(client, currentUserId, parameters[1], parameters[2]);
                    }
                    else
                    {
                        Print();
                    }
                }
                catch (Exception ex)
                {
                    Print();
                    ClientConfig.WriteLine(ex.ToString(), ConsoleColor.Red);
                }
            }
        }

        static void UserLogin(IMClientEngine client, ulong userId, string token)
        {
            ClientValidateExecutor clientValidate = new ClientValidateExecutor(response =>
            {
                client.GlobalMessageHead.Session = response.Output;
            });
            clientValidate.UserId = userId;
            clientValidate.Token = token;
            client.Send(clientValidate);
        }

        static void User(IMClientEngine client, ulong fromUser, ulong toUser, string textMessage)
        {
            UserExecutor userExecutor = new UserExecutor();
            userExecutor.UserMessage = new ImUserMessage()
            {
                FromUserId = fromUser,
                ToUserId = toUser,
                ContentType = "text",
                ContentBody = textMessage,
            };
            client.Send(userExecutor);
        }

        static void Group(IMClientEngine client, ulong fromUser, string toGroup, string textMessage)
        {
            GroupExecutor groupExecutor = new GroupExecutor();
            groupExecutor.GroupMessage = new ImGroupMessage()
            {
                FromUserId = fromUser,
                ToGroup = toGroup,
                ContentType = "text",
                ContentBody = textMessage,
            };
            client.Send(groupExecutor);
        }

        static void Print()
        {
            Console.WriteLine(@"Execute command:
connect
logout
login 1 token
user 1 hellow-1
group guid hellow-group-1");
            Console.WriteLine("Press key 'q' to end.");
        }
    }

    class IMClientLisenter : IClientListener
    {
        public void OnChannelError(string message)
        {
            ClientConfig.WriteLine("Channel error: " + message, ConsoleColor.Red);
        }

        public void OnClosed()
        {
            ClientConfig.WriteLine("Client closed.", ConsoleColor.Red);
        }

        public void OnConnected()
        {
            ClientConfig.WriteLine("Client connect to im server.", ConsoleColor.Green);
        }

        public void OnConnectError(string message)
        {
            ClientConfig.WriteLine("Connect error: " + message, ConsoleColor.Red);
        }

        public void OnDisconnected()
        {
            ClientConfig.WriteLine("Client disconnected.", ConsoleColor.Red);
        }
    }
}
