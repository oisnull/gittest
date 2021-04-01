using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Factory.Models
{
    public class OperationExecuteRequest
    {
        /// <summary>
        /// Targeting
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// UMS
        /// </summary>
        public string CustomerEnvironment { get; set; }

        public string Assembly { get; set; }
        public string Class { get; set; }
        public Dictionary<string, string> OperationParameters { get; set; }

        /// <summary>
        /// ex: /d1/d2/d3
        /// </summary>
        /// <returns></returns>
        public string GetAssemblyPath()
        {
            return Path.Combine(this.CustomerId, this.CustomerEnvironment, this.Assembly);
        }
    }
}
