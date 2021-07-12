using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Client
{
    public static class ClientConfig
    {
        //public static ImMessageHead DefaultHead
        //{
        //    get
        //    {
        //        return new ImMessageHead()
        //        {
        //            Client = "windows",
        //            Language = "c#",
        //            Session = null,
        //            Version = "1.0.0",
        //            Timestamp = 0,
        //        };
        //    }
        //}

        public static void WriteLine(string message, ConsoleColor? color = null)
        {
            if (color == null)
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.ForegroundColor = color.Value;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }
}
