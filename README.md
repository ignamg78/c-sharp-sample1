# Library App

## Description
Library App es una aplicación de consola para gestionar usuarios y préstamos de libros. La aplicación permite buscar usuarios, ver detalles de usuarios y préstamos, y realizar acciones sobre ellos.

## Project Structure
- `AccelerateDevGitHubCopilot.sln`
- `src/`
  - `Library.ApplicationCore/`
    - `Library.ApplicationCore.csproj`
    - `Entities/`
    - `Enums/`
    - `Interfaces/`
    - `Services/`
  - `Library.Console/`
    - `appSettings.json`
    - `CommonActions.cs`
    - `ConsoleApp.cs`
    - `ConsoleState.cs`
    - `Library.Console.csproj`
    - `Program.cs`
    - `Json/`
  - `Library.Infrastructure/`
    - `Library.Infrastructure.csproj`
    - `Data/`
- `tests/`
  - `UnitTests/`
    - `LoanFactory.cs`
    - `PatronFactory.cs`
    - `UnitTests.csproj`
    - `ApplicationCore/`

## Key Classes and Interfaces
- **Library.ApplicationCore**
  - `Entities/`
    - `Patron`: Representa un usuario de la biblioteca.
    - `Loan`: Representa un préstamo de libro.
  - `Interfaces/`
    - `IPatronRepository`: Define los métodos para acceder a los datos de los usuarios.
    - `ILoanRepository`: Define los métodos para acceder a los datos de los préstamos.
    - `ILoanService`: Define los métodos para gestionar los préstamos.
    - `IPatronService`: Define los métodos para gestionar los usuarios.
  - `Services/`
    - `LoanService`: Implementa la lógica de negocio para los préstamos.
    - `PatronService`: Implementa la lógica de negocio para los usuarios.
- **Library.Console**
  - `ConsoleApp`: Clase principal que gestiona la interacción con el usuario.
  - `Program`: Punto de entrada de la aplicación de consola.
- **Library.Infrastructure**
  - `Data/`
    - `JsonData`: Clase para cargar y guardar datos desde y hacia archivos JSON.
    - `JsonPatronRepository`: Implementación de `IPatronRepository` utilizando archivos JSON.
    - `JsonLoanRepository`: Implementación de `ILoanRepository` utilizando archivos JSON.

## Usage
1. Clona el repositorio:
   ```sh
   git clone <URL del repositorio>
2. Navega al directorio del proyecto:
    cd c-sharp-sample1
3. Restaura las dependencias y construye el proyecto:
    dotnet restore
    dotnet build
4. Ejecuta la aplicación de consola:
    dotnet run --project src/Library.Console/Library.Console.csproj

## License
Este proyecto está licenciado bajo los términos de la MIT License. ```
