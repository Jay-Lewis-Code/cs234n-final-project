# CS234 C# Advanced Final Project
# Jason Lewis
#
#
# Objective:
#     Research the existing capstone project "BrewTrack" created from a previous group of LCC students.
#     
#     For a single major feature of the project:
#        Create a set of user stories and a professional use-case document (following the Agile methodology)
#        Map out and simplify the existing project database retaining only what is needed for that feature.
#        Create a front end web page for that feature & preform RESTful API CRUD operations. Include unit testing and validate each endpoint via Postman or Swagger.
#
# Submission of documentation for completed project:
[Word Document: Restful API Screnshots](https://citstudent.lanecc.edu/~lewisj628/CS234N/jaylewis-final-restful-api.docx)
[PNG Screenshot: UX/UI Mockup](https://citstudent.lanecc.edu/~lewisj628/CS234N/jaylewis-final-project-ux-ui-mockup.png)
[Word Document: Use Case Document (Schedule Brew)](https://citstudent.lanecc.edu/~lewisj628/CS234N/jaylewis-final-project-use-case-schedule-brew.docx)
[Word Document: Entity Framework UnitTests Screenshots](https://citstudent.lanecc.edu/~lewisj628/CS234N/jaylewis-final-entity-framework-unit-tests.docx)
[PNG Screenshot: Trimmed Database Model](https://citstudent.lanecc.edu/~lewisj628/CS234N/brew-scheduler-trimmed-db-model.png)

# BrewTrack - Brewery Tracker Application

A brewery management and brew schedule tracking application built with ASP.NET Core and Entity Framework Core, backed by a MySQL database.

## Overview

BrewTrack helps breweries manage their brewing operations including:

- **Scheduled Brews** – Plan and track batch schedules
- **Recipes** – Manage beer recipes with ingredients and styles
- **Inventory** – Track ingredient inventory and product stock
- **Reports** – View brewing and inventory reports

## Project Structure

| Project | Description |
|---|---|
| `brewery-tracker-app/` | ASP.NET Core Web API with static frontend |
| `brew-schedule-data/` | Data access layer (EF Core models & DbContext) |
| `BrewScheduleDataTests/` | NUnit integration tests |

## Requirements

### Runtime

- **.NET 8.0 SDK** (or later) – [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)

### NuGet Packages

The following top-level packages are required (installed via NuGet):

| Package | Version |
|---|---|
| Microsoft.EntityFrameworkCore.InMemory | 8.0.22 |
| Microsoft.EntityFrameworkCore.Tools | 8.0.22 |
| Microsoft.Extensions.Configuration | 8.0.0 |
| Microsoft.Extensions.Configuration.FileExtensions | 8.0.1 |
| Microsoft.Extensions.Configuration.Json | 8.0.1 |
| MySql.EntityFrameworkCore | 9.0.0 |
| Pomelo.EntityFrameworkCore.MySql | 8.0.2 |
| Swashbuckle.AspNetCore | 6.6.2 |

> Packages will be restored automatically when building the solution. Run `dotnet restore` if needed.

### Database Setup

This project requires a **local MySQL server** instance.
# Scripts to create the BITS database and update the data are located here:
[BITS MySQL Database Create from Export:](https://citstudent.lanecc.edu/~lewisj628/CS234N/bitsDataFromExport1106.sql)
[BITS MySQL Database Data from Export:](https://citstudent.lanecc.edu/~lewisj628/CS234N/bitsCreateFromExport1106.sql)

1. **Install MySQL** – Ensure you have a MySQL server running locally (e.g., MySQL Community Server, XAMPP, or Docker).

2. **Create the database** – Create a database for the application (the schema is mapped to tables defined in `BitsContext`).

3. **Configure the connection string** – Add your MySQL credentials to the `mySqlSettings.json` file located in the `brew-schedule-data/` directory:

   ```json
   {
     "ConnectionStrings": {
       "mySql": "Server=localhost;Port=3306;Database=YOUR_DATABASE_NAME;User=YOUR_USERNAME;Password=YOUR_PASSWORD;"
     }
   }
   ```

   Replace:
   - `YOUR_DATABASE_NAME` with the name of your database
   - `YOUR_USERNAME` with your MySQL username
   - `YOUR_PASSWORD` with your MySQL password

> ⚠️ **Important:** The `mySqlSettings.json` file contains sensitive credentials and should **not** be committed to source control. Ensure it is listed in `.gitignore`.

## Getting Started

### 1. Clone the repository

```sh
git clone https://github.com/YOUR_USERNAME/brewery-tracker-app.git
cd brewery-tracker-app
```

### 2. Restore packages

```sh
dotnet restore brewery-tracker-app/brewery-tracker-app.sln
```

### 3. Build the solution

```sh
dotnet build brewery-tracker-app/brewery-tracker-app.sln
```

### 4. Run the application

```sh
dotnet run --project brewery-tracker-app/brewery-tracker-app.csproj
```

The API will be available at `https://localhost:<port>`. Swagger UI is enabled in development mode at `/swagger`.

### 5. Run tests

```sh
dotnet test BrewScheduleDataTests/BrewScheduleDataTests.csproj
```

> **Note:** Tests call a stored procedure `reset_bits_for_brew_tests()` to reset the database state before each test. Ensure this procedure exists in your database.

## Tech Stack

- **Backend:** ASP.NET Core 8.0 Web API
- **ORM:** Entity Framework Core 8.0 with Pomelo MySQL provider
- **Database:** MySQL
- **Frontend:** Static HTML/CSS served via `wwwroot/` (Tailwind CSS)
- **Testing:** NUnit 3
- **API Docs:** Swagger / Swashbuckle