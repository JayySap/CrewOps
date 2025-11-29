# CrewOps

A field crew management system built with .NET 8 Web API.

## Tech Stack

- **.NET 8** with ASP.NET Core
- **Minimal APIs** pattern
- **Entity Framework Core 8** (ORM)
- **SQLite** database
- **Swagger/OpenAPI** for API documentation

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

### Running the API

```bash
cd src/CrewOps.API
dotnet run
```

The API will start at `http://localhost:5152`. Open `/swagger` for the interactive API documentation.

### Database Setup

The project uses SQLite with EF Core migrations. To set up or update the database:

```bash
cd src/CrewOps.API
dotnet ef database update
```

To create a new migration after model changes:

```bash
dotnet ef migrations add <MigrationName>
```

## API Endpoints

### CrewMembers

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/crewmembers` | Get all crew members |
| GET | `/api/crewmembers/{id}` | Get a crew member by ID |
| POST | `/api/crewmembers` | Create a new crew member |
| PUT | `/api/crewmembers/{id}` | Update a crew member |
| DELETE | `/api/crewmembers/{id}` | Delete a crew member |

### Jobs

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/jobs` | Get all jobs |
| GET | `/api/jobs/{id}` | Get a job by ID |
| POST | `/api/jobs` | Create a new job |
| PUT | `/api/jobs/{id}` | Update a job |
| DELETE | `/api/jobs/{id}` | Delete a job |

## Project Structure

```bash

CrewOps/
├── CrewOps.sln
└── src/
    └── CrewOps.API/
        ├── Program.cs           # Entry point and endpoint registration
        ├── Data/
        │   └── CrewOpsDbContext.cs
        ├── Endpoints/
        │   ├── CrewMemberEndpoints.cs
        │   └── JobEndpoints.cs
        ├── Models/
        │   ├── CrewMember.cs
        │   ├── Job.cs
        │   └── JobStatus.cs
        └── Migrations/
```

## License

MIT
