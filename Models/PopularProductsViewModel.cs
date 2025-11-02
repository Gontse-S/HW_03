using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HW_03.Models
{
    public class PopularProductsViewModel
    {
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public int TimesOrdered { get; set; }
        public int TotalQuantity { get; set; }

        public double Percentage { get; set; }  
    }
}