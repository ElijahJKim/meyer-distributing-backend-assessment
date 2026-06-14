using System;
using InterviewTest.Customers;
using InterviewTest.Database;
using Microsoft.Data.Sqlite;

namespace InterviewTest.Reports
{
    public static class CustomerActivityReport
    {
        public static void Print(ICustomer customer)
        {
            Console.WriteLine($"--- Activity Report: {customer.GetName()} ---");

            var connectionString = $"Data Source={DatabasePaths.GetDatabasePath()}";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT o.OrderNumber, op.ProductNumber, op.SellingPrice, op.PurchasedAt, " +
                "CASE WHEN rp.Id IS NOT NULL THEN 1 ELSE 0 END AS IsReturned, rp.ReturnedAt " +
                "FROM OrderProducts op " +
                "INNER JOIN Orders o ON op.OrderId = o.Id " +
                "INNER JOIN Customers c ON o.CustomerId = c.Id " +
                "LEFT JOIN ReturnProducts rp ON rp.OrderProductId = op.Id " +
                "WHERE c.Name = @customerName " +
                "ORDER BY op.PurchasedAt";
            command.Parameters.AddWithValue("@customerName", customer.GetName());

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var orderNumber = reader.GetString(0);
                var productNumber = reader.GetString(1);
                var sellingPrice = reader.GetFloat(2);
                var purchasedAt = DateTime.Parse(reader.GetString(3));
                var isReturned = reader.GetInt64(4) == 1;
                var returnedAt = reader.IsDBNull(5) ? (DateTime?)null : DateTime.Parse(reader.GetString(5));

                var status = isReturned
                    ? $"Returned at {returnedAt:u}"
                    : "Purchased";

                Console.WriteLine(
                    $"{orderNumber} | {productNumber} | {sellingPrice:c} | {status} | Purchased {purchasedAt:u}");
            }

            Console.WriteLine();
        }
    }
}
