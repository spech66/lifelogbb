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
* 🛠️ RESTful API for all routes, Swagger UI
* 📅 iCal feeds: Todo for Todos and Goals, Event for Habits (Time boxing/blocking)
* ⚙️ Settings
* 🖼 [Tabler](https://tabler.io/) UI

[TODO](https://github.com/spech66/lifelogbb/blob/main/TODO.md)

## Technical

* 📦 Self hosting
* 🔐 Authentication
* 📂 SQLite database

## Screenshots

[![Start](https://raw.githubusercontent.com/spech66/lifelogbb/main/_screenshots/s_001_start.png "Start")](https://raw.githubusercontent.com/spech66/lifelogbb/main/_screenshots/001_start.png)
[![Dashboard](https://raw.githubusercontent.com/spech66/lifelogbb/main/_screenshots/s_002_dashboard.png "Dashboard")](https://raw.githubusercontent.com/spech66/lifelogbb/main/_screenshots/002_dashboard.png)
[![Weight](https://raw.githubusercontent.com/spech66/lifelogbb/main/_screenshots/s_002_weight_01.png "Weight")](https://raw.githubusercontent.com/spech66/lifelogbb/main/_screenshots/002_weight_01.png)
[![Bucket List Vision Board](https://raw.githubusercontent.com/spech66/lifelogbb/main/_screenshots/s_006_bucketlist_02.png "Bucket List Vision Board")](https://raw.githubusercontent.com/spech66/lifelogbb/main/_screenshots/006_bucketlist_02.png)

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

## Build and run from source

* Install [dotnet](https://dotnet.microsoft.com/download) SDK 7.0 on [Linux](https://learn.microsoft.com/en-us/dotnet/core/install/linux?WT.mc_id=dotnet-35129-website) or [Windows](https://learn.microsoft.com/en-us/dotnet/core/install/windows?tabs=net70).
* Install dotnet-ef for migrations `dotnet tool install --global dotnet-ef`
* Checkout code (see below) or download the [latest release](https://github.com/spech66/lifelogbb/releases)
* Adjust `appsettings.Production.json` to your needs
* Create empty database file `lifelogbb.db` in the `LifelogBb` folder (or adjust `appsettings.Production.json`)
 * Create an empty file or use sqlite cli `sqlite3 lifelogbb.db "VACUUM;"`
 * Run migrations (see below)
* Run `dotnet watch` in the `LifelogBb` folder

```sh
git clone https://github.com/spech66/lifelogbb.git
cd lifelogbb
cd LifelogBb
dotnet watch
```

## Migrate database

All migrations are bundled in the `efbundle` file. Run it with the `--connection` argument.
For Windows the `efbundle` is called `efbundle.exe`.

```sh
# Install dotnet-ef
dotnet tool install --global dotnet-ef | echo "already installed"

# Run efbundle
./efbundle --connection "Data Source=lifelogbb.db"
```

## Dependencies

* [.NET 7.0](https://dotnet.microsoft.com/en-us/) - .NET is a free, cross-platform, open source developer platform for building many different types of applications.
* [SQLite](https://www.sqlite.org/index.html) - SQLite is a C-language library that implements a small, fast, self-contained, high-reliability, full-featured, SQL database engine. SQLite is the most used database engine in the world.

## Helpful tools

* [Postman](https://www.postman.com/) - Test/debug API.
* [SQLite Browser](https://sqlitebrowser.org/) - DB Browser for SQLite.

## Configuration examples

### appsettings.production.json

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "Account": {
        "Password": "xxxx" // Generate on first run in the password dialog
    },
    "Database": {
        "Path": "/opt/lifelogbb/"
    },
    "Uploads": {
        "Path": "/opt/lifelogbb/uploads/"
    },
    "Authentication": {
        "Cookie": {
            "ExpireDays": "30"
        },
        "JwtToken": {
            "Issuer": "https://localhost:6067/",
            "Audience": "https://localhost:6067/",
            "SigningKey": "xxxxxxxx", // Generate e.g. openssl genrsa -out ./jwt.key 4096
            "TokenTimeoutMinutes": "60"
        }
    },
    "Kestrel": {
        "Endpoints": {
            "Http": {
                "Url": "http://localhost:6066"
            }
        }
    }
}
```

### nginx

```
server {
    listen 443 ssl http2;
    listen       [::]:443 ssl http2;

    server_name lifelog.example.org;

    index index.html index.htm;

    ssl_certificate /etc/letsencrypt/live/lifelog.example.org/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/lifelog.example.org/privkey.pem;
    ssl_session_timeout 1d;
    ssl_session_cache shared:MozSSL:10m;  # about 40000 sessions
    ssl_session_tickets off;
    ssl_dhparam /etc/ssl/dhparam.pem;

    # intermediate configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-ECDSA-CHACHA20-POLY1305:ECDHE-RSA-CHACHA20-POLY1305:DHE-RSA-AES128-GCM-SHA256:DHE-RSA-AES256-GCM-SHA384;
    ssl_prefer_server_ciphers off;

    location / {
            proxy_pass http://localhost:6066; # Port of your appsettings
            proxy_http_version 1.1;
            proxy_set_header   Upgrade $http_upgrade;
            proxy_set_header   Connection keep-alive;
            proxy_set_header   Host $host;
            proxy_cache_bypass $http_upgrade;
            proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
```

### Service file

Assuming a release is installed in `/var/www/lifelogbb` and the `www-data` user has write access to the database.
For enhanced security you can create a dedicated user for the service.

```
[Unit]
Description=LifelogBbDeamon

[Service]
WorkingDirectory=/var/www/lifelogbb
ExecStart=/usr/bin/dotnet /var/www/lifelogbb/LifelogBb.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-lifelogbb
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

## Development

Clone the repository and either install [Visual Studio](https://visualstudio.microsoft.com/) or just the [dotnet tools](https://dotnet.microsoft.com/en-us/learn/aspnet/hello-world-tutorial/install).

Run it by pressing F5 in Visual Studio or using the [dotnet cli](https://dotnet.microsoft.com/en-us/learn/aspnet/hello-world-tutorial/run).

```sh
git clone https://github.com/spech66/lifelogbb.git
cd lifelogbb
cd LifelogBb
dotnet watch
```

Swagger UI at `https://localhost:7290/Swagger/`.

Generate database migrations using `Add-Migration NAME`.

Apply migrations using `Update-Database`.

## Additional resources

* Hosting a [secure app](https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-6.0).
* [Combining Bearer Token and Cookie Authentication in ASP.NET](https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET)
* [Import a CSV File Into an SQLite Table](https://www.sqlitetutorial.net/sqlite-import-csv/) - Migrate from lifelogspd to LifelogBB
