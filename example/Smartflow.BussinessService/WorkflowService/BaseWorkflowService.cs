/*
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://github.com/chengderen/Smartflow
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Smartflow.BussinessService.Models;
using Smartflow;
using Smartflow.Elements;
using System.Dynamic;
using Smartflow.BussinessService.Services;
using System.Threading.Tasks;

namespace Smartflow.BussinessService.WorkflowService
{
    public sealed class BaseWorkflowService
    {
        private static WorkflowEngine context = BaseWorkflowEngine.CreateWorkflowEngine();
        private readonly static BaseWorkflowService singleton = new BaseWorkflowService();
        private RecordService recordService = new RecordService();
        private PendingService pendingService = new PendingService();

        private BaseWorkflowService()
        {
            WorkflowEngine.OnProcess += new DelegatingProcessHandle(OnProcess);
            WorkflowEngine.OnCompleted += new DelegatingCompletedHandle(OnCompleted);
        }

        public static BaseWorkflowService Instance
        {
            get { return singleton; }
        }

        public void OnCompleted(ExecutingContext executeContext)
        {
            //以下代码仅用于演示
            //流程结束（在完成事件中可以做业务操作）
            FileApplyService applyService = new FileApplyService();
            FileApply model = applyService.Get(apply => apply.INSTANCEID == executeContext.Instance.InstanceID);
            model.STATUS = 8;
            applyService.Persistent(model);


            new PendingService().Delete(p => p.INSTANCEID == executeContext.Instance.InstanceID);
        }

        public void OnProcess(ExecutingContext executeContext)
        {
            if (executeContext.Instance.Current.NodeType == Enums.WorkflowNodeCategeory.Decision)
            {
                DecisionJump(executeContext);
            }
            else
            {
                //写入审批记录
                WriteRecord(executeContext);
                
                var current = GetCurrentNode(executeContext.Instance.InstanceID);
                if (current.APPELLATION == "结束")
                {
                    pendingService.Delete(p => p.INSTANCEID == executeContext.Instance.InstanceID);
                }
                else
                {
                    if (executeContext.Operation == Enums.WorkflowAction.Rollback)
                    {
                        //流程回退(谁审就退给谁) 仅限演示
                        var item = executeContext.Instance.Current.GetFromNode().GetActors().FirstOrDefault();
                        WritePending(item.IDENTIFICATION,executeContext);
                    }
                    else
                    {
                        //流程跳转|流程撤销(重新指派人审批) 仅限演示
                        List<Group> items = (executeContext.Operation == Enums.WorkflowAction.Jump) ? current.Groups :
                            executeContext.Instance.Current.GetFromNode().Groups;
                        List<User> userList = GetUsersByGroup(items);
                        foreach (User user in userList)
                        {
                            WritePending(user.IDENTIFICATION,executeContext);
                        }
                    }
                    pendingService.Delete(pending =>
                       pending.NODEID == executeContext.Instance.Current.NID &&
                       pending.INSTANCEID == executeContext.Instance.InstanceID);
                }
            }
        }

        /// <summary>
        /// 写入审批记录
        /// </summary>
        /// <param name="executeContext"></param>
        public void WriteRecord(ExecutingContext executeContext)
        {
            //写入审批记录
            recordService.Insert(new Record()
            {
                INSTANCEID = executeContext.Instance.InstanceID,
                NODENAME = executeContext.From.APPELLATION,
                MESSAGE = executeContext.Data.Message
            });
        }

        /// <summary>
        /// 写待办信息
        /// </summary>
        /// <param name="actorID">参与者</param>
        /// <param name="executeContext"></param>
        public void WritePending(long actorID, ExecutingContext executeContext)
        {
            pendingService.Insert(new Pending()
            {
                ACTORID = actorID,
                ACTION = executeContext.Operation.ToString(),
                INSTANCEID = executeContext.Instance.InstanceID,
                NODEID =  GetCurrentNode(executeContext.Instance.InstanceID).NID,
                APPELLATION = string.Format("<a href=\"javascript:;\" onclick=\"parent.window.document.getElementById('frmContent').src='../FileApply/FileApply/{0}'\">你有待办任务。</a>", executeContext.Data.bussinessID)
            });
        }

        /// <summary>
        /// 多条件跳转
        /// </summary>
        /// <param name="executeContext">执行上下文</param>
        private void DecisionJump(ExecutingContext executeContext)
        {
            pendingService.Delete(pending =>
                pending.NODEID == executeContext.Instance.Current.NID &&
                pending.INSTANCEID == executeContext.Instance.InstanceID);

            var current = GetCurrentNode(executeContext.Instance.InstanceID);
            if (executeContext.Operation == Enums.WorkflowAction.Jump && current.NodeType != Enums.WorkflowNodeCategeory.Decision)
            {
                List<User> userList = GetUsersByGroup(current.Groups);
                foreach (var user in userList)
                {
                    WritePending(user.IDENTIFICATION,executeContext);
                }
                
                pendingService.Delete(pending =>
                    pending.NODEID == executeContext.Instance.Current.NID &&
                    pending.INSTANCEID == executeContext.Instance.InstanceID);
            }
        }

        private List<User> GetUsersByGroup(List<Group> items)
        {
            List<string> gList = new List<string>();
            foreach (Group g in items)
            {
                gList.Add(g.IDENTIFICATION.ToString());
            }

            if (gList.Count == 0)
            {
                return new List<User>();
            }

            return new UserService().GetUserList(string.Join(",", gList));
        }

        public ASTNode GetCurrent(string instanceID)
        {
            return GetCurrentNode(instanceID);
        }

        public ASTNode GetCurrentPrevNode(string instanceID)
        {
            var current = GetCurrentNode(instanceID);
            return current.GetFromNode();
        }

        public WorkflowNode GetCurrentNode(string instanceID)
        {
            return WorkflowInstance.GetInstance(instanceID).Current;
        }

        public void UndoSubmit(string instanceID, long actorID, string actorName, string bussinessID)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(instanceID);
            dynamic dynData = new ExpandoObject();
            dynData.bussinessID = bussinessID;
            dynData.Message = "撤销此节点";
            context.Cancel(new WorkflowContext()
            {
                Instance = instance,
                Data = dynData,
                ActorID = actorID,
                ActorName = actorName
            });
        }

        public void Rollback(string instanceID, long actorID, string actorName, dynamic data)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(instanceID);
            context.Rollback(new WorkflowContext()
            {
                Instance = instance,
                Data = data,
                ActorID = actorID,
                ActorName = actorName
            });
        }

        public List<Group> GetCurrentActorGroup(string instanceID)
        {
            return WorkflowInstance.GetInstance(instanceID).Current.Groups;
        }

        public string Start(string identification)
        {
            return context.Start(identification);
        }

        public void Jump(string instanceID, string transitionID, long actorID, string actorName, dynamic data)
        {
            WorkflowInstance instance = WorkflowInstance.GetInstance(instanceID);
            context.Jump(new WorkflowContext()
            {
                Instance = instance,
                TransitionID = transitionID,
                Data = data,
                ActorID = actorID,
                ActorName = actorName
            });
        }
    }
}