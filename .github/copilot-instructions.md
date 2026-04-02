# GitHub Copilot Instructions for LifelogBB

## Project Overview

LifelogBB is a self-hosted, single-user life tracking application built with **ASP.NET Core** and **SQLite**. It provides tracking for weight, journal, strength/endurance training, todos, goals, habits, bucket list, quotes, and more. All data is stored in a single SQLite file for portability.

## Technology Stack

- **Framework:** ASP.NET Core (MVC + Web API)
- **Database:** SQLite via Entity Framework Core
- **ORM Mapping:** AutoMapper
- **Authentication:** Cookie (web UI) + JWT Bearer (API), unified via a policy scheme
- **UI:** Razor Views with [Tabler](https://tabler.io/) frontend
- **API Docs:** Swagger / OpenAPI (Swashbuckle)

## Project Structure

```
LifelogBb/
├── ApiControllers/        # RESTful API controllers (extend BaseCRUDController)
├── ApiDTOs/               # Input/Output DTOs per entity, plus AutoMapperProfile
├── ApiRepositories/       # Generic EntityRepository<TEntity>
├── ApiServices/           # Business logic services (extend BaseCRUDService)
├── Controllers/           # MVC controllers for the web UI
├── Interfaces/            # IRepository, IBaseCRUDService, IBaseRService, DTO contracts
├── Models/
│   ├── Entities/          # Domain entity classes (BaseEntity, BaseEntityTagged, concretes)
│   └── LifelogBbContext   # EF Core DbContext
├── Views/                 # Razor views (.cshtml) for the web UI
├── Migrations/            # EF Core migrations (bundled as efbundle)
├── Utilities/             # Shared helpers
└── wwwroot/               # Static assets
```

---

## Core Patterns

### 1. Entity Hierarchy

All entities follow a simple, flat inheritance chain. Do **not** add deep hierarchies.

```
BaseEntity                   (Id, CreatedAt, UpdatedAt)
  └── BaseEntityTagged       (+ Category, Tags)
        └── <ConcreteEntity> (domain-specific properties)
```

- Use `BaseEntity` for entities that do not need categorisation.
- Use `BaseEntityTagged` for entities that benefit from `Category` and `Tags`.
- Keep entities **simple and flat**. Prefer primitive types and nullable primitives over complex value objects.
- Add data annotation attributes (`[Required]`, `[MinLength]`, `[Range]`, etc.) directly on entity properties.

**Example – minimal entity:**

```csharp
public class Weight : BaseEntity
{
    [Required]
    public double Value { get; set; }

    public string? Note { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
}
```

---

### 2. DTOs (Input / Output)

Each entity has two DTOs in `ApiDTOs/<EntityPlural>/`:

| DTO | Purpose |
|-----|---------|
| `<Entity>Input` | Request body for POST / PUT operations |
| `<Entity>Output` | Response body; implements `IBaseOutput` (includes `Id`, `CreatedAt`, `UpdatedAt`) |

Rules:
- Mirror entity properties in both DTOs.
- `<Entity>Output` **always** includes `Id`, `CreatedAt`, and `UpdatedAt`.
- Add the same data annotation attributes as on the entity.
- Register both mappings in `AutoMapperProfile`:
  ```csharp
  CreateMap<EntityInput, Entity>();
  CreateMap<Entity, EntityOutput>();
  ```

---

### 3. Generic API Layer (Controller → Service → Repository)

The API uses a **fully generic CRUD stack**. A new entity type typically requires only ~10 lines of new code.

```
HTTP Request
  → ApiController  (extends BaseCRUDController<TService, TInput, TOutput>)
  → Service        (extends BaseCRUDService<TEntity, TInput, TOutput>)
  → Repository     (IRepository<TEntity> / EntityRepository<TEntity>)
  → DbContext
```

#### Controller

Extend `BaseCRUDController` and set the route. No additional code is needed for standard CRUD.

```csharp
[Route("api/todos")]
[ApiController]
public class TodosApiController : BaseCRUDController<TodosService, TodoInput, TodoOutput>
{
    public TodosApiController(TodosService service) : base(service) { }
}
```

- All endpoints (`GET /`, `GET /{id}`, `POST /`, `PUT /{id}`, `DELETE /{id}`) are provided by the base class.
- All endpoints require authentication (`[Authorize]` is applied in the base class).
- Only add extra action methods in a concrete controller when the entity truly needs non-standard behaviour.

#### Service

Extend `BaseCRUDService`. No additional code is needed for standard CRUD.

```csharp
public class TodosService : BaseCRUDService<Todo, TodoInput, TodoOutput>
{
    public TodosService(IRepository<Todo> repository, IMapper mapper)
        : base(repository, mapper) { }
}
```

- `BaseCRUDService` uses AutoMapper and the generic repository for all operations.
- Override only when business logic beyond simple mapping is required.

#### Repository

`EntityRepository<TEntity>` is registered as an open generic in `Program.cs` and covers all entities automatically. Do **not** create entity-specific repository classes unless absolutely necessary.

```csharp
services.AddScoped(typeof(IRepository<>), typeof(EntityRepository<>));
```

---

### 4. Registering a New Entity

When adding a new entity, follow these steps in order:

1. Create `Models/Entities/<Entity>.cs` extending `BaseEntity` or `BaseEntityTagged`.
2. Add a `DbSet<Entity>` to `LifelogBbContext`.
3. Create `ApiDTOs/<EntityPlural>/<Entity>Input.cs` and `<Entity>Output.cs`.
4. Add `CreateMap` entries to `AutoMapperProfile`.
5. Create `ApiServices/<EntityPlural>Service.cs` extending `BaseCRUDService`.
6. Create `ApiControllers/<EntityPlural>ApiController.cs` extending `BaseCRUDController`.
7. Register the service in `Program.cs`: `services.AddScoped<EntityPlural>Service>();`
8. Generate and apply a new EF Core migration.

---

### 5. MVC Web UI

- MVC controllers live in `Controllers/` and inherit from `Controller` (not `ControllerBase`).
- Views live in `Views/<ControllerName>/` as `.cshtml` Razor files.
- View models live in `Models/<FeatureArea>/` and are separate from API DTOs.
- Use the existing Tabler UI components to keep the look-and-feel consistent.

---

### 6. Authentication

- The web UI uses **cookie authentication**.
- The API uses **JWT Bearer token** authentication.
- A combined policy scheme `"JWT_OR_COOKIE"` selects the correct scheme automatically.
- All controllers are protected by default via a global `[Authorize]` filter. Do not remove this.

---

## Code Style Guidelines

- Use **C#** / **.NET** features where appropriate (nullable reference types are enabled).
- Follow the existing naming conventions:
  - Entities: `PascalCase` singular (e.g., `StrengthTraining`)
  - Controllers / Services: `PascalCase` plural + suffix (e.g., `StrengthTrainingsApiController`, `StrengthTrainingsService`)
  - DTOs: `<Entity>Input` / `<Entity>Output`
- Keep classes **small and focused**. Avoid adding unrelated logic to a service or controller.
- Prefer **data annotations** on DTOs and entities for validation rather than manual checks.
- Do **not** bypass the generic base classes for standard CRUD. Extend them instead.
- Do **not** add new NuGet packages without a clear justification.
