# dotnet-api-clean-architecture

This repository demonstrates a simplified implementation of Clean Architecture principles in .NET through a practical plant tracking application. The PlantTracker Web Api enables users to maintain a comprehensive plant database, storing essential information including common names, scientific names, USDA website URLs, plant age, and growth duration. All data is persisted using Amazon DynamoDB, showcasing how Clean Architecture facilitates easy integration with cloud-based storage solutions.

## Overview

This code implements Clean Architecture, a design principle that enables separation of concerns and framework independence. The solution is organized into four distinct layers, each represented by a .NET project: PlantTracker.Application, PlantTracker.Core, PlantTracker.Infrastructure, and PlantTracker.WebApi.

This layered approach isolates business logic from infrastructure concerns, allowing individual layers to be replaced without requiring extensive refactoring. The Core layer serves as the foundation, minimizing dependencies between other layers. For example, when the Application layer needs to access Infrastructure services, it uses dependency injection through interfaces defined in the Core layer.

While Clean Architecture may require more initial code compared to simpler design patterns, it prioritizes long-term maintainability. This investment allows developers to modify, remove, or replace components without triggering major refactors or complete rewrites.

#### Project Layers
| Layer Name | Project Name | Description |
|---------|-----------|----------|
| Applicaiton | PlantTracker.Application.csproj | The Application layer encapsulates the application's business logic and use cases, depending solely on the Core layer to maintain separation from infrastructure concerns. |
| Core | PlantTracker.Core.csproj | The Core layer defines the shared interfaces and domain models used across all layers, serving as the architectural foundation. |
| Infrastructure | PlantTracker.Infrastructure.csproj | The Infrastructure layer contains implementations for external systems the application relies on, such as databases, APIs, caches, and file storage. |
| Presentation | PlantTracker.WebApi.csproj | The presentation layer is exposed to consumers and contains controllers, middleware (authentication, error handling), dependency injection configuration, and project initialization. |
<hr/>

### Project Structure and Dependencies

#### Project Structure Overview

The high-level folder structure demonstrates clear separation of concerns, organizing the solution into two primary directories: src for source code and test for testing projects. Additional supporting directories include docker for containerization, data for seed files or schemas, and scripts for automation and utility tasks.

```
dotnet-api-clean-architecture/
├── .github/
├── data/
├── docker/
├── scripts/
├── src/
│   ├── PlantTracker.Application.csproj/ 
│   │   ├── Services/
│   │   └── DependencyInjection/
│   ├── PlantTracker.Core.csproj/
│   │   ├── Constants/
│   │   ├── Exceptions/
│   │   ├── Interfaces/
│   │   ├── Models/
│   │   └── Validations/
│   ├── PlantTracker.Infrasturcture.csproj/
│   │   ├── Configurations/
│   │   ├── Converters/
│   │   ├── DependencyInjection/
│   │   ├── Models/
│   │   └── Repositories/
│   ├── PlantTracker.WebApi.csproj/
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   |   └── Identity/
│   │   └── Program.cs
├── tests/
│   ├── PlantTracker.Application.UnitTests.csproj/
│   ├── PlantTracker.Core.UnitTests.csproj/
│   ├── PlantTracker.Infrasturcture.UnitTests.csproj/
│   └── PlantTracker.WebApi.UnitTests.csproj/
├── .gitignore
├── docker-compose.yml
└── README.md
```

#### Project Dependency Diagram

This dependency graph illustrates the relationships between the main source projects. Notice that all layers depend on the Core layer, which contains the shared interfaces and contracts. The WebApi layer (presentation) references all other layers solely for dependency injection configuration, maintaining the architectural principle that higher-level layers don't directly invoke lower-level implementations.
 
 ![Dependency Diagram](./docs/images/ProjectDependencyDiagram.png)

<hr/>

## PlantTracker Web Api

The PlantTracker Web API provides full CRUD operations for managing plant records in DynamoDB. Security is implemented through JWT token-based authentication and authorization. A global error handler ensures that all responses return standardized Problem Details objects while preventing exposure of sensitive internal information or stack traces.

### API Endpoints

The table below lists the available Web API endpoints. Complete endpoint documentation is also available in the OpenAPI specification file (PlantTracker.WebApi.json) and through the interactive Scalar UI that launches automatically when running the Web API.

| Method | Endpoint | Description | Authentication Required | Authorization Admin Role Required |
|--------|----------------|----------------|-----|----|
| GET    | `/plant/users` | Get all plants | ✅ | ❌ |
| GET    | `/plant/{id}`  | Get plant      | ✅ | ❌ |
| PUT    | `/plant`       | Create plant   | ✅ | ✅ |
| PATCH  | `/plant/{id}`  | Update plant   | ✅ | ✅ |
| DELETE | `/plant/{id}`  | Delete user    | ✅ | ✅ |


### Plant Table

The table below outlines the Plant table schema, including column names, data types, and whether each column is required.

| Column | Data Type | Description | Required |
|--------|----------------|----------------|---- |
| Id                    | `string`          | Id of the plant. Value must be a Guid.             | ✅ |
| CommonName            | `string`          | Common Name of the plant.                          | ✅ |
| ScientificName        | `string`          | Scienific Name of the plant.                       | ✅ |
| Age                   | `int`             | Age of plant in years.                             | ✅ |
| Duration              | `string`          | Duration of plant (i.e. Annual, Perennial, Unknown)| ✅ |
| CreatedDateUtc        | `DateTimeOffset`  | Created Date and time of the record in UTC         | ✅ |
| ModifiedDateUtc       | `DateTimeOffset`  | Modified Date and time of the record in UTC        | ✅ |

### Authentication

This Web API uses JWT authentication for secure access. To generate a JWT token for testing and accessing protected endpoints, please reference and use the [dotnet-api-identity](https://github.com/ckonkol1/dotnet-api-identity) Web Api. 

## Local Development

Running the Web Api locally can be accomplished by installing Docker Desktop and executing the Docker Compose file. This creates a local DynamoDB database and automatically populates it with sample data. After launching the Api in your preferred IDE, you can test endpoints using the OpenApi Scalar UI, the PlantTracker.http file, or any HTTP testing tool.

### Requirements
1. [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Steps to Run the Web API locally
1. Clone or Fork the repository
2. Navigate to the project root directory via the command line and run ```docker compose up```. This will run the \dotnet-api-clean-architecture\local\scripts\init-dynamodb.sh script and utilitze the \dotnet-api-clean-architecture\local\data\plants-seed-data.json to populate the database with sample data. You may also access the [DynamoDB Admin UI](http://localhost:8001/)
3. Build, Launch, and obtain a JWT token from the [dotnet-api-identity](https://github.com/ckonkol1/dotnet-api-identity)
4. Build and launch the PlantTracker Web API.
5. Use the \dotnet-api-clean-architecture\src\PlantTracker.WebApi\PlantTracker.http file or the [OpenAPI Scalar UI](https://localhost:7205/scalar/v1) to access the endpoints.
