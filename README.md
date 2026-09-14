# Bison.CLI

### Commands
These Commands should be run while in the Bison folder (project root)
1. Building the project
   ```sh
   dotnet build
   ```
2. Testing the project
   ```sh
   dotnet test
   ```

   These Commands should be run while in the Bison.CLI folder
3. Reading from CSV file
   ```sh
   dotnet run read
   ```
4. Writing to the CSV observe file
   where observation is text
   ```sh
   dotnet run observe <observation>
   ```
5. Writing a comment to the CSV comment file
   Where id is an observation id, and comment is text
   ```sh
   dotnet run comment <id> <comment>
   ```
6. Displaying all comment relating to a observation
   Where id is an observation id
   ```sh
   dotnet run discussion <id>
   ```
