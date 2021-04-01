//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Operation.Factory.Configs
//{
//    public class EnvironmentConfig
//    {
//        /// <summary>
//        /// Prod
//        /// </summary>
//        public string LogicalEnvironment { get; set; }

//        /// <summary>
//        /// UMS
//        /// </summary>
//        public string CustomerEnvironment { get; set; }

//        /// <summary>
//        /// Targeting
//        /// </summary>
//        public string CustomerId { get; set; }

//        /// <summary>
//        /// Cosmos08 / Cosmos11
//        /// </summary>
//        public string VirtualCluster { get; set; }

//        /// <summary>
//        /// /d1/d2/d3
//        /// </summary>
//        /// <returns></returns>
//        public string GetEnvironmentDirectory()
//        {
//            return $"/{this.CustomerId}/{this.LogicalEnvironment}/{this.CustomerEnvironment}";
//        }
//    }
//}
