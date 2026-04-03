# .NET 10.0 Upgrade Plan — LifelogBb

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Migration Plans](#project-by-project-migration-plans)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Risk Management](#risk-management)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario
Upgrade **LifelogBb** from `net7.0` (packages) to `net10.0`, aligning all Microsoft platform packages to their .NET 10 equivalents and updating all third-party dependencies to their latest stable releases compatible with .NET 10.

### Scope

| Item | Value |
|------|-------|
| Projects | 1 (`LifelogBb`) |
| Current framework | `net7.0` (package alignment) → `net10.0` (target) |
| Target framework | `net10.0` |
| NuGet packages | 13 packages, 12 with available updates |
| Total LOC | 15,140 |
| Assessment API issues | ✅ 0 |
| Security vulnerabilities | ✅ None |

### Selected Strategy
**All-At-Once Strategy** — All changes applied simultaneously in a single coordinated operation.

**Rationale**:
- Single project solution — no inter-project dependency ordering required
- 0 blocking API issues identified by assessment
- Clean SDK-style project — straightforward `TargetFramework` property update
- All packages have known target versions
- Small enough surface to validate fully in one pass

### Complexity Classification
**Simple** — 1 project, depth 0, no security vulnerabilities, no API breaking changes detected by static analysis.

> ⚠️ Note: Several packages jump **multiple major versions** (AutoMapper 12→16, Ical.Net 4→5, Swashbuckle 6→10). These may introduce API-level breaking changes that only surface at compile time. The plan accounts for this risk.

### Critical Issues
- None blocking the upgrade.
- **AutoMapper 12→16**: Starting from v13 the `AutoMapper.Extensions.Microsoft.DependencyInjection` package was merged into the main `AutoMapper` package. The separate extension package may become redundant.
- **EF Core Tools**: Latest resolved version returned a preview build (`11.0.0-preview`); the plan targets `10.0.5` (stable, aligned with other EF packages).
- **Microsoft.VisualStudio.Web.CodeGeneration.Design**: Latest resolved version is a preview build; this is a scaffolding tool, not a runtime dependency — see Package Update Reference for guidance.
- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets**: No supported version found; keep current version or remove if Docker support is not in use.

---

## Migration Strategy

### Approach: All-At-Once

All changes are applied as a single coordinated atomic operation — no intermediate states, no multi-targeting.

**Justification:**
- 1 project — no dependency ordering complexity
- No assessed API breaking changes requiring staged preparation
- Good structural clarity (SDK-style, standard MVC layout)
- Single deploy unit with self-contained validation

### Execution Phases

| Phase | Description | Entry Criteria | Exit Criteria |
|-------|-------------|---------------|---------------|
| Phase 0 | Prerequisites | — | .NET 10 SDK installed, branch ready |
| Phase 1 | Atomic Upgrade | Phase 0 complete | Solution builds with 0 errors |
| Phase 2 | Test Validation | Phase 1 complete | All tests pass |

### Ordering Principles
1. Update `TargetFramework` in the project file first
2. Update all `PackageReference` versions in the same operation
3. Restore → build → fix any compilation errors surfaced by package major-version bumps
4. Run tests to validate runtime behaviour

---

## Detailed Dependency Analysis

### Project Graph

```
(no inter-project dependencies)

LifelogBb.csproj   [leaf + root — standalone project]
```

### Migration Order

Since there is exactly one project with no inter-project dependencies, there is only one phase:

| Phase | Projects | Notes |
|-------|----------|-------|
| Phase 1 (Atomic) | `LifelogBb` | Sole project; all changes applied together |

### Critical Path
`LifelogBb` → build → test (no blocking upstream dependencies)

### Circular Dependencies
None.

---

## Project-by-Project Migration Plans

### Project: LifelogBb (`LifelogBb\LifelogBb.csproj`)

**Current State**

| Property | Value |
|----------|-------|
| Target Framework | `net7.0` (package alignment; actual project TFM to verify) |
| Project Kind | ASP.NET Core MVC + Web API |
| SDK Style | ✅ Yes |
| LOC | 15,140 |
| Files | 198 |
| NuGet Packages | 13 |
| Dependencies | None |
| Dependants | None |
| Risk Level | **Medium** (major package version jumps across multiple libraries) |

**Target State**

| Property | Value |
|----------|-------|
| Target Framework | `net10.0` |
| NuGet Packages | 13 (12 updated, 1 no update available) |

**Migration Steps**

1. **Update TargetFramework**
   - In `LifelogBb\LifelogBb.csproj`, change:
     ```xml
     <TargetFramework>net7.0</TargetFramework>
     ```
     to:
     ```xml
     <TargetFramework>net10.0</TargetFramework>
     ```

2. **Update all PackageReference versions** — see [Package Update Reference](#package-update-reference) for the complete version matrix.

3. **Handle AutoMapper major version change (12 → 16)**
   - `AutoMapper.Extensions.Microsoft.DependencyInjection` was merged into the core `AutoMapper` package starting v13. After updating, verify that the separate extension package is still needed. If `AddAutoMapper()` is available directly from the `AutoMapper` package, remove the `AutoMapper.Extensions.Microsoft.DependencyInjection` reference.
   - Check for any AutoMapper profile or mapping configuration API changes introduced between v12 and v16.

4. **Handle Ical.Net major version change (4 → 5)**
   - Review Ical.Net 5.x changelog for API breaking changes. Key areas: calendar parsing, event/recurrence rule APIs, timezone handling.
   - Search codebase for all `Ical.Net` usages and update any changed types or methods.

5. **Handle Swashbuckle.AspNetCore major version change (6 → 10)**
   - Swashbuckle 7+ changed the OpenAPI document setup. Verify `Program.cs` service registration and middleware (`UseSwagger`, `UseSwaggerUI`) is still compatible.
   - If using `SwaggerDoc`, `OperationFilter`, or custom `ISchemaFilter`, verify these APIs have not changed.

6. **Restore dependencies**
   ```
   dotnet restore LifelogBb\LifelogBb.csproj
   ```

7. **Build solution and fix all compilation errors**
   ```
   dotnet build LifelogBb.sln
   ```
   Address any compilation errors surfaced by the package major-version changes. Refer to [Breaking Changes Catalog](#breaking-changes-catalog).

8. **Validate: solution builds with 0 errors**

**Expected Breaking Changes**
See [Breaking Changes Catalog](#breaking-changes-catalog).

**Validation Checklist**
- [ ] `TargetFramework` = `net10.0` in project file
- [ ] All package references at target versions
- [ ] `dotnet restore` completes without errors
- [ ] Solution builds with 0 errors
- [ ] Solution builds with 0 warnings (investigate any new warnings)
- [ ] Application starts successfully
- [ ] Swagger UI loads at `/swagger`
- [ ] Authentication (cookie + JWT) functions correctly
- [ ] EF Core migrations apply successfully

---

## Package Update Reference

All updates apply to the single project `LifelogBb\LifelogBb.csproj`.

### Microsoft Platform Packages (align with .NET 10)

| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | `7.0.4` | `10.0.5` | Align with .NET 10; major API stability improvements |
| `Microsoft.EntityFrameworkCore.Design` | `7.0.4` | `10.0.5` | Align with .NET 10 |
| `Microsoft.EntityFrameworkCore.Sqlite` | `7.0.4` | `10.0.5` | Align with .NET 10 |
| `Microsoft.EntityFrameworkCore.SqlServer` | `7.0.4` | `10.0.5` | Align with .NET 10 |
| `Microsoft.EntityFrameworkCore.Tools` | `7.0.4` | `10.0.5` | Align with .NET 10 (use stable `10.0.5`; tool resolved a preview — avoid preview in production) |

### Third-Party Packages

| Package | Current | Target | Version Change | Notes |
|---------|---------|--------|---------------|-------|
| `AutoMapper` | `12.0.1` | `16.1.1` | Major (4 major versions) | DI extensions absorbed into core package in v13+ — see notes below |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | `12.0.1` | `12.0.1` | None | ⚠️ Verify if still needed after AutoMapper 16 upgrade; may be removable |
| `BCrypt.Net-Next` | `4.0.3` | `4.1.0` | Patch | Low risk; no known breaking changes |
| `Ical.Net` | `4.2.0` | `5.2.1` | Major | ⚠️ Review API changes — see Breaking Changes Catalog |
| `Swashbuckle.AspNetCore` | `6.5.0` | `10.1.7` | Major (4 major versions) | ⚠️ OpenAPI setup changes in v7+; verify Program.cs registration |
| `Westwind.AspNetCore.Markdown` | `3.11.0` | `3.31.0` | Minor | Low risk |

### Tooling / Build Packages

| Package | Current | Target | Notes |
|---------|---------|--------|-------|
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | `7.0.5` | `10.0.x` (stable when available) | Scaffolding tool only; tool resolved a preview build — wait for stable `10.x` release or keep at `7.0.5` if scaffolding is not actively used |
| `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` | `1.18.1` | No update available | Docker tooling support package; keep at current version or remove from project if Docker support is unused |

### AutoMapper v13+ DI Extension Note
Starting from AutoMapper v13, `AddAutoMapper()` is built into the `AutoMapper` package directly. The `AutoMapper.Extensions.Microsoft.DependencyInjection` package is no longer required. After upgrading to `AutoMapper 16.1.1`:
1. Try removing `AutoMapper.Extensions.Microsoft.DependencyInjection` from the project file.
2. Verify `builder.Services.AddAutoMapper(...)` still resolves (it will if using the core package).
3. If compilation fails, temporarily keep the extension package and investigate.

---

## Breaking Changes Catalog

No API breaking changes were identified by the static analyser. The following are **expected breaking changes** based on the major package version jumps and the .NET 7→10 framework transition that may surface during compilation or testing.

### .NET Framework Breaking Changes (7.0 → 10.0)

| Area | Change | Impact | Action |
|------|--------|--------|--------|
| ASP.NET Core | Minimal API and `Program.cs` top-level statements are the standard pattern from .NET 6+ | Low | Verify `Program.cs` structure is already minimal-hosting-model |
| ASP.NET Core | `IHostBuilder` fully replaced by `WebApplication.CreateBuilder` in .NET 7+ | Low | Already resolved if project was on .NET 7 |
| HttpContext / HttpRequest | Some nullable annotation changes | Low | Fix any nullability warnings that become errors |
| Configuration | No structural changes from .NET 7 to 10 | None | — |
| EF Core 7→10 | Several LINQ query translation improvements; some edge cases may produce different SQL | Low | Run full test suite to verify query results |

### EF Core Breaking Changes (7.0 → 10.0)

| Area | Change | Impact | Action |
|------|--------|--------|--------|
| `ExecuteUpdate` / `ExecuteDelete` | Behaviour changes in bulk operations (introduced in EF 7, refined in 8/9/10) | Low | Verify any bulk update queries |
| Owned entities | Mapping conventions updated across EF 8/9/10 | Low | Test entity graph operations |
| Migrations | Existing migrations are compatible; no regeneration required | None | Run `dotnet ef database update` to verify |

### AutoMapper Breaking Changes (12.0 → 16.0)

| Area | Change | Impact | Action |
|------|--------|--------|--------|
| `IMapper` interface | Core interface stable; constructor injection pattern unchanged | Low | No action expected |
| `Profile` API | `CreateMap<>()` API largely stable; verify any `ForMember` / `ResolveUsing` usage | Low | Recheck any advanced mapping configurations |
| DI Extensions | Merged into core package (v13+) | Medium | Remove `AutoMapper.Extensions.Microsoft.DependencyInjection` if compilation succeeds without it |
| `MapperConfiguration` | Some internal validation changes | Low | Check startup-time mapping validation if used |

### Ical.Net Breaking Changes (4.x → 5.x)

| Area | Change | Impact | Action |
|------|--------|--------|--------|
| Namespace restructuring | Some types moved/renamed between v4 and v5 | Medium | Search for all `Ical.Net` usages; check `CalendarCollection`, `CalendarEvent`, `RecurrencePattern` |
| Timezone handling | Improved but potentially behaviour-changing timezone resolution | Medium | Test any calendar/event serialisation or recurrence logic |
| Serialisation | `CalendarSerializer` API may have changed | Medium | Test iCal export/import paths |

### Swashbuckle.AspNetCore Breaking Changes (6.x → 10.x)

| Area | Change | Impact | Action |
|------|--------|--------|--------|
| OpenAPI document setup | `AddSwaggerGen` options and `UseSwagger` middleware configuration changed in v7+ | Medium | Review `Program.cs` Swagger registration; update deprecated options |
| Schema filters | `ISchemaFilter` and `IOperationFilter` signatures may have changed | Medium | Update any custom filters |
| JWT Bearer security definition | Security scheme setup API may have changed | Medium | Verify JWT auth header is still configured correctly in Swagger UI |
| `SwaggerDoc` info | `OpenApiInfo` API stable but verify attribute naming | Low | Quick compile-time check |

### Summary by Likelihood of Requiring Code Changes

| Package | Likelihood of Code Changes | Key Files to Check |
|---------|--------------------------|-------------------|
| Swashbuckle.AspNetCore | **High** | `Program.cs` |
| AutoMapper | **Medium** | `ApiDTOs/AutoMapperProfile.cs`, `Program.cs` |
| Ical.Net | **Medium** | Any service using `Ical.Net` types |
| EF Core | **Low** | `Models/LifelogBbContext.cs`, migrations |
| JwtBearer | **Low** | `Program.cs` auth setup |
| All others | **Low** | — |

---

## Testing & Validation Strategy

### Phase 1 Validation (Post Atomic Upgrade)

| Check | Method | Expected Outcome |
|-------|--------|-----------------|
| Dependency restore | `dotnet restore` | 0 errors |
| Solution build | `dotnet build` | 0 errors, 0 warnings (or annotated warnings) |
| Application startup | Run app locally | Application starts without exceptions |
| Swagger UI | Navigate to `/swagger` | OpenAPI UI loads, all endpoints listed |
| JWT authentication | POST to `/api/auth`, use token | Token issued, authorized endpoints accessible |
| Cookie authentication | Web UI login | Session established, pages load correctly |
| EF Core migrations | `dotnet ef database update` | Migrations apply cleanly to SQLite DB |

### Phase 2 Validation (Functional Testing)

| Area | Test Approach | Notes |
|------|---------------|-------|
| All API endpoints | Manual or automated: GET, POST, PUT, DELETE for each entity | Covers ApiControllers using the generic `BaseCRUDController` |
| AutoMapper mappings | Verify entity → DTO and DTO → entity mappings produce correct data | Pay special attention after major version jump |
| Ical.Net features | Exercise any iCal export/import or recurrence rule features | Test timezone-sensitive scenarios |
| EF Core queries | Run typical data read/write operations on each tracked entity | Check for unexpected SQL or behaviour changes |
| JWT + Cookie auth | Full authentication flow end-to-end | Both schemes via the combined `JWT_OR_COOKIE` policy |

### No Automated Test Projects Found
The assessment identifies no test projects in the solution. Testing relies on:
1. Build-time validation (compiler errors)
2. Manual functional testing of API and web UI flows
3. EF Core migration validation

> ⚠️ Recommendation: After the upgrade, consider adding an xUnit/NUnit test project to prevent regressions in future upgrades.

---

## Risk Management

### Risk Register

| Risk | Level | Description | Mitigation |
|------|-------|-------------|------------|
| AutoMapper 12→16 API changes | **Medium** | 4 major version jumps; DI extension package merged; mapping API changes possible | Review `AutoMapperProfile.cs` post-upgrade; check startup mapping validation |
| Ical.Net 4→5 API changes | **Medium** | Major version with known namespace/API changes; limited community documentation | Search all Ical.Net usages before upgrade; test iCal paths thoroughly |
| Swashbuckle 6→10 configuration | **Medium** | Multiple major versions; OpenAPI setup changed in v7+; JWT security scheme registration may differ | Compare `Program.cs` Swagger config against Swashbuckle 10 docs; update registration |
| EF Core 7→10 query behaviour | **Low** | LINQ translation improvements may change edge-case query results | Run all data access paths manually after upgrade |
| No automated tests | **Low** | Regressions can only be caught manually | Follow the functional testing checklist; prioritise high-traffic paths |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` (preview) | **Low** | Scaffolding tool; no runtime impact; preview build available | Keep at `7.0.5` or omit from project file — does not affect production |
| `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (no update) | **Low** | Docker tooling; no runtime impact | Keep at `1.18.1` or remove if Docker is not used |

### Contingency Plans

| Scenario | Response |
|----------|----------|
| AutoMapper compilation failures after upgrade | Consult AutoMapper v13/v14/v16 migration guides; update `ForMember`, `ResolveUsing`, or profile API usage |
| Ical.Net compilation failures | Refer to Ical.Net 5.x migration notes; update type references and serialiser calls |
| Swashbuckle compilation failures | Refer to Swashbuckle 7+ release notes; rewrite Swagger setup in `Program.cs` following new fluent API |
| EF Core runtime behaviour change | Identify affected query in EF Core 8/9/10 breaking changes docs; add `.AsNoTracking()` or explicit query rewrite |
| Application fails to start | Use `git diff` to isolate change; revert individual package version and re-test incrementally |
| Cannot resolve stable version for a package | Keep package at current version and document as a known constraint; re-evaluate in a future upgrade cycle |

---

## Complexity & Effort Assessment

### Per-Project Complexity

| Project | Complexity | LOC | Packages to Update | Risk Factors |
|---------|-----------|-----|-------------------|--------------|
| `LifelogBb` | **Medium** | 15,140 | 12 | Multiple major-version package jumps (AutoMapper, Ical.Net, Swashbuckle); no automated tests |

### Phase Complexity

| Phase | Complexity | Dominant Risk |
|-------|-----------|---------------|
| Phase 0 — Prerequisites | **Low** | SDK availability |
| Phase 1 — Atomic Upgrade | **Medium** | Major package API changes requiring code fixes |
| Phase 2 — Test Validation | **Medium** | Manual testing required; no automated test suite |

### Resource Requirements

| Skill | Requirement |
|-------|-------------|
| .NET / ASP.NET Core | Needed — fix compilation errors from package changes |
| EF Core | Basic — verify migrations and queries |
| AutoMapper | Intermediate — resolve v12→v16 API changes |
| Ical.Net | Basic — search and fix any changed types |
| Swashbuckle / OpenAPI | Intermediate — update `Program.cs` Swagger setup |

---

## Source Control Strategy

### Branch Strategy
- **Source branch**: `main`
- **Upgrade branch**: `upgrade-to-NET10` ← all upgrade changes committed here
- **Merge approach**: Pull Request from `upgrade-to-NET10` → `main` once all validation passes

### Commit Strategy (All-At-Once)
Prefer a **single commit** containing the entire atomic upgrade:
- All `TargetFramework` changes
- All `PackageReference` version updates
- All code fixes from compilation errors
- Any `Program.cs` or configuration updates

**Commit message format:**
```
chore: upgrade to .NET 10 and align all NuGet packages

- TargetFramework: net7.0 → net10.0
- EF Core: 7.0.4 → 10.0.5
- JwtBearer: 7.0.4 → 10.0.5
- AutoMapper: 12.0.1 → 16.1.1
- Ical.Net: 4.2.0 → 5.2.1
- Swashbuckle.AspNetCore: 6.5.0 → 10.1.7
- BCrypt.Net-Next: 4.0.3 → 4.1.0
- Westwind.AspNetCore.Markdown: 3.11.0 → 3.31.0
- Fix compilation errors from package breaking changes
```

If code fixes for individual packages are complex, they may be split into focused commits on the same branch before the PR.

### Review Checklist (PR)
- [ ] All packages at target versions
- [ ] No preview package versions in production dependencies
- [ ] Build passes with 0 errors
- [ ] Functional validation checklist completed
- [ ] `CHANGELOG` or `README` updated if version is documented there

---

## Success Criteria

The upgrade is **complete** when all of the following are satisfied:

### Technical Criteria

| Criterion | Verification |
|-----------|-------------|
| `TargetFramework` = `net10.0` in `LifelogBb.csproj` | Inspect project file |
| All Microsoft platform packages at `10.0.5` | Inspect project file |
| AutoMapper at `16.1.1` | Inspect project file |
| Ical.Net at `5.2.1` | Inspect project file |
| Swashbuckle.AspNetCore at `10.1.7` | Inspect project file |
| BCrypt.Net-Next at `4.1.0` | Inspect project file |
| Westwind.AspNetCore.Markdown at `3.31.0` | Inspect project file |
| `dotnet restore` succeeds with 0 errors | Run restore |
| `dotnet build` succeeds with 0 errors | Run build |
| Application starts without exceptions | Manual start |
| Swagger UI accessible at `/swagger` | Browser check |
| EF Core migrations apply cleanly | `dotnet ef database update` |
| Authentication flows (cookie + JWT) functional | Manual test |
| No security vulnerabilities in final package set | `dotnet list package --vulnerable` |

### Quality Criteria

| Criterion | Verification |
|-----------|-------------|
| No preview-version packages in runtime dependencies | Inspect project file |
| No unnecessary packages (e.g. redundant DI extension after AutoMapper 16) | Inspect project file |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` removed if not needed | Inspect project file + build |

### Process Criteria

| Criterion | Verification |
|-----------|-------------|
| All changes on `upgrade-to-NET10` branch | `git log` |
| Single (or clean, well-described) commit(s) | `git log` |
| PR created `upgrade-to-NET10` → `main` | GitHub/Bitbucket PR |
| Plan followed without skipping steps | Review execution log |
