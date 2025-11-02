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
    public class storesController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // GET: stores
        public async Task<ActionResult> Index()
        {
            return View(await db.stores.ToListAsync());
        }

        // GET: stores/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            stores stores = await db.stores.FindAsync(id);
            if (stores == null)
            {
                return HttpNotFound();
            }
            return View(stores);
        }

        // GET: stores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: stores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "store_id,store_name,phone,email,street,city,state,zip_code")] stores stores)
        {
            if (ModelState.IsValid)
            {
                db.stores.Add(stores);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(stores);
        }

        // GET: stores/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            stores stores = await db.stores.FindAsync(id);
            if (stores == null)
            {
                return HttpNotFound();
            }
            return View(stores);
        }

        // POST: stores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "store_id,store_name,phone,email,street,city,state,zip_code")] stores stores)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stores).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(stores);
        }

        // GET: stores/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            stores stores = await db.stores.FindAsync(id);
            if (stores == null)
            {
                return HttpNotFound();
            }
            return View(stores);
        }

        // POST: stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            stores stores = await db.stores.FindAsync(id);
            db.stores.Remove(stores);
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
