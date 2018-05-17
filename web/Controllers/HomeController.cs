using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hawk.Common;
using Hawk.Exp.Models;

namespace Hawk.Exp.Controllers
{
    public class HomeController : Controller
    {
        log4net.ILog log = log4net.LogManager.GetLogger("home");

        // GET: Home
        public ActionResult Index()
        {

            //DataEx.AssemblyName = "mysql.data";
            //DataEx.FullName = "MySql.Data.MySqlClient.MySqlClientFactory";

            var sql = @"SELECT a.RoleId,a.MenuId,a.Power,a.Privilege,
            b.RoleName,b.IsAdmin,b.Remark as RoleRemark,
            c.`Name` as MenuName,c.DisplayName,c.ParentId,c.Url,c.Sort,c.Icon,c.Visible,c.Permission,c.Remark as MenuRemark
            FROM role_meun a LEFT JOIN role b ON a.RoleId = b.Id
            LEFT JOIN menu c on a.MenuId = c.Id WHERE RoleId IN
              (SELECT RoleId FROM user_role WHERE UserId = 5)";

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            IEnumerable<UserRole> roles = DataEx.Get<UserRole>(sql);
            stopwatch.Stop();

            LogHelper.Info(sql);

           // log.Info(sql);

            return Json(roles,JsonRequestBehavior.AllowGet);

            //Console.WriteLine(stopwatch.ElapsedMilliseconds);

            //int count = roles.Count();

            //Console.WriteLine(count);

            //return View();
        }

        // GET: Home/Details/5
        public ActionResult Xdf()
        {
            string[] s = new string[]
            {
               AppDomain.CurrentDomain.BaseDirectory,
               ControllerContext.RequestContext.HttpContext.Server.MapPath("/config/log4net")
            };
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Home/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Home/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
