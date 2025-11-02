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
    public class stocksController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // GET: stocks
        public async Task<ActionResult> Index()
        {
            var stocks = db.stocks.Include(s => s.products).Include(s => s.stores);
            return View(await stocks.ToListAsync());
        }

        // GET: stocks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            stocks stocks = await db.stocks.FindAsync(id);
            if (stocks == null)
            {
                return HttpNotFound();
            }
            return View(stocks);
        }

        // GET: stocks/Create
        public ActionResult Create()
        {
            ViewBag.product_id = new SelectList(db.products, "product_id", "product_name");
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name");
            return View();
        }

        // POST: stocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "store_id,product_id,quantity")] stocks stocks)
        {
            if (ModelState.IsValid)
            {
                db.stocks.Add(stocks);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.product_id = new SelectList(db.products, "product_id", "product_name", stocks.product_id);
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", stocks.store_id);
            return View(stocks);
        }

        // GET: stocks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            stocks stocks = await db.stocks.FindAsync(id);
            if (stocks == null)
            {
                return HttpNotFound();
            }
            ViewBag.product_id = new SelectList(db.products, "product_id", "product_name", stocks.product_id);
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", stocks.store_id);
            return View(stocks);
        }

        // POST: stocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "store_id,product_id,quantity")] stocks stocks)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stocks).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.product_id = new SelectList(db.products, "product_id", "product_name", stocks.product_id);
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", stocks.store_id);
            return View(stocks);
        }

        // GET: stocks/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            stocks stocks = await db.stocks.FindAsync(id);
            if (stocks == null)
            {
                return HttpNotFound();
            }
            return View(stocks);
        }

        // POST: stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            stocks stocks = await db.stocks.FindAsync(id);
            db.stocks.Remove(stocks);
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
