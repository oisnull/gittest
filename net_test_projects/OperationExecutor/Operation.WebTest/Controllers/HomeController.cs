using Newtonsoft.Json;
using Operation.Factory;
using Operation.Factory.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Operation.WebTest.Controllers
{
    public class HomeController : Controller
    {
        static readonly string PluginsWorkDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");

        public ActionResult Test()
        {
            Dictionary<string, string> requestParams = new Dictionary<string, string>()
            {
                {"InputParam1","input test 1" },
                {"InputParam2","input test 2" },
            };
            AssemblyManagerV2 assembliesManager = new AssemblyManagerV2($"{PluginsWorkDirectory}\\Implement\\Operation.Factory.Implement.dll", "Operation.Factory.Implement.UmsPublishFeedOperation");
            OperationExecuteResponse response = assembliesManager.Execute(requestParams);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test2()
        {
            AssemblyManagerV2 assembliesManager = new AssemblyManagerV2($"{PluginsWorkDirectory}\\Implement2\\Operation.Factory.Implement2.dll", "Operation.Factory.Implement2.SetWorkflowProcessDateOperation");
            OperationExecuteResponse response = assembliesManager.Execute();
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProxyTest()
        {
            Dictionary<string, string> requestParams = new Dictionary<string, string>()
            {
                {"InputParam1","input test 1" },
                {"InputParam2","input test 2" },
            };
            AssemblyProxyManager proxyManager = new AssemblyProxyManager();
            AssemblyManager assembliesManager = proxyManager.CreateInstance($"{PluginsWorkDirectory}\\Implement\\Operation.Factory.Implement.dll", "Operation.Factory.Implement.UmsPublishFeedOperation");
            OperationExecuteResponse response = assembliesManager.Execute(requestParams);
            string outputs = JsonConvert.SerializeObject(response);
            proxyManager.UnloadInstance();
            return Content(outputs);
        }

        public ActionResult ProxyTest2()
        {
            AssemblyProxyManager proxyManager = new AssemblyProxyManager();
            AssemblyManager assembliesManager = proxyManager.CreateInstance($"{PluginsWorkDirectory}\\Implement2\\Operation.Factory.Implement2.dll", "Operation.Factory.Implement2.SetWorkflowProcessDateOperation");
            OperationExecuteResponse response = assembliesManager.Execute();
            string outputs = JsonConvert.SerializeObject(response);
            proxyManager.UnloadInstance();
            return Content(outputs);
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}