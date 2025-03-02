using System.Collections.Generic;

namespace Njenga.Models
{
    public class SaleRequest
    {
        public List<SaleProduct> Products { get; set; } = new List<SaleProduct>();
    }

    public class SaleProduct
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
