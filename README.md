# Bison.CLI

### Commands
These commands can be run while in the Bison folder (project root)
1. Building the project
   ```sh
   dotnet build
   ```
2. Testing the project
   ```sh
   dotnet test
   ```

   These commands can be run while in the Bison.CLI folder
3. Building the project
   ```sh
   dotnet build
   ```
4. Reading from CSV file
   ```sh
   dotnet run read
   ```
5. Writing to the CSV observe file
   ```sh
   dotnet run observe <observation> <location>
   ```
6. Writing a comment to the CSV comment file
   ```sh
   dotnet run comment <id> <comment>
   ```
7. Displaying all comment relating to a observation
   ```sh
   dotnet run discussion <id>
   ```
6. Displaying all observations relating to a location
   ```sh
   dotnet run location <location>
   ```
