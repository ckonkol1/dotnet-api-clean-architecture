## PlantTracker Clean Architecture Web API

- Showcases Clean Architecture patterns in a .NET solution.
- Exposes a RESTful API for plant data management with cloud-ready persistence and clear separation of concerns.
- Supports full CRUD operations, JWT-based authentication, and comprehensive OpenAPI documentation.

This repository provides a practical example of Clean Architecture in .NET, centered around a plant tracking application. The PlantTracker Web API enables users to manage a database of plants, including details like common and scientific names, USDA URLs, age, and growth duration. Data is stored in Amazon DynamoDB, demonstrating how Clean Architecture facilitates integration with cloud-based storage.

## Features

- 🌱 Plant management (CRUD operations) using Clean Architecture
- 📱 RESTful API endpoints
- 📄 API documentation with Scalar/OpenAPI

## Tech Stack

- .NET 9.0
- ASP.NET Core
- Docker
- SQL Server / SQLite
- Swagger/OpenAPI for documentation

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server or SQLite (for development)
- Visual Studio Code or Visual Studio

## Overview

The solution is structured according to Clean Architecture design principles, promoting separation of concerns and framework independence. It is organized into four main layers, each as a separate .NET project:

- **PlantTracker.Application**: Contains business logic and use cases, depending only on the Core layer.
- **PlantTracker.Core**: Defines shared interfaces and domain models, serving as the architectural foundation.
- **PlantTracker.Infrastructure**: Implements external system integrations, such as databases and APIs.
- **PlantTracker.WebApi**: Exposes the API to consumers, containing controllers, middleware, and configuration.

This layered approach isolates business logic from infrastructure, making it easier to modify or replace components without major refactoring. The Core layer minimizes dependencies, and higher-level layers interact with lower-level services via interfaces and dependency injection.

While Clean Architecture may require more initial setup than simpler patterns, it prioritizes long-term maintainability and flexibility.

### Project Layers

| Layer Name   | Project Name                        | Description                                                                                  |
|--------------|-------------------------------------|----------------------------------------------------------------------------------------------|
| Application  | PlantTracker.Application.csproj     | Encapsulates business logic and use cases; depends only on Core.                             |
| Core         | PlantTracker.Core.csproj            | Defines shared interfaces and domain models; the architectural foundation.                   |
| Infrastructure| PlantTracker.Infrastructure.csproj | Implements external system integrations (databases, APIs, etc.).                             |
| Presentation | PlantTracker.WebApi.csproj          | Exposes the API, contains controllers, middleware, and configuration.                        |

---

### Project Structure and Dependencies

#### Folder Structure

The solution is organized for clear separation of concerns, with primary directories for source code (`src`) and tests (`tests`). Supporting directories include `docker` (containerization), `data` (seed files/schemas), and `scripts` (automation).

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
│   ├── PlantTracker.Infrastructure.csproj/
│   │   ├── Configurations/
│   │   ├── Converters/
│   │   ├── DependencyInjection/
│   │   ├── Models/
│   │   └── Repositories/
│   ├── PlantTracker.WebApi.csproj/
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   │   └── Identity/
│   │   └── Program.cs
├── tests/
│   ├── PlantTracker.Application.UnitTests.csproj/
│   ├── PlantTracker.Core.UnitTests.csproj/
│   ├── PlantTracker.Infrastructure.UnitTests.csproj/
│   └── PlantTracker.WebApi.UnitTests.csproj/
├── .gitignore
├── docker-compose.yml
└── README.md
```

#### Project Dependency Diagram

The following diagram illustrates project dependencies. All layers depend on the Core layer, which contains shared interfaces and contracts. The WebApi (presentation) layer references other layers only for dependency injection, maintaining the principle that higher-level layers do not directly invoke lower-level implementations.

![Dependency Diagram](./docs/images/ProjectDependencyDiagram.png)

---

## PlantTracker Web API

The PlantTracker Web API provides full CRUD operations for managing plant records in DynamoDB. Security is enforced via JWT-based authentication and authorization. A global error handler ensures standardized Problem Details responses and prevents exposure of sensitive information.

### API Endpoints

The table below lists available endpoints. Full documentation is available in the OpenAPI spec (`PlantTracker.WebApi.json`) and via the interactive Scalar UI.

| Method | Endpoint        | Description      | Auth Required | Admin Role Required |
|--------|----------------|-----------------|---------------|--------------------|
| GET    | `/plant`       | Get all plants  | ✅            | ❌                 |
| GET    | `/plant/{id}`  | Get plant       | ✅            | ❌                 |
| PUT    | `/plant`       | Create plant    | ✅            | ✅                 |
| PATCH  | `/plant/{id}`  | Update plant    | ✅            | ✅                 |
| DELETE | `/plant/{id}`  | Delete plant    | ✅            | ✅                 |

### Plant Table Structure

| Column          | Data Type         | Description                                         | Required |
|-----------------|------------------|-----------------------------------------------------|----------|
| Id              | `string`         | Plant ID (GUID)                                     | ✅       |
| CommonName      | `string`         | Common name                                         | ✅       |
| ScientificName  | `string`         | Scientific name                                     | ✅       |
| Age             | `int`            | Age in years                                        | ✅       |
| Duration        | `string`         | Duration (Annual, Perennial, Unknown)               | ✅       |
| CreatedDateUtc  | `DateTimeOffset` | Record creation date/time (UTC)                     | ✅       |
| ModifiedDateUtc | `DateTimeOffset` | Record last modified date/time (UTC)                | ✅       |

### Authentication

JWT authentication secures all endpoints. To generate a JWT token for testing, use the [dotnet-api-identity](https://github.com/ckonkol1/dotnet-api-identity) Web API.

## Local Development

You can run the Web API locally using Docker Compose, which provisions a local DynamoDB instance and automatically seeds it with sample plant data. Once the API is running, you can interact with endpoints using the OpenAPI Scalar UI, the provided `PlantTracker.http` file, or any HTTP client of your choice.

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Steps

1. Clone or fork the repository.
2. In the project root, run `docker compose up` to start DynamoDB and seed data using `scripts/init-dynamodb.sh` and `data/plants-seed-data.json`. Access the [DynamoDB Admin UI](http://localhost:8001/) if needed.
3. Build and launch the [dotnet-api-identity](https://github.com/ckonkol1/dotnet-api-identity) API to obtain a JWT token.
4. Build and start the PlantTracker Web API.
5. Use `src/PlantTracker.WebApi/PlantTracker.http` or the [OpenAPI Scalar UI](https://localhost:7205/scalar/v1) to test endpoints.

