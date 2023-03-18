# LifelogBB

![Dotnet Workflow](https://github.com/spech66/lifelogbb/actions/workflows/dotnet.yml/badge.svg)
![CodeQL Workflow](https://github.com/spech66/lifelogbb/actions/workflows/codeql.yml/badge.svg)

Lifelog - All your life related things in one place. **Journal, weight, Strength training, endurance training tracker, Bucket list, Vision board, ...**.

This service is build for a **single user**. **Password authentication** is included for the sites and API endpoints.

All **data** is stored in a single **SQLite database** for full control and portability over the data.

## Features

* 🦄 Free open source software
* ⚖️ Weight tracking
* 📔 Journal
* 🏋️ Strength training tracker
* 🏃 Endurance training tracker
* ✅ Todos
* 🎯 Goals
* 🚀 Habits
* 🌄 Bucket list and Vision board
* 📜 Quotes
* 🛠️ RESTful API for all routes
* ⚙️ Settings
* 🖼 [Tabler](https://tabler.io/) UI

[TODO](https://github.com/spech66/lifelogbb/blob/main/TODO.md)

## Technical

* 📦 Self hosting
* 🔐 Authentication
* 📂 SQLite database

## Screenshots

![Start](https://raw.githubusercontent.com/spech66/lifelogbb/main/_screenshots/s_001_start.jpg "Start")
![Dashboard](https://raw.githubusercontent.com/spech66/lifelogbb/main/_screenshots/s_002_dashboard.png "Dashboard")
![Weight](https://raw.githubusercontent.com/spech66/lifelogbb/main/_screenshots/s_002_weight_01.jpg "Weight")
![Bucket List Vision Board](https://raw.githubusercontent.com/spech66/lifelogbb/main/_screenshots/s_006_bucketlist_02.jpg "Bucket List Vision Board")

[More screenshots](https://github.com/spech66/lifelogbb/tree/main/_screenshots)

## Run from docker

Clone the repository and run docker.

```sh
git clone https://github.com/spech66/lifelogbb.git
cd lifelogbb
```

Set database path to `"/database"` in the `appsettings`.

```sh
cd LifelogBb
docker build . -t lifelogbb
docker run -v lifelogbbdatabase:/database -p 80:80 -p 443:443 lifelogbb
```

Hosting a [secure app](https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-6.0).

## Build and run from source

Clone the repository and either install [Visual Studio](https://visualstudio.microsoft.com/) or just the [dotnet tools](https://dotnet.microsoft.com/en-us/learn/aspnet/hello-world-tutorial/install).

Run it by pressing F5 in Visual Studio or using the [dotnet cli](https://dotnet.microsoft.com/en-us/learn/aspnet/hello-world-tutorial/run).

```powershell
git clone https://github.com/spech66/lifelogbb.git
cd LifelogBb
dotnet watch
```

## Helpful tools

* [Postman](https://www.postman.com/) - Test/debug API.
* [SQLite Browser](https://sqlitebrowser.org/) - DB Browser for SQLite.

## Dependencies

* [SQLite](https://www.sqlite.org/index.html) - SQLite is a C-language library that implements a small, fast, self-contained, high-reliability, full-featured, SQL database engine. SQLite is the most used database engine in the world.

## Development

Use Visual Studio.

Swagger UI at `https://localhost:7290/Swagger/`.

Generate database migrations using `Add-Migration NAME`.

Apply migrations using `Update-Database`.

## Resources used

* [Combining Bearer Token and Cookie Authentication in ASP.NET](https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET)
* [Import a CSV File Into an SQLite Table](https://www.sqlitetutorial.net/sqlite-import-csv/) - Migrate from lifelogspd to LifelogBB
