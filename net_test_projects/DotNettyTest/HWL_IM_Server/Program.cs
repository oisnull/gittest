using HWL_IM_Core;
using HWL_IM_Core.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            LogHelper.InitConfigure();

            IMServerEngine server = new IMServerEngine("127.0.0.1", 8050, option =>
            {
                option.SessionAction = new ServerSessionManager();
                //option.UserAction = new DefaultServerUserAction();
                option.UserAction = new RdsUserAction();
                option.OfflineMessageAction = new DefaultOfflineMessageManager();
            });

            try
            {
                server.Bind();
                Console.WriteLine($"Server bind {server.Host}:{server.Port} success");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server bind {server.Host}:{server.Port} failure: {ex}");
            }

            q();
        }

        static void q()
        {
            Console.WriteLine("Press key 'q' to end.");
            while (true)
            {
                string key = Console.ReadLine();
                if (key?.ToLower() == "q")
                {
                    break;
                }
            }
        }
    }
}
