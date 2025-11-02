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
    public class productsController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // GET: products
        // GET: products
        public async Task<ActionResult> Index()
        {
            var products = await db.products
                .Include(p => p.brands)
                .Include(p => p.categories)
                .ToListAsync();

            // ✅ Assign image to each product dynamically
            ViewBag.ProductImages = products.ToDictionary(p => p.product_id, p => GetProductImage(p));

            return View(products);
        }


        // GET: products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = await db.products
                .Include(p => p.brands)
                .Include(p => p.categories)
                .FirstOrDefaultAsync(p => p.product_id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            // ✅ Use the same helper method for image selection
            ViewBag.ImageFile = GetProductImage(product);

            return View(product);
        }



        // GET: products/Create
        public ActionResult Create()
        {
            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name");
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name");
            return View();
        }

        // POST: products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "product_id,product_name,brand_id,category_id,model_year,list_price")] products products)
        {
            if (ModelState.IsValid)
            {
                db.products.Add(products);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name", products.brand_id);
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", products.category_id);
            return View(products);
        }

        // GET: products/Edit/5
        // GET: products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = await db.products
                .Include(p => p.brands)
                .Include(p => p.categories)
                .FirstOrDefaultAsync(p => p.product_id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            // Compute image
            ViewBag.ImageFile = GetProductImage(product);

            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name", product.brand_id);
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", product.category_id);

            return View(product);
        }
        private string GetProductImage(products p)
        {
            string imageFile = "default_bike.jpeg";
            string brand = p.brands.brand_name.ToLower();
            string category = p.categories.category_name.ToLower();

            if (category.Contains("cruiser") && (brand.Contains("electra") || brand.Contains("ritchey") || brand.Contains("haro") || brand.Contains("pure")))
                imageFile = "Electra_cruiser.jpeg";
            else if (category.Contains("cruiser") && (brand.Contains("strider") || brand.Contains("trek") || brand.Contains("sun") || brand.Contains("surly") || brand.Contains("heller")))
                imageFile = "Electra_cruiser 1.jpeg";
            else if (category.Contains("comfort") && (brand.Contains("electra") || brand.Contains("ritchey") || brand.Contains("haro") || brand.Contains("pure")))
                imageFile = "Sun_Bicycles1.jpeg";
            else if (category.Contains("comfort") && (brand.Contains("strider") || brand.Contains("trek") || brand.Contains("sun") || brand.Contains("surly") || brand.Contains("heller")))
                imageFile = "Haro_Bikes1.jpeg";
            else if (category.Contains("cyclocross") && (brand.Contains("electra") || brand.Contains("ritchey") || brand.Contains("haro") || brand.Contains("pure")))
                imageFile = "Pure_Cycles1.jpeg";
            else if (category.Contains("cyclocross") && (brand.Contains("strider") || brand.Contains("trek") || brand.Contains("sun") || brand.Contains("surly") || brand.Contains("heller")))
                imageFile = "Pure_Cycles2.jpeg";
            else if (category.Contains("electric") && (brand.Contains("electra") || brand.Contains("ritchey") || brand.Contains("haro") || brand.Contains("pure")))
                imageFile = "Riychey_bike1.jpeg";
            else if (category.Contains("electric") && (brand.Contains("strider") || brand.Contains("trek") || brand.Contains("sun") || brand.Contains("surly") || brand.Contains("heller")))
                imageFile = "Ritchey_bike2.jpeg";
            else if (category.Contains("mountain") && (brand.Contains("electra") || brand.Contains("ritchey") || brand.Contains("haro") || brand.Contains("pure")))
                imageFile = "Haro_mountain_bike.jpeg";
            else if (category.Contains("mountain") && (brand.Contains("strider") || brand.Contains("trek") || brand.Contains("sun") || brand.Contains("surly") || brand.Contains("heller")))
                imageFile = "Heller_bike1.jpeg";
            else if (category.Contains("road") && (brand.Contains("electra") || brand.Contains("ritchey") || brand.Contains("haro") || brand.Contains("pure")))
                imageFile = "Surly_bike.jpeg";
            else if (category.Contains("road") && (brand.Contains("strider") || brand.Contains("trek") || brand.Contains("sun") || brand.Contains("surly") || brand.Contains("heller")))
                imageFile = "Surly_preamble_bike.jpeg";
            else if (category.Contains("child") && (brand.Contains("electra") || brand.Contains("ritchey") || brand.Contains("haro") || brand.Contains("pure")))
                imageFile = "ChildrensBike1.jpg";
            else if (category.Contains("child") && brand.Contains("strider"))
                imageFile = "Strider_balance_bike1.jpeg";
            else if (category.Contains("child") && (brand.Contains("strider") || brand.Contains("trek") || brand.Contains("sun") || brand.Contains("surly") || brand.Contains("heller")))
                imageFile = "ChildrensBike2.jpg";

            return imageFile;
        }


        // POST: products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "product_id,product_name,brand_id,category_id,model_year,list_price")] products products)
        {
            if (ModelState.IsValid)
            {
                db.Entry(products).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name", products.brand_id);
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", products.category_id);
            return View(products);
        }

        // GET: products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            products products = await db.products.FindAsync(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            products products = await db.products.FindAsync(id);
            db.products.Remove(products);
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
