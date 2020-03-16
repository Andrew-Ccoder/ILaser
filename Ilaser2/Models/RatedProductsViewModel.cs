using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaser2.Models
{
    public class RatedProductsViewModel
    {
        public string ProductName { get; set; }

        public IEnumerable<Rating> Item { get; set; }
        public IGrouping<string, Rating> Items { get; internal set; }
    }
}