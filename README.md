# Meyer Distributing Backend Assessment

This repository is the backend application for the Meyer Distributing Backend Assessment

## Tech Stack

- **.NET 5.0** (C#)
- **SQLite** (`Microsoft.Data.Sqlite`, ADO.NET)
- **xUnit** tests

## Getting Started

```bash
# Clone the repository
git clone https://github.com/ElijahJKim/meyer-distributing-backend-assessment.git

# Navigate to the directory
cd meyer-distributing-backend-assessment

# Restore, run, and test
dotnet restore
dotnet run --project InterviewTest
dotnet test
```

Requires [.NET SDK](https://dotnet.microsoft.com/download) 5+. `dotnet run` prints example scenarios and waits for a key press. SQLite DB: `InterviewTest/interview-test.db` (created on first run, gitignored).

## Features

### Core

- Local SQLite DB, repositories for orders, returns, products
- `GetTotalSales`, `GetTotalReturns`, `GetTotalProfit` on `CustomerBase`
- `PurchasedAt` / `ReturnedAt` timestamps; line-level return via `OrderedProduct.Id`

### Additional

- `CustomerRepository` + `DbCustomer`
- `CustomerActivityReport`, `ExchangeService`, loyalty 10% off after $500 cumulative sales
- Unit tests; transactional data reset on each app start

## Architecture

```
InterviewTest/
├── Program.cs
├── Database/          # Schema, seed, DB path
├── Customers/         # CustomerRepository, CustomerBase, DbCustomer
├── Orders/            # OrderRepository, Order, OrderedProduct
├── Returns/           # ReturnRepository, Return, ReturnedProduct
├── Products/          # ProductRepository
├── Services/          # ExchangeService
└── Reports/           # CustomerActivityReport

InterviewTest.Tests/
└── CustomerTotalsTests.cs
```

Tables: `Customers`, `Products`, `Orders`, `OrderProducts`, `Returns`, `ReturnProducts`

## Future improvements

### Use internal `Id` instead of `ProductNumber` for lookups

`Program.cs` loads products by part number (e.g. `productRepo.GetByProductNumber("DrawTite 5504")`). The `Products` table uses `ProductNumber` as the primary key.

In production you would typically:

- Add an integer `Id` as the PK on `Products` and keep `ProductNumber` as a unique business key (SKU).
- Accept `productId` from the client API; the server loads the product and adds it to the order.
- Reference `ProductId` (FK) on `OrderProducts` and snapshot `SellingPrice` at order time.

B2B distribution often orders by SKU externally; internal FKs and APIs usually use integer ids.

### Reference entities by id in the domain layer

Today `Order` holds a full `ICustomer` and `Return` holds a full `IOrder`. That matches the original assignment’s rich object style, but it forces circular repository wiring (`SetOrderRepository`, `SetCustomerRepository`) and heavy `GetAll()` reconstruction.

In production, associations are usually ids only:

```csharp
// Typical production style
class Order {
    long Id;
    long CustomerId;           // not ICustomer
    List<OrderLine> Lines;
}

class Return {
    long Id;
    long OrderId;              // not IOrder
    List<long> OrderProductIds;
}
```

Writes map cleanly to FKs (`customerId` + product lines for orders; `orderId` + `orderProductId` lines for returns). Reads use `GetById` or SQL JOINs when you need names or details—no need to load entire object graphs by default.

### Replace manual repository wiring with DI and id-based queries

`Program.cs` manually connects repositories so `GetAll()` can rebuild nested objects. A real service would register dependencies via DI, expose REST/gRPC endpoints with id-based DTOs, and avoid loading all orders/returns on every read path.
