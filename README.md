# Inventory API (Redarbor Technical Test)

## Overview
Inventory management API built with .NET 6, following a clean layered architecture and CQRS split:
- Reads: Entity Framework Core
- Writes: Dapper (transactional)

The solution is fully containerized with Docker Compose:
- SQL Server
- Inventory API

## Architecture
- `Inventory.Api`: HTTP layer (controllers, auth, swagger)
- `Inventory.Application`: use cases, DTOs, repository contracts
- `Inventory.Infrastructure`: EF DbContext (reads), Dapper repositories (writes), DI composition
- `Inventory.Domain`: domain layer (reserved for business rules/entities)

## Tech Stack
- .NET 6
- EF Core 6 (reads)
- Dapper (writes)
- SQL Server (Docker)
- JWT Bearer authentication
- Swagger (OpenAPI)

## Run with Docker
```bash

## Environment setup

1. Copy .env.example to .env
2. Set your own secure values
3. Run:

docker compose up -d --build
# luego ejecutar init.sql (una vez)