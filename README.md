 # TT Project - .NET Core API Implementation
         3 +  
         4 +  A comprehensive .NET Core 3.1 Web API project demonstrating three distinct database access patterns        
     and
           +  Entity Framework migrations. This project showcases raw SQL connections, database views, stored 
           + procedures, and dynamic property loading with real-time database reflection.
         5 +  
         6 +  ## 🎯 Project Overview
         7 +  
         8 +  This project implements three key tasks:
         9 +  
        10 +  - **Task 1**: DataController with raw SQL connections (no Entity Framework)
        11 +  - **Task 2**: Entity Framework migration to add Type field to ProductProperties
        12 +  - **Task 3**: ExportController with dynamic property loading and nested JSON generation
        13 +  
        14 +  ## 🚀 Getting Started
        15 +  
        16 +  ### Prerequisites
        17 +  - .NET Core 3.1 SDK
        18 +  - SQL Server (LocalDB or full instance)
        19 +  - Git
        20 +  
        21 +  ### Installation
        22 +  
        23 +  1. Clone the repository:
        24 +     ```bash
        25 +     git clone https://github.com/PavelVitov/tt.git
        26 +     cd tt
        27 +     ```
        28 +  
        29 +  2. Restore packages:
        30 +     ```bash
        31 +     dotnet restore
        32 +     ```
        33 +  
        34 +  3. Update database connection string in `TT.Api/appsettings.json`:
        35 +     ```json
        36 +     "ConnectionStrings": {
        37 +       "TTDbContext": "Server=localhost;Database=tt;Integrated 
           + Security=true;TrustServerCertificate=true;"
        38 +     }
        39 +     ```
        40 +  
        41 +  4. Apply database migrations:
        42 +     ```bash
        43 +     dotnet ef database update --project TT.Lib --startup-project TT.Api
        44 +     ```
        45 +  
        46 +  5. Run the application:
        47 +     ```bash
        48 +     cd TT.Api
        49 +     dotnet run --urls "https://localhost:5003"
        50 +     ```
        51 +  
        52 +  ## 📋 API Endpoints
        53 +  
        54 +  ### Task 1 - DataController (Raw SQL)
        55 +  - `GET /data/test-connection` - Database connection verification
        56 +  - `GET /data/products` - Load products using raw SQL queries
        57 +  - `GET /data/properties` - Query database view (vw_Properties)
        58 +  - `GET /data/schema-check` - Database schema verification
        59 +  
        60 +  ### Task 3 - ExportController (Entity Framework)
        61 +  - `GET /export/brand/{key}` - Export product with dynamic properties
        62 +  - `GET /export/products` - List available products for export
        63 +  - `GET /export/find-properties` - Property discovery helper
        64 +  
        65 +  ### Example Usage
        66 +  ```bash
        67 +  # Test database connection
        68 +  curl -k "https://localhost:5003/data/test-connection"
        69 +  
        70 +  # Export product with dynamic properties
        71 +  curl -k "https://localhost:5003/export/brand/SM2L"
        72 +  
        73 +  # Verify Type field migration
        74 +  curl -k "https://localhost:5003/data/schema-check"
        75 +  ```
        76 +  
        77 +  ## 🧪 Build and Test
        78 +  
        79 +  ### Build the project:
        80 +  ```bash
        81 +  dotnet build
        82 +  ```
        83 +  
        84 +  ### Run unit tests:
        85 +  ```bash
        86 +  dotnet test TT.Api.Tests --verbosity normal
        87 +  ```
        88 +  
        89 +  **Expected Result**: 17/17 tests passing
        90 +  - 8 DataController tests (routing, attributes, structure)
        91 +  - 9 ExportController tests (routing, attributes, structure)
        92 +  
        93 +  ## 🏗️ Technical Implementation
        94 +  
        95 +  ### Task 1: Raw SQL Approach
        96 +  - Uses `SqlConnection` and `SqlCommand` directly
        97 +  - No Entity Framework dependencies
        98 +  - Demonstrates stored procedure usage and view querying
        99 +  - Includes comprehensive error handling and connection management
       100 +  
       101 +  ### Task 2: Entity Framework Migration
       102 +  - Adds `Type` field to `ProductProperties` table
       103 +  - Migration: `20250826155154_AddTypeToProductProperties`
       104 +  - Maintains database integrity and relationships
       105 +  
       106 +  ### Task 3: Dynamic Property Loading
       107 +  - Real-time database reflection
       108 +  - Recursive property tree building
       109 +  - Complex nested JSON generation
       110 +  - Uses Entity Framework with Include for optimized queries
       111 +  
       112 +  ## 📊 Project Structure
       113 +  
       114 +  ```
       115 +  TT/
       116 +  ├── TT.Api/              # Web API project
       117 +  │   ├── Controllers/     # API controllers
       118 +  │   └── appsettings.json # Configuration
       119 +  ├── TT.Lib/              # Data access library
       120 +  │   ├── Entities/        # Entity models
       121 +  │   └── Migrations/      # EF migrations
       122 +  └── TT.Api.Tests/        # Unit tests
       123 +  ```
       124 +  
       125 +  ## 🔧 Key Features
       126 +  
       127 +  - **Dual Database Approach**: Raw SQL vs Entity Framework comparison
       128 +  - **Real-time Data Reflection**: Dynamic property loading from database
       129 +  - **Professional Code Organization**: Clean separation of concerns with detailed comments
       130 +  - **Comprehensive Testing**: Full unit test coverage for controllers
       131 +  - **Migration Management**: Proper Entity Framework migration workflow
       132 +  
       133 +  ---
       134 + 