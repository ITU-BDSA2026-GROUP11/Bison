# Testing Bison

Run all commands from the Bison project root.

## 1. Unit Tests

Works on both Mac and Windows. No server required.

```bash
dotnet test tests/Bison.Tests
```

## 2. Start the Server (Terminal 1)

Mac:

```bash
export BISON_E2E_DB="$PWD/bison-e2e.db"
export BISON_E2E_CONNECTION="Data Source=$BISON_E2E_DB"

ASPNETCORE_ENVIRONMENT=E2E dotnet run --no-launch-profile \
  --project src/Bison.Razor \
  --urls http://127.0.0.1:5005
```

Windows (PowerShell):

```powershell
$env:BISON_E2E_DB = Join-Path (Get-Location) "bison-e2e.db"
$env:BISON_E2E_CONNECTION = "Data Source=$env:BISON_E2E_DB"
$env:ASPNETCORE_ENVIRONMENT = "E2E"

dotnet run --no-launch-profile --project src/Bison.Razor --urls http://127.0.0.1:5005
```

Leave Terminal 1 running. The server must use an initialized, separate E2E database.

## 3. Run E2E and Accessibility Tests (Terminal 2)

Mac:

```bash
BISON_E2E_DB="$PWD/bison-e2e.db" dotnet test tests/Bison.E2ETests
```

Windows (PowerShell):

```powershell
$env:BISON_E2E_DB = Join-Path (Get-Location) "bison-e2e.db"

dotnet test tests/Bison.E2ETests
```

## 4. Other Commands

Run all tests

```bash
dotnet test
```

Run only axe accessibility tests:

```bash
dotnet test tests/Bison.E2ETests --filter "Name~Page_HasNoAxeViolations"
```

Important: Use a separate test database, never the production database.
