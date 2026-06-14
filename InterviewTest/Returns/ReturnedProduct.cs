using System;
using InterviewTest.Orders;

namespace InterviewTest.Returns
{
    public class ReturnedProduct
    {
        public ReturnedProduct(OrderedProduct product, DateTime returnedAt)
        {
            OrderProduct = product;
            ReturnedAt = returnedAt;
        }

        public OrderedProduct OrderProduct { get; set; }
        public DateTime ReturnedAt { get; }
    }
}
