# Clean Architecture Template

A production-ready .NET 10 Clean Architecture solution featuring Domain-Driven Design, CQRS, DAC (Discretionary Access Control) authorization, and comprehensive testing infrastructure.

## Features

### Architecture
- **Clean Architecture** - Strict layer separation: Domain → Application → Infrastructure → Web.Api
- **CQRS Pattern** - Command/Query handlers with decorator pattern for validation and logging
- **Result Pattern** - Typed error handling with `Result<T>` and `Error` types
- **Domain Events** - Event dispatcher for eventual consistency
- **Permission-based Authorization** - DAC with `resource:action` permission format

### Core Layers
- **SharedKernel** - Common DDD abstractions (Result, Error, Entity base classes)
- **Domain Layer** - Entities, value objects, domain events, no external dependencies
- **Application Layer** - CQRS, repository interfaces, FluentValidation, configuration models
- **Infrastructure Layer** - EF Core, JWT auth, repositories, external services
- **Web.Api Layer** - REST API with Controllers, middleware, API configuration

### API Features
- **API Versioning** - `[ApiVersion("1.0")]` with URL and header support
- **Pagination** - `IPagedQuery<T>` and `PagedResult<T>` for list endpoints
- **Rate Limiting** - Fixed window limiter (100 req/min, queue of 10)
- **CORS** - Configured for cross-origin requests
- **Health Checks** - `/health` endpoint with database connectivity check
- **Swagger** - API documentation at `/swagger`

### Security
- **JWT Authentication** - Bearer token with configurable expiration
- **DAC Authorization** - Permission-based policies enforced via `IAuthorizationHandler`
- **Password Hashing** - BCrypt via `IPasswordHasher`

### Developer Experience
- **Centralized Package Management** - `Directory.Packages.props` for consistent versions
- **Global Using Directives** - Reduced boilerplate in Application layer
- **Serilog Structured Logging** - With Seq integration for log aggregation
- **Strongly-Typed Configuration** - `IOptions<T>` pattern for JwtSettings, DatabaseSettings

### Testing
- **Architecture Tests** - Layer dependency enforcement via ArchUnitNET
- **Unit Tests** - xUnit + FluentAssertions + Moq
- **Integration Tests** - WebApplicationFactory with in-memory database support

## Architecture Rules

This project follows strict architecture principles enforced via ArchUnitNET tests:

- Domain layer has **no external dependencies**
- Application layer depends only on **Domain and SharedKernel**
- Infrastructure layer depends on **Application**
- Web.Api layer depends on **Infrastructure**

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
├── Application/
│   ├── Configuration/     # JwtSettings, DatabaseSettings
│   ├── Abstractions/       # Interfaces (repositories, messaging, auth)
│   └── ...                 # Commands, Queries, Validators
├── Domain/                 # Entities, Value Objects, Events
├── Infrastructure/
│   ├── Authentication/     # JWT, PasswordHasher, TokenProvider
│   ├── Authorization/      # Permission-based auth handlers
│   ├── Data/               # DevelopmentSeeder
│   └── Database/           # EF Core DbContext, Repositories
├── SharedKernel/          # Result, Error, Entity base classes
└── Web.Api/
    ├── Controllers/        # API endpoints
    ├── Extensions/         # DI extensions
    └── Infrastructure/     # Exception handler, CustomResults
tests/
├── ArchitectureTests/      # Layer dependency tests
├── Application.UnitTests/  # Handler and validator tests
└── Application.IntegrationTests/  # API integration tests
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
    "Secret": "your-secret-key-minimum-32-characters-long-for-security",
    "Issuer": "CleanArchitecture",
    "Audience": "CleanArchitecture",
    "ExpirationInMinutes": 60
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
- **Health Check**: http://localhost:8080/health
- **PostgreSQL**: localhost:5432
- **Seq (Logs)**: http://localhost:8081

### Running Locally

```bash
dotnet restore
dotnet build
dotnet run --project src/Web.Api
```

The application will:
1. Apply database migrations automatically
2. Seed development data (in development mode)

### Default Development Credentials

After seeding:
- **Email**: `admin@example.com`
- **Password**: `Admin123!`

### Running Tests

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test tests/Application.UnitTests

# Run architecture tests only
dotnet test tests/ArchitectureTests
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
| GET | `/roles?page=1&pageSize=10` | `roles:read` | List roles (paginated) |
| GET | `/roles/{id}` | `roles:read` | Get role by ID |
| PUT | `/roles/{id}` | `roles:update` | Update role |
| DELETE | `/roles/{id}` | `roles:delete` | Delete role |

## Permission Format

This project uses DAC (Discretionary Access Control) with `resource:action` permission format:

- `admin:full` - Full administrative access
- `roles:create` - Can create roles
- `roles:read` - Can view roles
- `roles:update` - Can modify roles
- `roles:delete` - Can delete roles
- `users:read` - Can view users
- `users:write` - Can create/update users
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

## Template Usage for New Projects

1. Clone this template
2. Replace `CleanArchitecture` with your project name in solution file
3. Update JWT secret and database connection in `appsettings.json`
4. Delete sample entities (User, Role, Permission) and add your own domain
5. Run `dotnet ef migrations add InitialCreate` to generate initial migration
6. Start building your features!

## License

MIT