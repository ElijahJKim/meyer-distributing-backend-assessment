using System;
using System.Collections.Generic;
using InterviewTest.Database;
using Microsoft.Data.Sqlite;

namespace InterviewTest.Products
{
    public class ProductRepository
    {
        public IProduct GetByProductNumber(string productNumber)
        {
            var connectionString = $"Data Source={DatabasePaths.GetDatabasePath()}";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT ProductNumber, SellingPrice FROM Products WHERE ProductNumber = @productNumber";
            command.Parameters.AddWithValue("@productNumber", productNumber);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                throw new InvalidOperationException($"Product not found: {productNumber}");
            }

            return new DbProduct(reader.GetString(0), reader.GetFloat(1));
        }

        public List<IProduct> GetAll()
        {
            var products = new List<IProduct>();
            var connectionString = $"Data Source={DatabasePaths.GetDatabasePath()}";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT ProductNumber, SellingPrice FROM Products ORDER BY ProductNumber";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new DbProduct(reader.GetString(0), reader.GetFloat(1)));
            }

            return products;
        }
    }
}
