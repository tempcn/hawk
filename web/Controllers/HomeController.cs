﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using Hawk.Common;
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


            //Console.WriteLine(stopwatch.ElapsedMilliseconds);

            //int count = roles.Count();

            //Console.WriteLine(count);

            return View();
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
