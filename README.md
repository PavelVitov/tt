# TT Project - .NET Core API Implementation

A comprehensive .NET Core 3.1 Web API project demonstrating three distinct database access patterns and Entity Framework migrations. This project showcases raw SQL connections, database views, stored procedures, and dynamic property loading with real-time database reflection.

## Project Overview

This project implements three key tasks:

- **Task 1**: DataController with raw SQL connections (no Entity Framework)
- **Task 2**: Entity Framework migration to add Type field to ProductProperties
- **Task 3**: ExportController with dynamic property loading and nested JSON generation

## Getting Started

### Prerequisites
- .NET Core 3.1 SDK
- SQL Server (LocalDB or full instance)
- Git

### Installation

1. Clone the repository:
   ```
   git clone https://github.com/PavelVitov/tt.git
   cd tt
   ```

2. Restore packages:
   ```
   dotnet restore
   ```

3. Update database connection string in `TT.Api/appsettings.json`:
   ```
   "ConnectionStrings": {
     "TTDbContext": "Server=localhost;Database=tt;Integrated Security=true;TrustServerCertificate=true;"
   }
   ```

4. Apply database migrations:
   ```
   dotnet ef database update --project TT.Lib --startup-project TT.Api
   ```

5. Run the application:
   ```
   cd TT.Api
   dotnet run --urls "https://localhost:5003"
   ```

## API Endpoints

### Task 1 - DataController (Raw SQL)
- `GET /data/test-connection` - Database connection verification
- `GET /data/products` - Load products using raw SQL queries
- `GET /data/properties` - Query database view (vw_Properties)
- `GET /data/schema-check` - Database schema verification

### Task 3 - ExportController (Entity Framework)
- `GET /export/brand/{key}` - Export product with dynamic properties
- `GET /export/products` - List available products for export
- `GET /export/find-properties` - Property discovery helper

### Example Usage
```
# Test database connection
curl -k "https://localhost:5003/data/test-connection"

# Export product with dynamic properties
curl -k "https://localhost:5003/export/brand/SM2L"

# Verify Type field migration
curl -k "https://localhost:5003/data/schema-check"
```

## Build and Test

### Build the project:
```
dotnet build
```

### Run unit tests:
```
dotnet test TT.Api.Tests --verbosity normal
```

**Expected Result**: 17/17 tests passing
- 8 DataController tests (routing, attributes, structure)
- 9 ExportController tests (routing, attributes, structure)

## Technical Implementation

### Task 1: Raw SQL Approach
- Uses `SqlConnection` and `SqlCommand` directly
- No Entity Framework dependencies
- Demonstrates stored procedure usage and view querying
- Includes comprehensive error handling and connection management

### Task 2: Entity Framework Migration
- Adds `Type` field to `ProductProperties` table
- Migration: `20250826155154_AddTypeToProductProperties`
- Maintains database integrity and relationships

### Task 3: Dynamic Property Loading
- Real-time database reflection
- Recursive property tree building
- Complex nested JSON generation
- Uses Entity Framework with Include for optimized queries

## Project Structure

```
TT/
├── TT.Api/              # Web API project
│   ├── Controllers/     # API controllers
│   └── appsettings.json # Configuration
├── TT.Lib/              # Data access library
│   ├── Entities/        # Entity models
│   └── Migrations/      # EF migrations
└── TT.Api.Tests/        # Unit tests
```

## Key Features

- **Dual Database Approach**: Raw SQL vs Entity Framework comparison
- **Real-time Data Reflection**: Dynamic property loading from database
- **Professional Code Organization**: Clean separation of concerns with detailed comments
- **Comprehensive Testing**: Full unit test coverage for controllers
- **Migration Management**: Proper Entity Framework migration workflow

---
