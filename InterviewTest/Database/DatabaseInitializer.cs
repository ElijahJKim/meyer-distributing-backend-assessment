using Microsoft.Data.Sqlite;

namespace InterviewTest.Database
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            var connectionString = $"Data Source={DatabasePaths.GetDatabasePath()}";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText =
                "CREATE TABLE IF NOT EXISTS Customers (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Name TEXT NOT NULL UNIQUE" +
                ")";
            command.ExecuteNonQuery();

            command.CommandText =
                "CREATE TABLE IF NOT EXISTS Products (" +
                "ProductNumber TEXT PRIMARY KEY, " +
                "SellingPrice REAL NOT NULL" +
                ")";
            command.ExecuteNonQuery();

            command.CommandText =
                "CREATE TABLE IF NOT EXISTS Orders (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "OrderNumber TEXT NOT NULL UNIQUE, " +
                "CustomerId INTEGER NOT NULL, " +
                "FOREIGN KEY (CustomerId) REFERENCES Customers(Id)" +
                ")";
            command.ExecuteNonQuery();

            command.CommandText =
                "CREATE TABLE IF NOT EXISTS OrderProducts (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "OrderId INTEGER NOT NULL, " +
                "ProductNumber TEXT NOT NULL, " +
                "SellingPrice REAL NOT NULL, " +
                "PurchasedAt TEXT NOT NULL, " +
                "FOREIGN KEY (OrderId) REFERENCES Orders(Id), " +
                "FOREIGN KEY (ProductNumber) REFERENCES Products(ProductNumber)" +
                ")";
            command.ExecuteNonQuery();

            command.CommandText =
                "CREATE TABLE IF NOT EXISTS Returns (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "ReturnNumber TEXT NOT NULL UNIQUE, " +
                "OrderId INTEGER NOT NULL, " +
                "FOREIGN KEY (OrderId) REFERENCES Orders(Id)" +
                ")";
            command.ExecuteNonQuery();

            command.CommandText =
                "CREATE TABLE IF NOT EXISTS ReturnProducts (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "ReturnId INTEGER NOT NULL, " +
                "OrderProductId INTEGER NOT NULL, " +
                "ReturnedAt TEXT NOT NULL, " +
                "FOREIGN KEY (ReturnId) REFERENCES Returns(Id), " +
                "FOREIGN KEY (OrderProductId) REFERENCES OrderProducts(Id)" +
                ")";
            command.ExecuteNonQuery();

            command.CommandText =
                "INSERT OR IGNORE INTO Customers (Name) VALUES ('Meyer Truck Equipment'), ('Ruxer Ford Lincoln, Inc.')";
            command.ExecuteNonQuery();

            command.CommandText =
                "INSERT OR IGNORE INTO Products (ProductNumber, SellingPrice) VALUES ('DrawTite 5504', 70)";
            command.ExecuteNonQuery();

            command.CommandText =
                "INSERT OR IGNORE INTO Products (ProductNumber, SellingPrice) VALUES ('Rugged Liner F55U15', 150)";
            command.ExecuteNonQuery();

            command.CommandText =
                "INSERT OR IGNORE INTO Products (ProductNumber, SellingPrice) VALUES ('Sherman 036-87-1', 155)";
            command.ExecuteNonQuery();

            command.CommandText =
                "INSERT OR IGNORE INTO Products (ProductNumber, SellingPrice) VALUES ('Mobil 1 5W-30', 25)";
            command.ExecuteNonQuery();

            command.CommandText = "DELETE FROM ReturnProducts";
            command.ExecuteNonQuery();
            command.CommandText = "DELETE FROM Returns";
            command.ExecuteNonQuery();
            command.CommandText = "DELETE FROM OrderProducts";
            command.ExecuteNonQuery();
            command.CommandText = "DELETE FROM Orders";
            command.ExecuteNonQuery();
        }
    }
}
