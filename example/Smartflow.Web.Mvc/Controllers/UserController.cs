using Smartflow.BussinessService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Smartflow.Web.Mvc.Controllers
{
    public class UserController : Controller
    {
        private UserService userService = new UserService();

        //
        // GET: /User/
        public ActionResult UserList()
        {
            return View(userService.GetUserList());
        }

        //
        // GET: /User/
        public ActionResult RoleStatistics()
        {
            return View(userService.GetStatisticsDataTable());
        }

        public JsonResult GetUser(string userName)
        {
            //演示使用
            Smartflow.BussinessService.Models.User userInfo = new UserService().GetUser(userName);

            if (userInfo == null)
            {
                return Json(false);
            }
            else
            {
                System.Web.HttpContext.Current.Session["user"] = userInfo;
                return Json(true);
            }
        }
    }
}
