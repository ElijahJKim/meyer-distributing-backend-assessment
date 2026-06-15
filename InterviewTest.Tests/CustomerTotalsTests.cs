using System;
using System.IO;
using System.Linq;
using InterviewTest.Customers;
using InterviewTest.Database;
using InterviewTest.Orders;
using InterviewTest.Products;
using InterviewTest.Returns;
using InterviewTest.Services;

namespace InterviewTest.Tests
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly string _databasePath;

        public ProductRepositoryTests()
        {
            _databasePath = Path.Combine(Path.GetTempPath(), $"interview-test-{Guid.NewGuid():N}.db");
            DatabasePaths.SetDatabasePath(_databasePath);
            DatabaseInitializer.Initialize();
        }

        public void Dispose()
        {
            DatabasePaths.ResetDatabasePath();
            if (File.Exists(_databasePath))
            {
                File.Delete(_databasePath);
            }
        }

        [Fact]
        public void GetByProductNumber_ReturnsSeededProduct()
        {
            var repository = new ProductRepository();
            var product = repository.GetByProductNumber("DrawTite 5504");

            Assert.Equal("DrawTite 5504", product.GetProductNumber());
            Assert.Equal(70f, product.GetSellingPrice());
        }

        [Fact]
        public void GetAll_ReturnsFourSeededProducts()
        {
            var repository = new ProductRepository();
            var products = repository.GetAll();

            Assert.Equal(4, products.Count);
        }
    }

    public class CustomerTotalsTests : IDisposable
    {
        private readonly string _databasePath;
        private readonly ReturnRepository _returnRepository;
        private readonly OrderRepository _orderRepository;
        private readonly ProductRepository _productRepository;
        private readonly CustomerRepository _customerRepository;

        public CustomerTotalsTests()
        {
            _databasePath = Path.Combine(Path.GetTempPath(), $"interview-test-{Guid.NewGuid():N}.db");
            DatabasePaths.SetDatabasePath(_databasePath);
            DatabaseInitializer.Initialize();

            _returnRepository = new ReturnRepository();
            _orderRepository = new OrderRepository(_returnRepository);
            _returnRepository.SetOrderRepository(_orderRepository);
            _productRepository = new ProductRepository();
            _customerRepository = new CustomerRepository(_orderRepository, _returnRepository);
        }

        public void Dispose()
        {
            DatabasePaths.ResetDatabasePath();
            if (File.Exists(_databasePath))
            {
                File.Delete(_databasePath);
            }
        }

        [Fact]
        public void TruckAccessoriesCustomer_HasExpectedTotals()
        {
            var customer = _customerRepository.GetByName("Meyer Truck Equipment");
            var order = new Order("TruckAccessoriesOrder123", customer);
            order.AddProduct(_productRepository.GetByProductNumber("DrawTite 5504"));
            order.AddProduct(_productRepository.GetByProductNumber("Rugged Liner F55U15"));
            customer.CreateOrder(order);

            Assert.Equal(220f, customer.GetTotalSales());
            Assert.Equal(0f, customer.GetTotalReturns());
            Assert.Equal(220f, customer.GetTotalProfit());
        }

        [Fact]
        public void CarDealershipCustomer_HasExpectedTotals()
        {
            var customer = _customerRepository.GetByName("Ruxer Ford Lincoln, Inc.");
            var order = new Order("CarDealerShipOrder123", customer);
            order.AddProduct(_productRepository.GetByProductNumber("Sherman 036-87-1"));
            order.AddProduct(_productRepository.GetByProductNumber("Mobil 1 5W-30"));
            customer.CreateOrder(order);

            var rga = new Return("CarDealerShipReturn123", order);
            rga.AddProduct(order.Products.First());
            customer.CreateReturn(rga);

            Assert.Equal(180f, customer.GetTotalSales());
            Assert.Equal(155f, customer.GetTotalReturns());
            Assert.Equal(25f, customer.GetTotalProfit());
        }
    }

    public class ExchangeServiceTests : IDisposable
    {
        private readonly string _databasePath;
        private readonly ReturnRepository _returnRepository;
        private readonly OrderRepository _orderRepository;
        private readonly ProductRepository _productRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly ExchangeService _exchangeService;

        public ExchangeServiceTests()
        {
            _databasePath = Path.Combine(Path.GetTempPath(), $"interview-test-{Guid.NewGuid():N}.db");
            DatabasePaths.SetDatabasePath(_databasePath);
            DatabaseInitializer.Initialize();

            _returnRepository = new ReturnRepository();
            _orderRepository = new OrderRepository(_returnRepository);
            _returnRepository.SetOrderRepository(_orderRepository);
            _productRepository = new ProductRepository();
            _customerRepository = new CustomerRepository(_orderRepository, _returnRepository);
            _exchangeService = new ExchangeService();
        }

        public void Dispose()
        {
            DatabasePaths.ResetDatabasePath();
            if (File.Exists(_databasePath))
            {
                File.Delete(_databasePath);
            }
        }

        [Fact]
        public void ProcessExchange_BedLinerForHitchAndOil_RefundsFiftyFive()
        {
            var customer = _customerRepository.GetByName("Meyer Truck Equipment");
            var originalOrder = new Order("Truck-777", customer);
            originalOrder.AddProduct(_productRepository.GetByProductNumber("Rugged Liner F55U15"));
            customer.CreateOrder(originalOrder);

            var newProducts = new IProduct[]
            {
                _productRepository.GetByProductNumber("DrawTite 5504"),
                _productRepository.GetByProductNumber("Mobil 1 5W-30")
            };

            var netRefund = _exchangeService.ProcessExchange(
                customer,
                originalOrder,
                originalOrder.Products.First(),
                "Truck-777-RGA",
                "Truck-777-EX",
                newProducts);

            Assert.Equal(55f, netRefund);
            Assert.Equal(245f, customer.GetTotalSales());
            Assert.Equal(150f, customer.GetTotalReturns());
            Assert.Equal(95f, customer.GetTotalProfit());
        }
    }
}
