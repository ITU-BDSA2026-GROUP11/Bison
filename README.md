# Bison (Has been removed from main and exist only in the Bison.CLI branch)

## Build and test

From the project root:

```sh
dotnet build
```

```sh
dotnet test
```

## Bison.CLI

From the `Bison.CLI` folder:

```sh
dotnet run read
```

```sh
dotnet run observe <observation> <location>
```

```sh
dotnet run comment <id> <comment>
```

```sh
dotnet run discussion <id>
```

```sh
dotnet run location <location>
```

## Bison.Razor

From the `Bison.Razor` folder:

Install SQLite package if needed:

```sh
dotnet add package Microsoft.Data.Sqlite
```

Create and fill the database:

```sh
sqlite3 bison.db < schema.sql
```

```sh
sqlite3 bison.db < dump.sql
```

On Windows PowerShell (`<` does not work there):

```powershell
sqlite3 bison.db ".read schema.sql"
sqlite3 bison.db ".read dump.sql"
```

Run on macOS/Linux:

```sh
BISONDBPATH=./bison.db dotnet run
```

Run on Windows PowerShell:

```powershell
$env:BISONDBPATH="./bison.db"
dotnet run
```

## Timelines

Public timeline:

```text
/obs
```

Public timeline with pagination:

```text
/obs?page=2
```

User timeline:

```text
/obs/<username>
```

User timeline with pagination:

```text
/obs/<username>?page=2
```

Each page contains at most 32 observations.

If no page is specified, page 1 is used.