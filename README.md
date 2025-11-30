# CrewOps

A field crew management system built with .NET 8 Web API. Manage crews, jobs, assignments, and track working hours.

## Tech Stack

- **.NET 8** with ASP.NET Core
- **Minimal APIs** pattern
- **Entity Framework Core 8** (ORM)
- **SQLite** database
- **Swagger/OpenAPI** for API documentation

## Features

- Crew member management (CRUD)
- Job management with status tracking
- Assign crew members to jobs with roles
- Time tracking with clock in/out functionality
- Work history and duration tracking

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
| POST | `/api/jobs/{id}/assign` | Assign a crew member to a job |
| GET | `/api/jobs/{id}/crew` | Get crew members assigned to a job |

### Time Tracking

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/time/clockin` | Clock in to a job |
| POST | `/api/time/clockout` | Clock out from current shift |
| GET | `/api/time/active/{crewMemberId}` | Check if crew member is clocked in |
| GET | `/api/time/history/{crewMemberId}` | Get time entry history |

## Project Structure

```
CrewOps/
├── CrewOps.sln
├── README.md
└── src/
    └── CrewOps.API/
        ├── Program.cs
        ├── Data/
        │   └── CrewOpsDbContext.cs
        ├── Endpoints/
        │   ├── CrewMemberEndpoints.cs
        │   ├── JobEndpoints.cs
        │   └── TimeEntryEndpoints.cs
        ├── Models/
        │   ├── CrewMember.cs
        │   ├── Job.cs
        │   ├── JobStatus.cs
        │   ├── JobAssignment.cs
        │   └── TimeEntry.cs
        └── Migrations/
```

## Data Models

### CrewMember
- Id, FirstName, LastName, Email, Status, CreatedAt

### Job
- Id, ReferenceNumber, Description, Location, Status, StartDate, EndDate, CreatedAt
- Status: Pending, Scheduled, InProgress, Completed, Canceled

### JobAssignment
- JobId, CrewMemberId, Role, AssignedOn

### TimeEntry
- Id, CrewMemberId, JobId, ClockInTime, ClockOutTime, Notes

## Contributing

This repository uses branch protection. All changes must go through a pull request:

1. Create a feature branch: `git checkout -b feature/your-feature`
2. Make your changes and commit
3. Push and create a pull request
4. Get approval before merging

## License

MIT
