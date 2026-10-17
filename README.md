# RaceDay API

## 1. Project Overview

RaceDay is an ASP.NET Core Web API developed to manage race-day activities. It provides functionality for managing users, events, categories, event routes, enrolments, weather information and race results.

The API allows organisers to manage race events and related information, while participants can register, log in, enrol in events and view their relevant information.

## 2. Technologies Used

- C#
- ASP.NET Core Web API
- .NET 8
- Entity Framework Core
- Microsoft SQL Server LocalDB
- Swagger for API testing and documentation
- xUnit for automated testing
- GitHub and GitHub Actions

## 3. Main Features

- User registration, login and logout.
- Session-based authentication and role-based access.
- Viewing and managing race events.
- Managing event categories and routes.
- Participant enrolments.
- Recording and viewing race results.
- Accessing weather information associated with events.
- Viewing and updating user profile information.

Access to certain operations depends on the user's role and permissions.

## 4. Database

The project uses Entity Framework Core to communicate with the SQL Server database.

The database includes the following main entities:

- Users
- Events
- Weather
- Categories
- EventRoutes
- Enrolments
- Results

The database connection is configured in `appsettings.json` using the `RaceDayConnection` connection string.

## 5. Getting Started

### Prerequisites

Before running the project, install:

- Visual Studio 2022
- .NET 8 SDK
- SQL Server LocalDB
- Git, if you want to clone the repository

### Running the API

1. Clone or download the repository.
2. Open the solution in Visual Studio 2022.
3. Check the database connection string in `appsettings.json`.
4. Restore the NuGet packages.
5. Apply the Entity Framework Core database migrations if the database has not been created.
6. Set `RaceDay.Api` as the startup project.
7. Run the application.
8. Open the Swagger URL displayed by the application to explore and test the API endpoints.

## 6. Automated Tests

The project includes automated tests using xUnit. The tests cover password verification and selected API operations, including user registration and authentication-related behaviour.

To run the tests from the solution directory, use:

```bash
dotnet test
```

## 7. Continuous Integration

GitHub Actions is configured to restore dependencies, build the solution and run automated tests when changes are pushed to the repository or pull requests are created.

## 8. Project Structure

- `RaceDay.Api` – contains the Web API, models, controllers, database context and configuration.
- `RaceDay.Tests` – contains automated tests.
- `docs` – contains project documentation and database-related deliverables.
- `.github/workflows` – contains the GitHub Actions workflow.

## 9. Project Purpose

The purpose of RaceDay is to provide a centralised system for managing race events and participant information through a Web API. It also demonstrates the use of database integration, authentication, automated testing and continuous integration in an ASP.NET Core application.
