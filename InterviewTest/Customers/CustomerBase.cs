using System.Collections.Generic;
using InterviewTest.Orders;
using InterviewTest.Returns;

namespace InterviewTest.Customers
{
  public abstract class CustomerBase : ICustomer
  {
    private readonly OrderRepository _orderRepository;
    private readonly ReturnRepository _returnRepository;
    private readonly CustomerRepository _customerRepository;

    protected CustomerBase(
      OrderRepository orderRepo,
      ReturnRepository returnRepo,
      CustomerRepository customerRepository)
    {
      _orderRepository = orderRepo;
      _returnRepository = returnRepo;
      _customerRepository = customerRepository;
    }

    public abstract string GetName();

    public void CreateOrder(IOrder order)
    {
      if (GetTotalSales() >= 500f)
      {
        foreach (var line in order.Products)
        {
          line.SellingPrice *= 0.9f;
        }
      }

      _orderRepository.Add(order);
    }

    public List<IOrder> GetOrders()
    {
      return _orderRepository.GetAll();
    }

    public void CreateReturn(IReturn rga)
    {
      _returnRepository.Add(rga);
    }

    public List<IReturn> GetReturns()
    {
      return _returnRepository.GetAll();
    }

    // Assignment requirement: totals on CustomerBase (ICustomer API).
    // Bonus: SQL implementation delegated to CustomerRepository.

    public float GetTotalSales()
    {
      return _customerRepository.GetTotalSales(GetName());
    }

    public float GetTotalReturns()
    {
      return _customerRepository.GetTotalReturns(GetName());
    }

    public float GetTotalProfit()
    {
      return _customerRepository.GetTotalProfit(GetName());
    }
  }
}
