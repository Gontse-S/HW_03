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
    public class ordersController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // GET: orders
        public async Task<ActionResult> Index()
        {
            var orders = db.orders.Include(o => o.customers).Include(o => o.staffs).Include(o => o.stores);
            return View(await orders.ToListAsync());
        }

        // GET: orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            orders orders = await db.orders.FindAsync(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // GET: orders/Create
        public ActionResult Create()
        {
            ViewBag.customer_id = new SelectList(db.customers, "customer_id", "first_name");
            ViewBag.staff_id = new SelectList(db.staffs, "staff_id", "first_name");
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name");
            return View();
        }

        // POST: orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "order_id,customer_id,order_status,order_date,required_date,shipped_date,store_id,staff_id")] orders orders)
        {
            if (ModelState.IsValid)
            {
                db.orders.Add(orders);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.customer_id = new SelectList(db.customers, "customer_id", "first_name", orders.customer_id);
            ViewBag.staff_id = new SelectList(db.staffs, "staff_id", "first_name", orders.staff_id);
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", orders.store_id);
            return View(orders);
        }

        // GET: orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            orders orders = await db.orders.FindAsync(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            ViewBag.customer_id = new SelectList(db.customers, "customer_id", "first_name", orders.customer_id);
            ViewBag.staff_id = new SelectList(db.staffs, "staff_id", "first_name", orders.staff_id);
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", orders.store_id);
            return View(orders);
        }

        // POST: orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "order_id,customer_id,order_status,order_date,required_date,shipped_date,store_id,staff_id")] orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orders).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.customer_id = new SelectList(db.customers, "customer_id", "first_name", orders.customer_id);
            ViewBag.staff_id = new SelectList(db.staffs, "staff_id", "first_name", orders.staff_id);
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", orders.store_id);
            return View(orders);
        }

        // GET: orders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            orders orders = await db.orders.FindAsync(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            orders orders = await db.orders.FindAsync(id);
            db.orders.Remove(orders);
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
