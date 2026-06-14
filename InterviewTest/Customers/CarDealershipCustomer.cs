using InterviewTest.Orders;
using InterviewTest.Returns;

namespace InterviewTest.Customers
{
    public class CarDealershipCustomer : CustomerBase
    {
        public CarDealershipCustomer(
            OrderRepository orderRepo,
            ReturnRepository returnRepo,
            CustomerRepository customerRepository)
            : base(orderRepo, returnRepo, customerRepository)
        {
        }

        public override string GetName()
        {
            return "Ruxer Ford Lincoln, Inc.";
        }
    }
}
