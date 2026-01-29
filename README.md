# Inventory API (Redarbor Technical Test)

## Overview

Inventory Management API built with **.NET 6**, developed as a **technical assessment for a Senior / Lead .NET Developer position**.

The project follows **Clean Architecture principles** and implements a **real CQRS approach**:

- **Read operations**: Entity Framework Core  
- **Write operations**: Dapper (transactional)

The solution is fully containerized using **Docker Compose**, providing a reproducible local development environment.

---

## Architecture

The solution is structured following **Clean Architecture** guidelines:

```
src/
 ├── Inventory.Api
 │   └── HTTP layer (Controllers, Authentication, Swagger, Middleware)
 │
 ├── Inventory.Application
 │   └── Application layer (CQRS Commands & Queries, Handlers, DTOs, Validators)
 │
 ├── Inventory.Domain
 │   └── Domain layer (Entities and business rules)
 │
 └── Inventory.Infrastructure
     └── Data access (EF Core for reads, Dapper for writes, DI composition)
```

### Architectural Highlights

- **CQRS with MediatR**  
  Commands and Queries are explicitly separated.  
  MediatR is used to orchestrate application use cases.

- **Domain-driven approach**  
  Business rules live in the Domain layer.  
  The Application layer coordinates behavior, not business logic.

- **Persistence strategy**  
  EF Core is used for query operations.  
  Dapper is used for command operations to keep SQL explicit and transactional.

- **Validation**  
  FluentValidation integrated through MediatR pipeline behaviors.

- **Error handling**  
  Centralized global exception handling middleware.

- **Security**  
  JWT Bearer authentication (OAuth2-ready design).

---

## Tech Stack

- .NET 6  
- ASP.NET Core Web API  
- Entity Framework Core 6 (read side)  
- Dapper (write side)  
- SQL Server 2022 (Docker)  
- MediatR  
- FluentValidation  
- JWT Bearer Authentication  
- Swagger / OpenAPI  
- Docker & Docker Compose  
- xUnit + Moq (testing)

---

## Authentication

The API is protected using **JWT Bearer authentication**.

Tokens must be sent using the `Authorization` header:

```
Authorization: Bearer <access_token>
```

Swagger UI supports authentication via the **Authorize** button.

> The authentication setup is designed to be compatible with OAuth2 / OpenID Connect flows.

---

## API Features

### Categories
- Create category
- Update category
- Delete category
- Get paginated list of categories

### Products
- Get products

### Inventory Movements
- Register inventory **IN / OUT** movements
- Business rules enforced at domain level
- Stock validation before applying movements

---

## Pagination

To avoid performance issues, list endpoints support pagination.

Example:

```
GET /api/categories?pageNumber=1&pageSize=20
```

Default and maximum page sizes are enforced at application level.

---

## Error Handling

The API includes **centralized global error handling** with consistent responses:

| HTTP Code | Description |
|----------|------------|
| 400 | Validation or domain errors |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Resource not found |
| 500 | Unexpected server error |

All errors return structured JSON responses.

---

## Swagger Documentation

Swagger (OpenAPI) is enabled and includes:

- Endpoint documentation  
- Request and response schemas  
- Security definitions for JWT authentication  

Swagger UI:

```
http://localhost:5000/swagger
```

---

## Testing Strategy

The project includes automated tests to validate behavior and architecture.

### Test Types

- **Unit Tests**
  - CQRS Handlers
  - Domain business rules
  - Validators

- **Integration Tests**
  - API endpoints using `WebApplicationFactory`

- **Architecture Tests**
  - Enforce Clean Architecture dependency rules

### Tools

- xUnit  
- Moq  
- FluentAssertions  

> Tests focus on validating **business behavior**, not implementation details.

---

## Run with Docker

### Prerequisites

- Docker  
- Docker Compose  

### Environment Setup

1. Copy the example environment file:
   ```
   cp .env.example .env
   ```

2. Configure secure values in `.env`

3. Start the environment:
   ```
   docker compose up -d --build
   ```

4. **First run only** – execute the database initialization script:
   ```
   docker cp db/init.sql inventory-sql:/init.sql
   docker exec -it inventory-sql /opt/mssql-tools18/bin/sqlcmd \
     -S localhost \
     -U sa \
     -P <YOUR_PASSWORD> \
     -i /init.sql \
     -C
   ```

---

## Database

- **SQL Server 2022**
- Database name: `InventoryDb`

Core tables:
- Categories
- Products
- InventoryMovements

---

## Design Principles Applied

- Clean Architecture  
- CQRS  
- SOLID principles  
- Separation of concerns  
- Dependency Inversion  
- Explicit domain logic  
- Testability and maintainability  

---

## Author

**Tony Adeciga**  
Senior / Lead .NET Developer

---

## Notes for Reviewers

- The project prioritizes **clarity and correctness over over-engineering**
- Some infrastructure aspects are intentionally simplified for a technical assessment
- Architectural decisions reflect real-world enterprise backend practices
