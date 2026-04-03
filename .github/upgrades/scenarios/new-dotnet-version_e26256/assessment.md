# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v10.0.

## Table of Contents

- [Executive Summary](#executive-Summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [LifelogBb\LifelogBb.csproj](#lifelogbblifelogbbcsproj)


## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 1 | 0 require upgrade |
| Total NuGet Packages | 13 | All compatible |
| Total Code Files | 190 |  |
| Total Code Files with Incidents | 0 |  |
| Total Lines of Code | 15140 |  |
| Total Number of Issues | 0 |  |
| Estimated LOC to modify | 0+ | at least 0,0% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :--- |
| [LifelogBb\LifelogBb.csproj](#lifelogbblifelogbbcsproj) | net10.0 | ✅ None | 0 | 0 |  | AspNetCore, Sdk Style = True |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 13 | 100,0% |
| ⚠️ Incompatible | 0 | 0,0% |
| 🔄 Upgrade Recommended | 0 | 0,0% |
| ***Total NuGet Packages*** | ***13*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |
| AutoMapper | 12.0.1 |  | [LifelogBb.csproj](#lifelogbblifelogbbcsproj) | ✅Compatible |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 |  | [LifelogBb.csproj](#lifelogbblifelogbbcsproj) | ✅Compatible |
| BCrypt.Net-Next | 4.0.3 |  | [LifelogBb.csproj](#lifelogbblifelogbbcsproj) | ✅Compatible |
| Ical.Net | 4.2.0 |  | [LifelogBb.csproj](#lifelogbblifelogbbcsproj) | ✅Compatible |
| Microsoft.AspNetCore.Authentication.JwtBearer | 7.0.4 |  | [LifelogBb.csproj](#lifelogbblifelogbbcsproj) | ✅Compatible |
| Microsoft.EntityFrameworkCore.Design | 7.0.4 |  | [LifelogBb.csproj](#lifelogbblifelogbbcsproj) | ✅Compatible |
| Microsoft.EntityFrameworkCore.Sqlite | 7.0.4 |  | [LifelogBb.csproj](#lifelogbblifelogbbcsproj) | ✅Compatible |
| Microsoft.EntityFrameworkCore.SqlServer | 7.0.4 |  | [LifelogBb.csproj](#lifelogbblifelogbbcsproj) | ✅Compatible |
| Microsoft.EntityFrameworkCore.Tools | 7.0.4 |  | [LifelogBb.csproj](#lifelogbblifelogbbcsproj) | ✅Compatible |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.18.1 |  | [LifelogBb.csproj](#lifelogbblifelogbbcsproj) | ✅Compatible |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 7.0.5 |  | [LifelogBb.csproj](#lifelogbblifelogbbcsproj) | ✅Compatible |
| Swashbuckle.AspNetCore | 6.5.0 |  | [LifelogBb.csproj](#lifelogbblifelogbbcsproj) | ✅Compatible |
| Westwind.AspNetCore.Markdown | 3.11.0 |  | [LifelogBb.csproj](#lifelogbblifelogbbcsproj) | ✅Compatible |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |

## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;LifelogBb.csproj</b><br/><small>net10.0</small>"]
    click P1 "#lifelogbblifelogbbcsproj"

```

## Project Details

<a id="lifelogbblifelogbbcsproj"></a>
### LifelogBb\LifelogBb.csproj

#### Project Info

- **Current Target Framework:** net10.0✅
- **SDK-style**: True
- **Project Kind:** AspNetCore
- **Dependencies**: 0
- **Dependants**: 0
- **Number of Files**: 198
- **Lines of Code**: 15140
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["LifelogBb.csproj"]
        MAIN["<b>📦&nbsp;LifelogBb.csproj</b><br/><small>net10.0</small>"]
        click MAIN "#lifelogbblifelogbbcsproj"
    end

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

