# Clean Architecture Template

A .NET 10 Clean Architecture solution featuring Domain-Driven Design, CQRS, and DAC (Discretionary Access Control) authorization.

## Features

- **SharedKernel** - Common DDD abstractions and utilities
- **Domain Layer** - Entities, value objects, domain events
- **Application Layer** - CQRS pattern with:
  - Command/Query handlers
  - Repository abstractions
  - Authorization service
  - FluentValidation
- **Infrastructure Layer** - Cross-cutting concerns:
  - Entity Framework Core with PostgreSQL
  - JWT Authentication
  - DAC Permission-based Authorization
  - Serilog structured logging
- **Web.Api Layer** - REST API with Controllers
- **Testing** - Architecture tests and unit test support

## Architecture Rules

This project follows strict architecture principles enforced via ArchUnitNET tests:

- Domain layer has no external dependencies
- Application layer depends only on Domain and SharedKernel
- Infrastructure layer depends on Application
- Web.Api layer depends on Infrastructure

## Coding Standards

- C# 12 primary constructors for dependency injection
- Async/await for all I/O operations
- Record types for immutable data structures
- Explicit typing (use `var` only when evident from context)
- Internal sealed classes by default
- GUID identifiers for all entities
- Use `is null` / `is not null` checks instead of `== null` / `!= null`

## Project Structure

```
src/
├── Domain/           # Core business logic, entities, value objects
├── Application/      # Use cases, CQRS, repository interfaces
├── Infrastructure/   # EF Core, authentication, external services
├── SharedKernel/    # Shared abstractions (Result, Error, etc.)
└── Web.Api/         # Controllers, middleware, API configuration
```

## Getting Started

### Prerequisites

- .NET 10 SDK
- Docker & Docker Compose
- PostgreSQL 17+ (or use Docker)

### Configuration

Update `appsettings.json` with your configuration:

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Database=clean-architecture;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Secret": "your-secret-key-min-32-characters-long",
    "Issuer": "CleanArchitecture",
    "Audience": "CleanArchitecture"
  }
}
```

### Running with Docker

```bash
docker-compose up -d
```

Services:
- **Web.Api**: http://localhost:8080
- **Swagger**: http://localhost:8080/swagger
- **PostgreSQL**: localhost:5432
- **Seq (Logs)**: http://localhost:8081

### Running Locally

```bash
dotnet restore
dotnet build
dotnet run --project src/Web.Api
```

### Running Tests

```bash
dotnet test
```

## API Endpoints

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/users/register` | Register new user |
| POST | `/users/login` | Login and get JWT token |

### Users

| Method | Endpoint | Permission | Description |
|--------|----------|------------|-------------|
| GET | `/users/{id}` | `users:read` | Get user by ID |
| POST | `/users/{id}/roles` | `users:assign` | Assign role to user |

### Roles (Admin)

| Method | Endpoint | Permission | Description |
|--------|----------|------------|-------------|
| POST | `/roles` | `roles:create` | Create new role |
| GET | `/roles` | `roles:read` | List all roles |
| GET | `/roles/{id}` | `roles:read` | Get role by ID |
| PUT | `/roles/{id}` | `roles:update` | Update role |
| DELETE | `/roles/{id}` | `roles:delete` | Delete role |

## Permission Format

This project uses DAC (Discretionary Access Control) with `resource:action` permission format:

- `roles:create` - Can create roles
- `roles:read` - Can view roles
- `roles:update` - Can modify roles
- `roles:delete` - Can delete roles
- `users:read` - Can view users
- `users:assign` - Can assign roles to users

## Authorization Flow

1. User registers or logs in
2. User is assigned roles (by admin)
3. Roles have permissions (e.g., `roles:create`, `users:read`)
4. Endpoints require specific permissions via `[Authorize(Policy = "permission:name")]`
5. `PermissionAuthorizationHandler` checks if user has required permission

## Database Migrations

```bash
# Add migration
dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/Web.Api

# Apply migrations
dotnet ef database update --project src/Infrastructure --startup-project src/Web.Api
```

## Learn More

For a comprehensive course on Clean Architecture, check out [**Pragmatic Clean Architecture**](https://www.milanjovanovic.tech/pragmatic-clean-architecture?utm_source=ca-template):

- Domain-Driven Design
- Role-based authorization
- Permission-based authorization
- Distributed caching with Redis
- OpenTelemetry
- Outbox pattern
- API Versioning
- Unit testing
- Functional testing
- Integration testing

---

Stay awesome!
