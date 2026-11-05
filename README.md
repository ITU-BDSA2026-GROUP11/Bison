# Bison

## Bison.Razor is now deployed on Azure App Service at:

[https://bdsagroup11bison-crhsanhra4h2e9bt.denmarkeast-01.azurewebsites.net](https://bdsagroup11bison-crhsanhra4h2e9bt.denmarkeast-01.azurewebsites.net)

## Login and registration

All users can access the following pages:

```text
/Identity/Account/Login
/Identity/Account/Register
```
To create an account and or login with either Github OAuth authentication, or by registering with an email and password.

Logged in user have the additionel navigation element: "My Observationen" that shows post made by the user itself.

The following accounts are available for testing:

| User      | Email         | Password |
| --------- | ------------- | -------- |
| Eduard    | edka@itu.dk   | 123456   |
| Debasmita | debloh@itu.dk | password |

## Build and test

From the project root:

```sh
dotnet build
```

```sh
dotnet test
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

or

```text
/<username>
```

User timeline with pagination:

```text
/obs/<username>?page=2
```

Each page contains at most 32 observations.

If no page is specified, page 1 is used.

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
