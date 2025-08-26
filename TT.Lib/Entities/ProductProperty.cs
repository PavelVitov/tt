using System;
using System.Collections.Generic;
using System.Text;

namespace TT.Lib.Entities
{
    public class ProductProperty : Base
    {
        public string Value { get; set; }

        public virtual Product Product { get; set; }

        public int ProductId { get; set; }

        public virtual Property Property { get; set; }

        public int PropertyId { get; set; }

        //The new field for task 2
        public string Type { get; set; }
        
    }
}
