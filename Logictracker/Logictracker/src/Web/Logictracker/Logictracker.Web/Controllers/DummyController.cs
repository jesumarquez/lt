using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Logictracker.Web.Controllers
{
    [Route()]
    public class DummyController : BaseFunctionController, IFunctionController
    {
        // GET: Dummy
        public ActionResult Index()
        {
            return View();
        }

        // GET: Dummy/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Dummy/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dummy/Create
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

        // GET: Dummy/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Dummy/Edit/5
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

        // GET: Dummy/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Dummy/Delete/5
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

        public string VariableName { get { return "Dummy"; } }
        public string GetRefference() { return "Dummy"; }
    }
}
