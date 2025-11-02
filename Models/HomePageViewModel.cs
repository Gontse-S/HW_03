using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HW_03.Models
{
    public class HomePageViewModel
    {
        public IEnumerable<staffs> StaffList
        {
            get; set;
        }
        public IEnumerable<customers> CustomerList { get; set; }
        public IEnumerable<products> ProductList { get; set; }
        public IEnumerable<brands> BrandList { get; set; }
        public IEnumerable<categories> CategoryList { get; set; }
    }
}