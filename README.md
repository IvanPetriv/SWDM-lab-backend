# University Project: Laboratory Work on XP Methodology

## Description
This project is a laboratory work designed to learn and apply **Extreme Programming (XP)** methodology. It demonstrates the implementation of a course management system following modern software architecture practices.

## Features
The system provides the following functionality:

- **User Management**
  - Registration and authentication for students, teachers, and administrators.
  - Administrator panel for managing users.
- **Course Management**
  - Create, edit, and delete courses.
  - Browse and search available courses.
- **Educational Materials**
  - Add learning materials including files, links, and videos.

## Project Structure
The project is structured according to **Clean Architecture** principles and divided into the following layers:

- **Domain**
  - Contains core entities and business rules.
- **Application**
  - Implements application-specific business logic.
  - References **Domain**.
- **ApplicationTests**
  - xUnit project for testing **Application** logic.
  - References **Domain** and **Application**.
- **EFCore**
  - Infrastructure layer handling database operations via Entity Framework Core.
  - References **Domain** and **Application**.
- **Api**
  - ASP.NET presentation layer exposing endpoints.
  - References **Domain**, **Application**, and **EFCore**.

## Technologies Used
- .NET 9 / C#
- ASP.NET Core Web API
- Entity Framework Core
- xUnit for testing
- PostgreSQL

## Getting Started
### Prerequisites
- .NET SDK installed
- Visual Studio IDE
- Database server (PostgreSQL)

### Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/IvanPetriv/SWDM-lab-frontend.git
   ```

2. Navigate to the Api project:
	```
	cd Api
	```

3. Configure the database connection string in 	`appsettings.json`.

4. Run database migrations:
	```
	dotnet ef database update --project ../EFCore
	```

5. Run the API:
	```
	dotnet run
	```

## Running Tests

Navigate to the `ApplicationTests` project and run:
```
dotnet test
```
## Notes

The project follows Clean Architecture to separate concerns and make the code maintainable.

This work focuses on XP methodology principles, including iterative development and continuous testing.
