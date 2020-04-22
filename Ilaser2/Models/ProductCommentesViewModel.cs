using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaser2.Models
{
    public class ProductCommentesViewModel
    {
        public string ProductName { get; set; }

        public IEnumerable<Comment> Item { get; set; }
        public IGrouping<string, Comment> Items { get; internal set; }
    }
}