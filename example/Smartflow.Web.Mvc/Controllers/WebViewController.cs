using Smartflow.BussinessService.WorkflowService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Smartflow.Web.Mvc.Controllers
{
    public class WebViewController : Controller
    {
        private WorkflowEngine workflowEngine = BaseWorkflowEngine.CreateWorkflowEngine();

        private WorkflowDesignService designService = new WorkflowDesignService();

        public ActionResult StartWebPageView(string id)
        {
            WorkflowStructure structure = designService.GetWorkflowStructure(id);
            string instanceID = workflowEngine.Start(structure.STRUCTUREXML);
            WorkflowInstance workflowInstance = WorkflowInstance.GetInstance(instanceID);
            Stack<Smartflow.Elements.Form> stack = new Stack<Smartflow.Elements.Form>();
            WorkflowNode current = workflowInstance.Current;
            stack.Push(current.WebView);

            ViewBag.InstanceID = instanceID;
            ViewBag.Stack = stack;
            ViewBag.Transitions = current.Transitions;

            return View("~/Views/Shared/WebPageView.cshtml");
        }

        public ActionResult WebPageView(string instanceID)
        {
            WorkflowInstance workflowInstance = WorkflowInstance.GetInstance(instanceID);
            WorkflowNode current = workflowInstance.Current;
            Stack<Smartflow.Elements.Form> stack = new Stack<Smartflow.Elements.Form>();
            while (current != null)
            {
                if (current.WebView != null)
                {
                    stack.Push(current.WebView);
                }
                current = current.GetFromNode();
            }
            ViewBag.InstanceID = instanceID;
            ViewBag.Stack = stack;
            ViewBag.Transitions = current.Transitions;
            return View();
        }

        [HttpPost]
        public JsonResult GetWebView(Smartflow.Form.FormRelationship relation, string instanceID)
        {
            return Json(false);
        }

        [HttpPost]
        public JsonResult SaveWebView(Smartflow.Form.FormRelationship relation, string form)
        {
            return Json(false);
        }
    }
}
