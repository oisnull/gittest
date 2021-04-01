using Operation.Factory;
using Operation.Factory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Operation.WebTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Test()
        {
            AssembliesManager assembliesManager = new AssembliesManager(@"E:\ExtInfo\github\gittest\net_test_projects\OperationExecutor\Operation.Factory.Implement\bin\Debug\Operation.Factory.Implement.dll", "Operation.Factory.Implement.UmsPublishFeedOperation");
            OperationExecuteResponse response = assembliesManager.Execute();
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test2()
        {
            AssembliesManager assembliesManager = new AssembliesManager(@"E:\ExtInfo\github\gittest\net_test_projects\OperationExecutor\Operation.Factory.Implement2\bin\Debug\Operation.Factory.Implement2.dll", "Operation.Factory.Implement2.SetWorkflowProcessDateOperation");
            OperationExecuteResponse response = assembliesManager.Execute();
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}