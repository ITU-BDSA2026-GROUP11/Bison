# Bison (Has been removed from main and exist only in the Bison.CLI branch)

## Build and test

From the project root:

```sh
dotnet build
```

```sh
dotnet test
```

## Bison.CLI (older project - look in the Bison.CLI branch)

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

Run the program

```sh
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
