using HW_03.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;

namespace HW_03.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        
        private BikeStoresEntities db = new BikeStoresEntities();

        // Combined async Index
        public async Task<ActionResult> Index(string brandFilter, string categoryFilter, string clearFilters, int staffPage = 1, int customerPage = 1)
        {
            int pageSize = 1;

            // ✅ If Clear button was clicked, reset filters and reload page
            if (!string.IsNullOrEmpty(clearFilters))
            {
                return RedirectToAction("Index");
            }

            // Staff
            var staffList = await db.staffs
                .Include(s => s.orders.Select(o => o.order_items.Select(i => i.products)))
                .OrderBy(s => s.staff_id)
                .Skip((staffPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Customers
            var customerList = await db.customers
                .Include(c => c.orders.Select(o => o.order_items.Select(i => i.products)))
                .OrderBy(c => c.customer_id)
                .Skip((customerPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Products with optional filters
            var productsQuery = db.products
                .Include(p => p.brands)
                .Include(p => p.categories)
                .AsQueryable();

            if (!string.IsNullOrEmpty(brandFilter))
                productsQuery = productsQuery.Where(p => p.brands.brand_name == brandFilter);

            if (!string.IsNullOrEmpty(categoryFilter))
                productsQuery = productsQuery.Where(p => p.categories.category_name == categoryFilter);

            // Pass filters to dropdowns with selected value
            ViewBag.BrandFilter = new SelectList(
                await db.brands.Select(b => b.brand_name).ToListAsync(),
                selectedValue: brandFilter
            );

            ViewBag.CategoryFilter = new SelectList(
                await db.categories.Select(c => c.category_name).ToListAsync(),
                selectedValue: categoryFilter
            );

            var model = new HomePageViewModel
            {
                StaffList = staffList,
                CustomerList = customerList,
                ProductList = await productsQuery.ToListAsync()
            };

            ViewBag.StaffPage = staffPage;
            ViewBag.CustomerPage = customerPage;

            return View(model);
        }

        public async Task<ActionResult> About(int selectedStaffId = 0, int selectedCustomerId = 0, int selectedProductId = 0)
        {
            // Load all staff, customers, products with related orders and products
            var staffList = await db.staffs
                .Include(s => s.orders.Select(o => o.order_items.Select(i => i.products)))
                .OrderBy(s => s.staff_id)
                .ToListAsync();

            var customerList = await db.customers
                .Include(c => c.orders.Select(o => o.order_items.Select(i => i.products)))
                .OrderBy(c => c.customer_id)
                .ToListAsync();

            var productList = await db.products
                .Include(p => p.brands)
                .Include(p => p.categories)
                .OrderBy(p => p.product_id)
                .ToListAsync();

            // Default to first record if none selected
            if (selectedStaffId == 0 && staffList.Any())
                selectedStaffId = staffList.First().staff_id;

            if (selectedCustomerId == 0 && customerList.Any())
                selectedCustomerId = customerList.First().customer_id;

            if (selectedProductId == 0 && productList.Any())
                selectedProductId = productList.First().product_id;

            // Pass selected IDs to view
            ViewBag.SelectedStaffId = selectedStaffId;
            ViewBag.SelectedCustomerId = selectedCustomerId;
            ViewBag.SelectedProductId = selectedProductId;

            var model = new HomePageViewModel
            {
                StaffList = staffList,
                CustomerList = customerList,
                ProductList = productList
            };

            return View(model);
        }
        public async Task<ActionResult> Contact(int topCount = 8)
        {
            // Get report data for table (full data, or you can limit it)
            var reportData = await db.order_items
                .Include(i => i.products)
                .Include(i => i.products.categories)
                .Include(i => i.products.brands)
                .GroupBy(i => new
                {
                    ProductName = i.products.product_name,
                    Brand = i.products.brands.brand_name,
                    Category = i.products.categories.category_name
                })
                .Select(g => new PopularProductsViewModel
                {
                    ProductName = g.Key.ProductName,
                    Brand = g.Key.Brand,
                    Category = g.Key.Category,
                    TimesOrdered = g.Count(),
                    TotalQuantity = g.Sum(x => x.quantity)
                })
                .OrderByDescending(r => r.TimesOrdered)
                .ToListAsync();

            ViewBag.TopCount = topCount; // pass the topCount to the view
            ViewBag.ReportData = reportData;

            return View(reportData);
        }

        private Chart CreatePieChart(List<PopularProductsViewModel> reportData, string title)
        {
            var chart = new Chart();
            chart.Width = 800;
            chart.Height = 400;
            chart.Titles.Add(title);
            chart.ChartAreas.Add(new ChartArea());

            var series = new Series("Popularity")
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true
            };

            series.Points.DataBindXY(
                reportData.Select(r => r.ProductName).ToArray(),
                reportData.Select(r => r.TimesOrdered).ToArray()
            );

            foreach (var point in series.Points)
            {
                point.Label = "#PERCENT{P1}"; // percentage with 1 decimal
                point.LegendText = "#VALX";    // product name in legend
            }

            chart.Series.Add(series);
            chart.Legends.Add(new Legend("Legend"));

            return chart;
        }




        // =============================
        // 📊 Pie Chart Image
        // =============================
        public async Task<ActionResult> ChartImage(int topCount = 8)
        {
            // --- Use same grouping as Contact() ---
            var allProducts = await db.order_items
                .Include(i => i.products)
                .Include(i => i.products.categories)
                .Include(i => i.products.brands)
                .GroupBy(i => new
                {
                    ProductName = i.products.product_name,
                    Brand = i.products.brands.brand_name,
                    Category = i.products.categories.category_name
                })
                .Select(g => new PopularProductsViewModel
                {
                    ProductName = g.Key.ProductName,
                    Brand = g.Key.Brand,
                    Category = g.Key.Category,
                    TimesOrdered = g.Count(),
                    TotalQuantity = g.Sum(x => x.quantity)
                })
                .ToListAsync();

            double totalOrders = allProducts.Sum(p => p.TimesOrdered);

            // --- Take top-N products ---
            var topProducts = allProducts
                .OrderByDescending(p => p.TimesOrdered)
                .Take(topCount)
                .ToList();

            // --- Calculate percentages ---
            foreach (var item in topProducts)
                item.Percentage = totalOrders > 0 ? (double)item.TimesOrdered / totalOrders * 100 : 0;

            // --- Create chart using consistent data ---
            var chart = CreatePieChart(topProducts, $"Top {topCount} Popular Products");

            // --- Ensure labels show percentages ---
            foreach (var point in chart.Series["Popularity"].Points)
            {
                point.Label = "#PERCENT{P1}";
                point.LegendText = "#VALX";
            }

            using (var stream = new MemoryStream())
            {
                chart.SaveImage(stream, ChartImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream.ToArray(), "image/png");
            }
        }





        // =============================
        // 💾 Save Report (Pie Chart)
        // =============================
        [HttpPost]
        public async Task<ActionResult> SaveReport(string reportName, string fileType, string reportDescription, int topCount = 8)
        {
            string folderPath = Server.MapPath("~/ReportArchive/");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, $"{reportName}.png");
            string descPath = Path.Combine(folderPath, $"{reportName}.txt");

            // --- Get product data ---
            var allProducts = await db.order_items
                .Include(i => i.products)
                .Include(i => i.products.brands)
                .Include(i => i.products.categories)
                .GroupBy(i => new
                {
                    ProductName = i.products.product_name,
                    Brand = i.products.brands.brand_name,
                    Category = i.products.categories.category_name
                })
                .Select(g => new PopularProductsViewModel
                {
                    ProductName = g.Key.ProductName,
                    Brand = g.Key.Brand,
                    Category = g.Key.Category,
                    TimesOrdered = g.Count(),
                    TotalQuantity = g.Sum(x => x.quantity)
                })
                .ToListAsync();

            double totalOrders = allProducts.Sum(p => p.TimesOrdered);

            var topProducts = allProducts
                .OrderByDescending(p => p.TimesOrdered)
                .Take(topCount)
                .Select(p =>
                {
                    p.Percentage = totalOrders > 0 ? (double)p.TimesOrdered / totalOrders * 100 : 0;
                    return p;
                })
                .ToList();

            // --- Create the pie chart ---
            var chart = CreatePieChart(topProducts, $"Top {topCount} Popular Products");

            // ============================
            // OPTION 1: CHART ONLY (PNG)
            // ============================
            if (fileType == "png")
            {
                using (var ms = new MemoryStream())
                {
                    chart.SaveImage(ms, ChartImageFormat.Png);
                    System.IO.File.WriteAllBytes(filePath, ms.ToArray());
                }
            }

            // =====================================
            // OPTION 2: CHART + TABLE (still PNG)
            // =====================================
            else if (fileType == "pdf") // just reuse “pdf” choice but keep PNG
            {
                int width = 1000;
                int height = 1400;
                using (var bmp = new Bitmap(width, height))
                using (var g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);

                    // Draw the chart
                    using (var ms = new MemoryStream())
                    {
                        chart.SaveImage(ms, ChartImageFormat.Png);
                        var chartImage = Image.FromStream(ms);
                        g.DrawImage(chartImage, new Rectangle(50, 20, width - 100, 400));
                    }

                    // --- Draw table ---
                    int startY = 450;
                    int rowHeight = 40;
                    int colX = 50;

                    // Adjusted column widths (total ≈ 1100px)
                    int[] colWidths = { 250, 140, 160, 160, 160, 200 };
                    string[] headers = { "Name", "Brand", "Category", "Times Ordered", "Quantity", "%" };

                    // Text formatting (centered + wrapping for headers)
                    StringFormat headerFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.Word,
                        FormatFlags = StringFormatFlags.LineLimit
                    };

                    StringFormat cellFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };

                    // --- Draw header row ---
                    int currentX = colX;
                    for (int i = 0; i < headers.Length; i++)
                    {
                        RectangleF headerRect = new RectangleF(currentX, startY, colWidths[i], rowHeight * 1.5f);
                        g.DrawRectangle(Pens.Black, headerRect.X, headerRect.Y, headerRect.Width, headerRect.Height);
                        g.DrawString(headers[i], new Font("Arial", 10, FontStyle.Bold), Brushes.Black, headerRect, headerFormat);
                        currentX += colWidths[i];
                    }

                    // --- Draw data rows ---
                    for (int i = 0; i < topProducts.Count; i++)
                    {
                        int y = (int)(startY + (i + 1.5f) * rowHeight);
                        var item = topProducts[i];
                        currentX = colX;

                        string[] values =
                        {
        item.ProductName.Length > 35 ? item.ProductName.Substring(0, 32) + "..." : item.ProductName,
        item.Brand,
        item.Category,
        item.TimesOrdered.ToString(),
        item.TotalQuantity.ToString(),
        item.Percentage.ToString("0.0") + "%"
    };

                        for (int j = 0; j < values.Length; j++)
                        {
                            RectangleF cellRect = new RectangleF(currentX, y, colWidths[j], rowHeight);
                            g.DrawRectangle(Pens.Black, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                            g.DrawString(values[j], new Font("Arial", 10), Brushes.Black, cellRect, cellFormat);
                            currentX += colWidths[j];
                        }
                    }



                    bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                }
            }

            // --- Save optional description ---
            if (!string.IsNullOrEmpty(reportDescription))
                System.IO.File.WriteAllText(descPath, reportDescription);

            TempData["Message"] = "Report saved successfully!";
            return RedirectToAction("Contact", new { topCount });
        }






        // =============================
        // Download
        // =============================
        public ActionResult Download(string fileName)
        {
            string filePath = Server.MapPath("~/ReportArchive/" + fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }

        // =============================
        // Delete
        // =============================
        public ActionResult Delete(string fileName)
        {
            string filePath = Server.MapPath("~/ReportArchive/" + fileName);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            return RedirectToAction("Contact");
        }
    }
}
