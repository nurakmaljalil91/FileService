# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run API locally (uses in-memory DB by default in Development)
dotnet run --project src/WebAPI

# Run all tests
dotnet test

# Run tests for a specific project
dotnet test tests/Domain.UnitTests
dotnet test tests/Application.UnitTests
dotnet test tests/Infrastructure.UnitTests
dotnet test tests/WebAPI.UnitTests
dotnet test tests/IntegrationTests

# Run a single test by name
dotnet test --filter "FullyQualifiedName~CreateTodoListCommandHandlerTests.Handle_ValidCommand_ReturnsSuccess"

# Docker (development)
docker compose up --build

# Docker (production)
docker compose -f docker-compose.yml -f docker-compose.prod.yml up --build -d
```

## Architecture

This is a Clean Architecture ASP.NET Core (.NET 10) Web API with four layers:

- **Domain** (`src/Domain`) — entities, value objects, enums, domain exceptions. No dependencies on other layers.
- **Application** (`src/Application`) — use cases (commands/queries), interfaces, pipeline behaviors, FluentValidation validators. Depends only on Domain.
- **Infrastructure** (`src/Infrastructure`) — EF Core + PostgreSQL, interceptors, services. Implements Application interfaces.
- **WebAPI** (`src/WebAPI`) — controllers, middlewares, DI wiring, JWT auth config.
- **lib/Mediator** — hand-rolled mediator (not MediatR). Resolves `IRequestHandler<,>` and `IPipelineBehavior<,>` from DI via reflection.

### Request pipeline

Each controller method calls `_mediator.Send(command/query)`. The mediator composes a pipeline of registered `IPipelineBehavior<,>` implementations (in order: AuthorizationBehaviour → LoggingBehaviour → PerformanceBehaviour → UnhandledExceptionBehaviour → ValidationBehaviour) before invoking the handler.

Commands and their validators live in the same file (e.g., `CreateTodoListCommand.cs` contains the command, handler, and `CreateTodoListCommandValidator`).

### Response envelope

All API endpoints return `BaseResponse<T>` (`src/Domain/Common/BaseResponse.cs`). Use the static factories: `BaseResponse<T>.Ok(data, message)` and `BaseResponse<T>.Fail(message, errors)`.

### Database

- PostgreSQL via `Npgsql.EntityFrameworkCore.PostgreSQL` with **snake_case** column naming (`EFCore.NamingConventions`).
- NodaTime `Instant` is used for audit timestamps (`CreatedDate`, `UpdatedDate`) on `BaseAuditableEntity<TId>`.
- EF configurations are in `src/Infrastructure/Data/Configurations/`.
- `UseInMemoryDatabase: true` in `appsettings.Development.json` skips PostgreSQL for local dev.

### Authentication

JWT Bearer auth is configured in `WebAPI/DependencyInjection.cs`. The default fallback policy requires an authenticated user. `[AllowAnonymous]` opts out per-controller or per-action.

`AuthController` (`POST /api/Auth/login`) issues tokens for development/demo — it accepts any username/email without validating credentials. **Replace this with a real auth flow before production.**

### Exception handling

`ExceptionHandlingMiddleware` catches application exceptions and maps them to HTTP status codes:
- `ValidationException` → 400
- `NotFoundException` → 404
- `UnauthorizedAccessException` → 401
- `ForbiddenAccessException` → 403
- `BadRequestException` → 400

### Package management

All NuGet versions are centrally managed in `Directory.Packages.props`. Do not specify `Version=` in individual `.csproj` files.

`Directory.Build.props` applies solution-wide settings (target framework `net10.0`, nullable enabled, `SonarAnalyzer.CSharp` on all projects). Release builds treat warnings as errors for non-test projects.

### Integration tests

`tests/IntegrationTests/ApiFactory.cs` extends `WebApplicationFactory<Program>` and forces `UseInMemoryDatabase=true` with a hardcoded JWT config. Tests inherit `ApiTestBase`, which provides `CreateClient()` and `CreateAuthenticatedClientAsync()`. The `IntegrationTestCollection` class pins all integration tests to a shared factory instance (xUnit collection fixture).

### Health checks

- `GET /health/live` — always returns Healthy if the process is running (no checks run).
- `GET /health/ready` — runs all registered health checks (Postgres connectivity when not using in-memory DB).

### Logging

Serilog reads config from `appsettings.json`. `CorrelationIdMiddleware` injects a `CorrelationId` into every log event. Logs go to Console and rolling file (`/logs/log-.txt`).
