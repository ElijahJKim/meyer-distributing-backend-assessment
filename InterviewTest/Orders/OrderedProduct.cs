using System;
using InterviewTest.Products;

namespace InterviewTest.Orders
{
    public class OrderedProduct
    {
        public OrderedProduct(IProduct product, DateTime purchasedAt, float sellingPrice)
        {
            Product = product;
            PurchasedAt = purchasedAt;
            SellingPrice = sellingPrice;
        }

        public IProduct Product { get; set; }
        public DateTime PurchasedAt { get; }
        public float SellingPrice { get; set; }
        public long Id { get; set; }
    }
}
