using System;
using System.Collections.Generic;
using InterviewTest.Customers;
using InterviewTest.Database;
using InterviewTest.Products;
using InterviewTest.Returns;
using Microsoft.Data.Sqlite;

namespace InterviewTest.Orders
{
    public class OrderRepository
    {
        private readonly ReturnRepository _returnRepository;
        private CustomerRepository _customerRepository;

        public OrderRepository(ReturnRepository returnRepository)
        {
            _returnRepository = returnRepository;
        }

        public void SetCustomerRepository(CustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public void Add(IOrder newOrder)
        {
            var connectionString = $"Data Source={DatabasePaths.GetDatabasePath()}";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Customers WHERE Name = @name";
            command.Parameters.AddWithValue("@name", newOrder.Customer.GetName());
            var customerId = command.ExecuteScalar();
            if (customerId == null)
            {
                throw new InvalidOperationException($"Customer not found: {newOrder.Customer.GetName()}");
            }

            command.Parameters.Clear();
            command.CommandText =
                "INSERT INTO Orders (OrderNumber, CustomerId) VALUES (@orderNumber, @customerId)";
            command.Parameters.AddWithValue("@orderNumber", newOrder.OrderNumber);
            command.Parameters.AddWithValue("@customerId", customerId);
            command.ExecuteNonQuery();

            command.Parameters.Clear();
            command.CommandText = "SELECT last_insert_rowid()";
            var orderId = (long)command.ExecuteScalar();

            foreach (var orderedProduct in newOrder.Products)
            {
                command.Parameters.Clear();
                command.CommandText =
                    "INSERT INTO OrderProducts (OrderId, ProductNumber, SellingPrice, PurchasedAt) " +
                    "VALUES (@orderId, @productNumber, @sellingPrice, @purchasedAt)";
                command.Parameters.AddWithValue("@orderId", orderId);
                command.Parameters.AddWithValue("@productNumber", orderedProduct.Product.GetProductNumber());
                command.Parameters.AddWithValue("@sellingPrice", orderedProduct.Product.GetSellingPrice());
                command.Parameters.AddWithValue("@purchasedAt", orderedProduct.PurchasedAt.ToString("o"));
                command.ExecuteNonQuery();

                command.Parameters.Clear();
                command.CommandText = "SELECT last_insert_rowid()";
                orderedProduct.Id = (long)command.ExecuteScalar();
            }
        }

        public void Remove(IOrder removedOrder)
        {
            var connectionString = $"Data Source={DatabasePaths.GetDatabasePath()}";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Orders WHERE OrderNumber = @orderNumber";
            command.Parameters.AddWithValue("@orderNumber", removedOrder.OrderNumber);
            command.ExecuteNonQuery();
        }

        public List<IOrder> GetAll()
        {
            var orders = new List<IOrder>();
            var connectionString = $"Data Source={DatabasePaths.GetDatabasePath()}";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var orderRows = new List<(long orderId, string orderNumber, string customerName)>();

            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT o.Id, o.OrderNumber, c.Name AS CustomerName " +
                    "FROM Orders o " +
                    "INNER JOIN Customers c ON o.CustomerId = c.Id";

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orderRows.Add((reader.GetInt64(0), reader.GetString(1), reader.GetString(2)));
                }
            }

            foreach (var (orderId, orderNumber, customerName) in orderRows)
            {
                var customer = CreateCustomer(customerName);
                var order = new Order(orderNumber, customer);

                using var productCommand = connection.CreateCommand();
                productCommand.CommandText =
                    "SELECT ProductNumber, SellingPrice, PurchasedAt " +
                    "FROM OrderProducts WHERE OrderId = @orderId";
                productCommand.Parameters.AddWithValue("@orderId", orderId);

                using var productReader = productCommand.ExecuteReader();
                while (productReader.Read())
                {
                    var productNumber = productReader.GetString(0);
                    var sellingPrice = productReader.GetFloat(1);
                    var purchasedAt = DateTime.Parse(productReader.GetString(2));
                    var product = CreateProduct(productNumber, sellingPrice);
                    order.Products.Add(new OrderedProduct(product, purchasedAt));
                }

                orders.Add(order);
            }

            return orders;
        }

        private ICustomer CreateCustomer(string customerName)
        {
            if (_customerRepository == null)
            {
                throw new InvalidOperationException("CustomerRepository must be set before loading orders.");
            }

            return _customerRepository.GetByName(customerName);
        }

        private static IProduct CreateProduct(string productNumber, float sellingPrice)
        {
            return new DbProduct(productNumber, sellingPrice);
        }
    }
}
