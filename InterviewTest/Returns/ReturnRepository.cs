using System;
using System.Collections.Generic;
using System.Linq;
using InterviewTest.Database;
using InterviewTest.Orders;
using Microsoft.Data.Sqlite;

namespace InterviewTest.Returns
{
    public class ReturnRepository
    {
        private OrderRepository _orderRepository;

        public void SetOrderRepository(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public void Add(IReturn newReturn)
        {
            var connectionString = $"Data Source={DatabasePaths.GetDatabasePath()}";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Orders WHERE OrderNumber = @orderNumber";
            command.Parameters.AddWithValue("@orderNumber", newReturn.OriginalOrder.OrderNumber);
            var orderId = command.ExecuteScalar();
            if (orderId == null)
            {
                throw new InvalidOperationException($"Order not found: {newReturn.OriginalOrder.OrderNumber}");
            }

            command.Parameters.Clear();
            command.CommandText =
                "INSERT INTO Returns (ReturnNumber, OrderId) VALUES (@returnNumber, @orderId)";
            command.Parameters.AddWithValue("@returnNumber", newReturn.ReturnNumber);
            command.Parameters.AddWithValue("@orderId", orderId);
            command.ExecuteNonQuery();

            command.Parameters.Clear();
            command.CommandText = "SELECT last_insert_rowid()";
            var returnId = (long)command.ExecuteScalar();

            foreach (var returnedProduct in newReturn.ReturnedProducts)
            {
                var orderProductId = returnedProduct.OrderProduct.Id;
                if (orderProductId == 0)
                {
                    command.Parameters.Clear();
                    command.CommandText =
                        "SELECT Id FROM OrderProducts WHERE OrderId = @orderId AND ProductNumber = @productNumber";
                    command.Parameters.AddWithValue("@orderId", orderId);
                    command.Parameters.AddWithValue("@productNumber", returnedProduct.OrderProduct.Product.GetProductNumber());
                    var lookupId = command.ExecuteScalar();
                    if (lookupId == null)
                    {
                        throw new InvalidOperationException(
                            $"Order product not found: {returnedProduct.OrderProduct.Product.GetProductNumber()}");
                    }

                    orderProductId = (long)lookupId;
                }

                command.Parameters.Clear();
                command.CommandText =
                    "INSERT INTO ReturnProducts (ReturnId, OrderProductId, ReturnedAt) " +
                    "VALUES (@returnId, @orderProductId, @returnedAt)";
                command.Parameters.AddWithValue("@returnId", returnId);
                command.Parameters.AddWithValue("@orderProductId", orderProductId);
                command.Parameters.AddWithValue("@returnedAt", returnedProduct.ReturnedAt.ToString("o"));
                command.ExecuteNonQuery();
            }
        }

        public void Remove(IReturn removedReturn)
        {
            var connectionString = $"Data Source={DatabasePaths.GetDatabasePath()}";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Returns WHERE ReturnNumber = @returnNumber";
            command.Parameters.AddWithValue("@returnNumber", removedReturn.ReturnNumber);
            command.ExecuteNonQuery();
        }

        public List<IReturn> GetAll()
        {
            if (_orderRepository == null)
            {
                throw new InvalidOperationException("OrderRepository must be set before calling Get.");
            }

            var returns = new List<IReturn>();
            var orders = _orderRepository.GetAll();
            var connectionString = $"Data Source={DatabasePaths.GetDatabasePath()}";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT r.ReturnNumber, o.OrderNumber " +
                "FROM Returns r " +
                "INNER JOIN Orders o ON r.OrderId = o.Id";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var returnNumber = reader.GetString(0);
                var orderNumber = reader.GetString(1);
                var originalOrder = orders.FirstOrDefault(o => o.OrderNumber == orderNumber);
                if (originalOrder == null)
                {
                    continue;
                }

                var rga = new Return(returnNumber, originalOrder);
                returns.Add(rga);
            }

            return returns;
        }
    }
}
