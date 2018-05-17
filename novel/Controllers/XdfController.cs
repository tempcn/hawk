using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace Hawk.Novel.Contro
{
    public class XdfController : Controller
    {
        readonly IHostingEnvironment _hosting;

        public XdfController(IHostingEnvironment hosting)
        {
            _hosting = hosting;
        }

        // GET: Xdf
        public ActionResult Index()
        {
            var s = new string[] {
                     AppDomain.CurrentDomain.BaseDirectory, //d:\git-demo\novel\bin\netcoreapp2.0\
                     AppContext.BaseDirectory,              //d:\git-demo\novel\bin\netcoreapp2.0\
                     _hosting.WebRootPath,                  //d:\git-demo\novel\wwwroot
                     _hosting.ContentRootPath               //d:\git-demo\novel
            };
            return Json(s);
        }

        // GET: Xdf/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Xdf/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Xdf/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Xdf/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Xdf/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Xdf/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Xdf/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}