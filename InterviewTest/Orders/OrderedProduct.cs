using System;
using InterviewTest.Products;

namespace InterviewTest.Orders
{
    public class OrderedProduct
    {
        public OrderedProduct(IProduct product, DateTime purchasedAt)
        {
            Product = product;
            PurchasedAt = purchasedAt;
        }

        public IProduct Product { get; set; }
        public DateTime PurchasedAt { get; }
        public long Id { get; set; }
    }
}
