using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Factory.Models
{
    public class OperationExecuteResponse : MarshalByRefObject
    {
        public OperationResult Result { get; set; }
        public Dictionary<string, string> Outputs { get; set; }
    }
}
