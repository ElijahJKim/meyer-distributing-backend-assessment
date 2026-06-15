using System;
using System.Linq;
using InterviewTest.Customers;
using InterviewTest.Database;
using InterviewTest.Orders;
using InterviewTest.Products;
using InterviewTest.Reports;
using InterviewTest.Returns;
using InterviewTest.Services;

namespace InterviewTest
{
    public class Program
    {
        private static readonly ReturnRepository returnRepo = new ReturnRepository();
        private static readonly OrderRepository orderRepo = new OrderRepository(returnRepo);
        private static readonly ProductRepository productRepo = new ProductRepository();
        private static readonly CustomerRepository customerRepo = new CustomerRepository(orderRepo, returnRepo);
        private static readonly ExchangeService exchangeService = new ExchangeService();

        static void Main(string[] args)
        {
            // ------------------------
            // Coding Challenge Requirements
            // ------------------------

            // 1: Create a database, contained locally within this project, and refactor all repositories (Order, Return, and Product) to utilize it.
            // 2: Implement get total sales, returns, and profit in the CustomerBase class.
            // 3: Record when an item was purchased.
            // 4: Ensure all output results, when running this console app, are correct.

            // ------------------------
            // Bonus
            // ------------------------

            // 1: Refactor the customer classes to be repository/database based
            // 2: Create unit tests

            DatabaseInitializer.Initialize();
            returnRepo.SetOrderRepository(orderRepo);
            orderRepo.SetCustomerRepository(customerRepo);

            ProcessTruckAccessoriesExample();

            ProcessCarDealershipExample();

            ProcessExchangeExample();

            Console.ReadKey();
        }

        private static void ProcessTruckAccessoriesExample()
        {
            var customer = customerRepo.GetByName("Meyer Truck Equipment");

            IOrder order = new Order("TruckAccessoriesOrder123", customer);
            order.AddProduct(productRepo.GetByProductNumber("DrawTite 5504"));
            order.AddProduct(productRepo.GetByProductNumber("Rugged Liner F55U15"));
            customer.CreateOrder(order);

            IReturn rga = new Return("TruckAccessoriesReturn123", order);
            rga.AddProduct(order.Products.First());

            ConsoleWriteLineResults(customer);
            CustomerActivityReport.Print(customer);
        }

        private static void ProcessCarDealershipExample()
        {
            var customer = customerRepo.GetByName("Ruxer Ford Lincoln, Inc.");

            IOrder order = new Order("CarDealerShipOrder123", customer);
            order.AddProduct(productRepo.GetByProductNumber("Sherman 036-87-1"));
            order.AddProduct(productRepo.GetByProductNumber("Mobil 1 5W-30"));
            customer.CreateOrder(order);

            IReturn rga = new Return("CarDealerShipReturn123", order);
            rga.AddProduct(order.Products.First());
            customer.CreateReturn(rga);

            ConsoleWriteLineResults(customer);
            CustomerActivityReport.Print(customer);
        }

        private static void ProcessExchangeExample()
        {
            var customer = customerRepo.GetByName("Meyer Truck Equipment");

            IOrder originalOrder = new Order("Truck-777", customer);
            originalOrder.AddProduct(productRepo.GetByProductNumber("Rugged Liner F55U15"));
            customer.CreateOrder(originalOrder);

            var newProducts = new IProduct[]
            {
                productRepo.GetByProductNumber("DrawTite 5504"),
                productRepo.GetByProductNumber("Mobil 1 5W-30")
            };

            var netRefund = exchangeService.ProcessExchange(
                customer,
                originalOrder,
                originalOrder.Products.First(),
                "Truck-777-RGA",
                "Truck-777-EX",
                newProducts);

            Console.WriteLine("--- Exchange: Meyer Truck Equipment ---");
            Console.WriteLine("Returned: Rugged Liner F55U15 ($150)");
            Console.WriteLine("New order Truck-777-EX: DrawTite 5504 + Mobil 1 5W-30 ($95)");
            if (netRefund >= 0)
            {
                Console.WriteLine($"Net settlement: refund {netRefund.ToString("c")} to customer");
            }
            else
            {
                Console.WriteLine($"Net settlement: additional charge {(-netRefund).ToString("c")} from customer");
            }

            Console.WriteLine();
            CustomerActivityReport.Print(customer);
        }

        private static void ConsoleWriteLineResults(ICustomer customer)
        {
            Console.WriteLine(customer.GetName());

            Console.WriteLine($"Total Sales: {customer.GetTotalSales().ToString("c")}");

            Console.WriteLine($"Total Returns: {customer.GetTotalReturns().ToString("c")}");

            Console.WriteLine($"Total Profit: {customer.GetTotalProfit().ToString("c")}");

            Console.WriteLine();
        }
    }
}
