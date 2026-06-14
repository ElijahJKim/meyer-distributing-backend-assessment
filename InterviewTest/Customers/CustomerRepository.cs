using System;
using System.Collections.Generic;
using InterviewTest.Database;
using InterviewTest.Orders;
using InterviewTest.Returns;
using Microsoft.Data.Sqlite;

namespace InterviewTest.Customers
{
  public class CustomerRepository
  {
    private readonly OrderRepository _orderRepository;
    private readonly ReturnRepository _returnRepository;

    public CustomerRepository(OrderRepository orderRepository, ReturnRepository returnRepository)
    {
      _orderRepository = orderRepository;
      _returnRepository = returnRepository;
    }

    public ICustomer GetByName(string name)
    {
      if (!Exists(name))
      {
        throw new InvalidOperationException($"Customer not found: {name}");
      }

      return new DbCustomer(name, _orderRepository, _returnRepository, this);
    }

    public List<ICustomer> GetAll()
    {
      var customers = new List<ICustomer>();
      var connectionString = $"Data Source={DatabasePaths.GetDatabasePath()}";
      using var connection = new SqliteConnection(connectionString);
      connection.Open();

      using var command = connection.CreateCommand();
      command.CommandText = "SELECT Name FROM Customers ORDER BY Name";

      using var reader = command.ExecuteReader();
      while (reader.Read())
      {
        customers.Add(new DbCustomer(reader.GetString(0), _orderRepository, _returnRepository, this));
      }

      return customers;
    }

    public bool Exists(string name)
    {
      var connectionString = $"Data Source={DatabasePaths.GetDatabasePath()}";
      using var connection = new SqliteConnection(connectionString);
      connection.Open();

      using var command = connection.CreateCommand();
      command.CommandText = "SELECT COUNT(1) FROM Customers WHERE Name = @name";
      command.Parameters.AddWithValue("@name", name);
      return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    // Bonus: sales / returns / profit aggregation lives in the repository (DB access layer).
    // CustomerBase still exposes these via ICustomer and delegates here.

    public float GetTotalSales(string customerName)
    {
      return QuerySingleFloat(
        customerName,
        "SELECT COALESCE(SUM(op.SellingPrice), 0) " +
        "FROM OrderProducts op " +
        "INNER JOIN Orders o ON op.OrderId = o.Id " +
        "INNER JOIN Customers c ON o.CustomerId = c.Id " +
        "WHERE c.Name = @customerName");
    }

    public float GetTotalReturns(string customerName)
    {
      return QuerySingleFloat(
        customerName,
        "SELECT COALESCE(SUM(op.SellingPrice), 0) " +
        "FROM ReturnProducts rp " +
        "INNER JOIN Returns r ON rp.ReturnId = r.Id " +
        "INNER JOIN OrderProducts op ON rp.OrderProductId = op.Id " +
        "INNER JOIN Orders o ON r.OrderId = o.Id " +
        "INNER JOIN Customers c ON o.CustomerId = c.Id " +
        "WHERE c.Name = @customerName");
    }

    public float GetTotalProfit(string customerName)
    {
      return GetTotalSales(customerName) - GetTotalReturns(customerName);
    }

    private float QuerySingleFloat(string customerName, string sql)
    {
      var connectionString = $"Data Source={DatabasePaths.GetDatabasePath()}";
      using var connection = new SqliteConnection(connectionString);
      connection.Open();

      using var command = connection.CreateCommand();
      command.CommandText = sql;
      command.Parameters.AddWithValue("@customerName", customerName);

      var result = command.ExecuteScalar();
      return Convert.ToSingle(result);
    }
  }
}
