using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaser2.Models
{
    public class ProductPhotosViewModel
    {
        public string Productphoto { get; set; }

        public IEnumerable<Photo> Item { get; set; }
        public IGrouping<string, Photo> Items { get; internal set; }
    }
}