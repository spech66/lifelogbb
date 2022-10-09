# lifelogbb

[![Known Vulnerabilities](https://snyk.io/test/github/spech66/lifelogbb/badge.svg)](https://snyk.io/test/github/spech66/lifelogbb)

Lifelog - All your life related things in one place. **Journal, weight, Strength training, endurance training tracker, Bucket list, Vision board, ... **.

This service is build for a **single user**. **Password authentication** is included for the sites and API endpoints.

All **data** is stored in a single **SQLite database** for full control and portability over the data.

(This is the new work in progress version of the old [lifelogspd](https://github.com/spech66/lifelogspd) project).

## Status

* **Weight:** :x:
* **Weight chart:** :x:
* **Journal:** :x:
* **Strength training:** :x:
* **Strength training chart:** :x:
* **Endurance workout:** :x:
* **Endurance workout chart:** :x:
* **Dockerfile** :heavy_check_mark:

## Screenshots

[More screenshots](https://github.com/spech66/lifelogbb/tree/master/_screenshots)

## Run from docker

Clone the repository and run docker.

```powershell
git clone https://github.com/spech66/lifelogbb.git
docker run
```

Hosting a [secure app](https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-6.0).

## Build and run from source

Clone the repository and either install [Visual Studio](https://visualstudio.microsoft.com/) or just the [dotnet tools](https://dotnet.microsoft.com/en-us/learn/aspnet/hello-world-tutorial/install).

Run it by pressing F5 in Visual Studio or using the [dotnet cli](https://dotnet.microsoft.com/en-us/learn/aspnet/hello-world-tutorial/run).

```powershell
git clone https://github.com/spech66/lifelogbb.git
cd LifelogBB
dotnet watch
```

## Helpful tools

* [Postman](https://www.postman.com/) - Test/debug API.
* [SQLite Browser](https://sqlitebrowser.org/) - DB Browser for SQLite.

## Dependencies

* [SQLite](https://www.sqlite.org/index.html) - SQLite is a C-language library that implements a small, fast, self-contained, high-reliability, full-featured, SQL database engine. SQLite is the most used database engine in the world.

## Development

Use Visual Studio.
