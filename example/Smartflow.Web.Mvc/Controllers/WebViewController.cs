using Newtonsoft.Json;
using Smartflow.BussinessService;
using Smartflow.BussinessService.WorkflowService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Smartflow.Form;

namespace Smartflow.Web.Mvc.Controllers
{
    public class WebViewController : Controller
    {
        private WorkflowEngine workflowEngine = BaseWorkflowEngine.CreateWorkflowEngine();

        private WorkflowDesignService designService = new WorkflowDesignService();

        public ActionResult StartWebPageView(string id)
        {
            WorkflowStructure structure = designService.GetWorkflowStructure(id);
            Smartflow.Elements.Form webForm = workflowEngine.Ready(structure.STRUCTUREXML);
            Stack<Smartflow.Elements.Form> stack = new Stack<Smartflow.Elements.Form>();
            stack.Push(webForm);
            ViewBag.Stack = stack;
            ViewBag.ID = id;
            return View();
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

        /// <summary>
        /// 获取记录
        /// </summary>
        /// <param name="relation"></param>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetWebView(Relation relation, string instanceID)
        {
            return Json(DynamicRepository.GetInstance(instanceID, relation));
        }


        /// <summary>
        /// 保存视图
        /// </summary>
        /// <param name="relation"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveWebView(string relation, string form,string resourceID)
        {
            Relation r = JsonConvert.DeserializeObject<Relation>(relation);
            Object proxy = JsonConvert.DeserializeObject(form,DynamicRepository.BuildDynamicObjectType(r));
            WorkflowStructure structure = designService.GetWorkflowStructure(resourceID);
            IBase entity = (proxy as IBase);
            entity.INSTANCEID = workflowEngine.Start(structure.STRUCTUREXML);
            entity.RESOURCEID = resourceID;
            DynamicRepository.Persistent(entity, r);
            return Json(true);
        }
    }
}
