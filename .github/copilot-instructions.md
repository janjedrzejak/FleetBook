**Repository Overview**
- **Type:** ASP.NET Core solution with two main projects: `FleetBook` (Blazor server components frontend) and `FleetBook.API` (REST API).
- **Runtime / DB:** API uses SQLite (`Data Source=fleetbook.db`) and seeds data on startup (`FleetBook.API/Data/DbInitializer.cs`).
- **Auth:** JWT-based authentication. Tokens issued and validated by `FleetBook.API/Services/JwtTokenService.cs` and stored client-side using `Blazored.LocalStorage` in `FleetBook/Services/AuthService.cs`.

**How To Run (developer flow)**
- Run the API (serves REST endpoints):
  - Open PowerShell in `FleetBook.API` and run:
    `dotnet run --launch-profile http`
  - The API binds to `http://localhost:5056` (see `FleetBook.API/Properties/launchSettings.json`).
- Run the frontend (Blazor server components):
  - Open PowerShell in `FleetBook` and run:
    `dotnet run --launch-profile http`
  - The frontend runs on `http://localhost:5132` (see `FleetBook/Properties/launchSettings.json`).

**Important Runtime Details**
- The API enables CORS for the frontend origin `http://localhost:5132` in `FleetBook.API/Program.cs` (policy `AllowFleetBookFrontend`).
- The frontend's `HttpClient` points to the API base address: `FleetBook/Program.cs` uses `BaseAddress = new Uri("http://localhost:5056")`.
- Database is created via `DbInitializer.Initialize()` which calls `context.Database.EnsureCreated()` and seeds:
  - Admin: `admin@fleetbook.com` / `Admin123!`
  - User: `user@fleetbook.com` / `User123!`
  - Sample cars are added. See `FleetBook.API/Data/DbInitializer.cs` for exact data.

**Auth & Token Handling (practical tips)**
- Access token generation: `FleetBook.API/Services/JwtTokenService.cs` — tokens contain user claims and roles.
- Login flow: `FleetBook.API/Services/AuthService.cs` returns an `AuthResponse` containing `AccessToken` and `RefreshToken` stored in the DB (`RefreshTokens` table).
- Client-side storage: `FleetBook/Services/AuthService.cs` writes to `Blazored.LocalStorage` keys `accessToken` and `refreshToken`. Use `AuthService.GetAccessTokenAsync()` to obtain token for outgoing requests.
- Example of using token in API calls: `FleetBook/Services/CarApiService.cs` sets `Authorization: Bearer <token>` on `HttpClient` before requests.

**Key Files to Inspect for Changes**
- `FleetBook.API/Program.cs` — DI, JWT setup, CORS, DB seeding.
- `FleetBook.API/Controllers/*` — API surface (e.g., `CarsController.cs`, `AuthController.cs`).
- `FleetBook.API/Services/*` — business logic and token generation (`JwtTokenService`, `AuthService`, `CarService`).
- `FleetBook.API/Data/ApplicationDbContext.cs` — EF model mapping and `DbSet`s.
- `FleetBook/Program.cs` — Blazor startup, `HttpClient` base address, auth provider registrations.
- `FleetBook/Services/*` — client-side API wrappers, token caching and storage (`AuthService`, `CarApiService`).

**Project Conventions & Patterns**
- Token lifecycle: backend issues short-lived access tokens + refresh tokens persisted in DB (`RefreshToken` entity). Refresh endpoints live in `FleetBook.API/Controllers`.
- Passwords use BCrypt with `EnhancedHashPassword(..., 13)` (see `FleetBook.API/Services/AuthService.cs`).
- Logging & diagnostics: controllers and services use `ILogger` and many console/logging statements with emoji markers (useful when searching logs).
- Polish-language inline comments and messages are present — search for `// 🔹` markers to find clarified intentions in code.

**Common Tasks for an AI Agent**
- Adding an API endpoint: update `FleetBook.API/Controllers/*`, add service method in `FleetBook.API/Services/*`, and register DI if new.
- Adding a UI page/component: add Razor file under `FleetBook/Components/Pages` and call `CarApiService` or `AuthService` where needed.
- Changing DB model: update `FleetBook.Models` and `ApplicationDbContext`, then update `DbInitializer` and migrations (note: the project uses `EnsureCreated()` and seed data — prefer careful migration strategy).

**Build / Debug Hints**
- Use the explicit launch profiles to reproduce local ports: `--launch-profile http` runs the project on the port defined in `launchSettings.json`.
- The persistent DB file `fleetbook.db` is created next to the API project's working directory; delete it to reset seeded state.

**When You See Issues**
- If tokens are rejected: confirm `JwtSettings` in `FleetBook.API/appsettings.json` and matching `JwtSettings` in frontend `appsettings.json` (issuer/audience/secret must match).
- If CORS errors occur: confirm the frontend origin `http://localhost:5132` is allowed in `FleetBook.API/Program.cs`.

**Questions For The Maintainer**
- Do you prefer Agents to open pull requests for code changes or create drafts only?
- Any CI or secret-management steps (e.g., production JWT key rotation) that should be respected?

Please review — I can iterate on tone, add commit/PR guidance, or merge this into an existing file if you prefer.
