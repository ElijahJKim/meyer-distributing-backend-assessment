using InterviewTest.Orders;
using InterviewTest.Returns;

namespace InterviewTest.Customers
{
  public class DbCustomer : CustomerBase
  {
    private readonly string _name;

    public DbCustomer(
      string name,
      OrderRepository orderRepo,
      ReturnRepository returnRepo,
      CustomerRepository customerRepository)
      : base(orderRepo, returnRepo, customerRepository)
    {
      _name = name;
    }

    public override string GetName() => _name;
  }
}
