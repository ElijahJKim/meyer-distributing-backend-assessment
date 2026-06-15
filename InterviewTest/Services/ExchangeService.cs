using System.Collections.Generic;
using System.Linq;
using InterviewTest.Customers;
using InterviewTest.Orders;
using InterviewTest.Products;
using InterviewTest.Returns;

namespace InterviewTest.Services
{
    public class ExchangeService
    {
        public float ProcessExchange(
            ICustomer customer,
            IOrder originalOrder,
            OrderedProduct lineToReturn,
            string returnNumber,
            string newOrderNumber,
            IReadOnlyList<IProduct> newProducts)
        {
            var rga = new Return(returnNumber, originalOrder);
            rga.AddProduct(lineToReturn);
            customer.CreateReturn(rga);

            var newOrder = new Order(newOrderNumber, customer);
            foreach (var product in newProducts)
            {
                newOrder.AddProduct(product);
            }

            customer.CreateOrder(newOrder);

            var returnedAmount = lineToReturn.Product.GetSellingPrice();
            var newOrderTotal = newProducts.Sum(p => p.GetSellingPrice());
            return returnedAmount - newOrderTotal;
        }
    }
}
