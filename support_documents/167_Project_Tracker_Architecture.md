# Project Tracker Architecture

## Separation of Concerns
Per executive orders, `ClinicProjectTracker` is built as an entirely separated Visual Studio Solution (`ClinicProjectTracker.sln`) disconnected from the main `Clinic.Web.sln` to prevent contamination of the production source code with PMO monitoring overhead.

## Tech Stack
- **Framework:** ASP.NET Core MVC (Latest)
- **UI Framework:** Bootstrap 5.3
- **Data Tables:** jQuery DataTables (v1.13)
- **Visualization:** Chart.js (for Gantt, Pie, Burndown, Velocity, and Progress indicators)

## Module Layout
- `Controllers/ProjectControllers.cs`: Contains routing for all 10 Modules (Dashboard, Wbs, Sprint, Timeline, Risk, Issue, Milestone, Report).
- `Services/ProjectDataService.cs`: In-memory mock engine providing generated datasets (WBS Trees, Sprint math, Risk counters) to simulate a live database since a backend is out-of-scope for this rapid dashboard.
- `Models/ProjectModels.cs`: Schema definitions for PMO entities.
- `Views/Shared/_Layout.cshtml`: Global responsive PMO sidebar layout with Dark/Light theme hooks.
