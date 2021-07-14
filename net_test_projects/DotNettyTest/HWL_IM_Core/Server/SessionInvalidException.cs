using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Server
{
    public class SessionInvalidException : Exception
    {
        public SessionInvalidException() : base("User session is invalid")
        {
        }
    }
}
