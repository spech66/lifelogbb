$sqlitebin = ".\sqlite3.exe"

if (!(Test-Path $sqlitebin)) {
  # Check for SQLite version
  $url = "https://www.sqlite.org/download.html"
  $html = Invoke-WebRequest -Uri $url
  $result = select-string -Pattern 'PRODUCT.*,(.*tools-win32.*\.zip)' -inputobject $html.Content
  $version = $result.matches.groups[1].value

  "SQLite3.exe was not found in current directory. Please download and put in the script dir."
  "Latest version at https://www.sqlite.org/$version on https://www.sqlite.org/download.html"
  exit 1
}

$dbfile = "$env:LOCALAPPDATA\liefelogbb.db"
if (!(Test-Path $dbfile)) {
  "Could not find database at '$dbfile'."
  exit 2
}
# TODO: Parse path from app config

Invoke-Expression -Command "$sqlitebin $dbfile .dump" > backup.sql
