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
    public class brandsController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // GET: brands
        public async Task<ActionResult> Index()
        {
            return View(await db.brands.ToListAsync());
        }

        // GET: brands/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            brands brands = await db.brands.FindAsync(id);
            if (brands == null)
            {
                return HttpNotFound();
            }
            return View(brands);
        }

        // GET: brands/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: brands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "brand_id,brand_name")] brands brands)
        {
            if (ModelState.IsValid)
            {
                db.brands.Add(brands);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(brands);
        }

        // GET: brands/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            brands brands = await db.brands.FindAsync(id);
            if (brands == null)
            {
                return HttpNotFound();
            }
            return View(brands);
        }

        // POST: brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "brand_id,brand_name")] brands brands)
        {
            if (ModelState.IsValid)
            {
                db.Entry(brands).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(brands);
        }

        // GET: brands/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            brands brands = await db.brands.FindAsync(id);
            if (brands == null)
            {
                return HttpNotFound();
            }
            return View(brands);
        }

        // POST: brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            brands brands = await db.brands.FindAsync(id);
            db.brands.Remove(brands);
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
