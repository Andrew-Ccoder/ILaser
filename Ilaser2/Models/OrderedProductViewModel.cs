using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaser2.Models
{
    public class OrderedProductViewModel
    {
        public string ProductName { get; set; }

        public IEnumerable<Order_Product> Item { get; set; }
        public IGrouping<string, Order_Product> Items { get; internal set; }
    }
}