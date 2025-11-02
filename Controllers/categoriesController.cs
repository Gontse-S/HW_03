using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HW_03.Models;

namespace HW_03.Controllers
{
    public class categoriesController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // GET: categories
        public async Task<ActionResult> Index()
        {
            return View(await db.categories.ToListAsync());
        }

        // GET: categories/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            categories categories = await db.categories.FindAsync(id);
            if (categories == null)
            {
                return HttpNotFound();
            }
            return View(categories);
        }

        // GET: categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "category_id,category_name")] categories categories)
        {
            if (ModelState.IsValid)
            {
                db.categories.Add(categories);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(categories);
        }

        // GET: categories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            categories categories = await db.categories.FindAsync(id);
            if (categories == null)
            {
                return HttpNotFound();
            }
            return View(categories);
        }

        // POST: categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "category_id,category_name")] categories categories)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categories).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(categories);
        }

        // GET: categories/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            categories categories = await db.categories.FindAsync(id);
            if (categories == null)
            {
                return HttpNotFound();
            }
            return View(categories);
        }

        // POST: categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            categories categories = await db.categories.FindAsync(id);
            db.categories.Remove(categories);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
