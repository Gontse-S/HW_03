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
    public class order_itemsController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // GET: order_items
        public async Task<ActionResult> Index()
        {
            var order_items = db.order_items.Include(o => o.products).Include(o => o.orders);
            return View(await order_items.ToListAsync());
        }

        // GET: order_items/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            order_items order_items = await db.order_items.FindAsync(id);
            if (order_items == null)
            {
                return HttpNotFound();
            }
            return View(order_items);
        }

        // GET: order_items/Create
        public ActionResult Create()
        {
            ViewBag.product_id = new SelectList(db.products, "product_id", "product_name");
            ViewBag.order_id = new SelectList(db.orders, "order_id", "order_id");
            return View();
        }

        // POST: order_items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "order_id,item_id,product_id,quantity,list_price,discount")] order_items order_items)
        {
            if (ModelState.IsValid)
            {
                db.order_items.Add(order_items);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.product_id = new SelectList(db.products, "product_id", "product_name", order_items.product_id);
            ViewBag.order_id = new SelectList(db.orders, "order_id", "order_id", order_items.order_id);
            return View(order_items);
        }

        // GET: order_items/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            order_items order_items = await db.order_items.FindAsync(id);
            if (order_items == null)
            {
                return HttpNotFound();
            }
            ViewBag.product_id = new SelectList(db.products, "product_id", "product_name", order_items.product_id);
            ViewBag.order_id = new SelectList(db.orders, "order_id", "order_id", order_items.order_id);
            return View(order_items);
        }

        // POST: order_items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "order_id,item_id,product_id,quantity,list_price,discount")] order_items order_items)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order_items).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.product_id = new SelectList(db.products, "product_id", "product_name", order_items.product_id);
            ViewBag.order_id = new SelectList(db.orders, "order_id", "order_id", order_items.order_id);
            return View(order_items);
        }

        // GET: order_items/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            order_items order_items = await db.order_items.FindAsync(id);
            if (order_items == null)
            {
                return HttpNotFound();
            }
            return View(order_items);
        }

        // POST: order_items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            order_items order_items = await db.order_items.FindAsync(id);
            db.order_items.Remove(order_items);
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
