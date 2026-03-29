# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

### Build & Run
```bash
# Build the solution
dotnet build tallow-server.slnx

# Run the API (from root)
dotnet run --project API.Public

# Run with specific environment
dotnet run --project API.Public --environment Development
```

### Database Migrations (run from repo root)
```bash
# Add a new migration
dotnet ef migrations add <MigrationName> --project Repository --startup-project API.Public

# Apply migrations
dotnet ef database update --project Repository --startup-project API.Public

# Revert last migration
dotnet ef migrations remove --project Repository --startup-project API.Public
```

### Tests
```bash
dotnet test
```

## Architecture

This is a **4-project clean architecture ASP.NET Core 10 e-commerce API** (Tallow):

```
API.Public/        → HTTP layer: controllers, DTOs, middleware config, startup
Domain/            → Business logic: services, interfaces, entities, enumerators
Repository/        → Data access: EF Core DbContext, repository implementations, migrations
IoC/               → Dependency injection wiring + Serilog logger setup
```

### Request Flow

`Controller` → `IService` (Domain) → `IRepository` (Domain interface) → `Repository` implementation → `AppDbContext` (SQL Server)

### Key Patterns

**Base classes to extend:**
- `_BaseController` (`API.Public/Controllers/_Base/`) — provides `GetSecurityInfo()`, `GenerateAuthCookie()`, `Authenticated` (thread principal), and `RemoveAuthCookie()`
- `BaseRepository<E>` (`Repository/Repository/_Base/`) — generic CRUD with soft delete, audit fields, pagination (`PaginateAsync`), and expression-based queries

**Entities:** All inherit from `BaseEntity` which provides `Id` (GUID string), `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`, `DeletedAt` (nullable — used for soft delete). Always filter `DeletedAt == null` for active records.

**Soft vs hard delete:** `DeleteAsync()` sets `DeletedAt`; `HardDeleteAsync()` removes the row. Prefer soft delete.

**Services pattern:** Domain defines `IXxxService` interface + `XxxService` abstract base; concrete implementations live under `Domain/Services/`. IoC wires `IXxxService → XxxService` as scoped.

**Authentication:** JWT Bearer tokens delivered as `HttpOnly` cookies (`AccessToken` + `RefreshToken`). `Thread.CurrentPrincipal` is cast to `IdentityPrincipal` — access via `_BaseController.Authenticated` static property.

**Configuration:** All app settings flow through `Domain/Utils/Constants/Settings.cs` and are accessed statically via `Constant.Settings`. In Production, secrets are loaded from an external source via `ConfigureEnvironmentAsync()`.

### External Integrations

- **Mercado Pago** (`mercadopago-sdk`) — payment processing via `IMercadoPagoService`
- **SuperFrete** — shipping rate calculation via named `HttpClient`
- **Scalar** — API documentation UI (replaces Swagger UI), available at `/scalar` in dev

### Database

- **SQL Server** (LocalDB for dev: `TALLOW_DEV`)
- DbContext pool size: 128 (configurable via `Settings.MaxPoolConnections`)
- Migrations live in `Repository/Migrations/`
- EF configurations applied via `ApplyConfigurationsFromAssembly()` — add `IEntityTypeConfiguration<T>` classes in `Repository/` for new entity mappings

### CORS & Cookies

CORS is configured for `localhost:5173` and `localhost:5174` (Vite frontend). Cookies require `SameSite=None; Secure=true` — this means HTTPS is required for auth cookies in any non-localhost scenario.
