# Bison

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

Create the database:

```sh
sqlite3 bison.db < schema.sql
```

```sh
sqlite3 bison.db < dump.sql
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

The public timeline is available at:

```text
/
```

A user's timeline is available at:

```text
/<username>
```