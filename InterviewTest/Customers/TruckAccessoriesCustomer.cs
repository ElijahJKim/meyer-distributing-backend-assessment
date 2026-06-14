using InterviewTest.Orders;
using InterviewTest.Returns;

namespace InterviewTest.Customers
{
    public class TruckAccessoriesCustomer : CustomerBase
    {
        public TruckAccessoriesCustomer(
            OrderRepository orderRepo,
            ReturnRepository returnRepo,
            CustomerRepository customerRepository)
            : base(orderRepo, returnRepo, customerRepository)
        {
        }

        public override string GetName()
        {
            return "Meyer Truck Equipment";
        }
    }
}
