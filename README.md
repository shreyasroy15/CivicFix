# CivicFix

CivicFix is a modern web application for tracking and resolving civic issues.

## Architecture

The project is structured into a React/TypeScript frontend (`civicfix-client`) and an ASP.NET Core backend.

### Backend (.NET 10.0)
- **CivicFix.API**: ASP.NET Core Web API, configuring DI, CORS, Global Exceptions, Swagger, and Controllers.
- **CivicFix.Application**: Application logic and interfaces.
- **CivicFix.Domain**: Core domain entities.
- **CivicFix.Infrastructure**: Entity Framework Core implementation and database configuration.
- **CivicFix.Tests**: xUnit project for testing the backend.

### Frontend (React + Vite + TypeScript)
- Built with standard modern practices: React Router DOM, React Query, React Hook Form, Zod, and TailwindCSS.

## Getting Started

### Prerequisites
- Node.js (v18+)
- .NET 10.0 SDK
- SQL Server (or LocalDB)

### Run Backend
```bash
cd CivicFix.API
dotnet run
```
API Documentation (Swagger) is available at `https://localhost:<port>/swagger` in development.

### Run Frontend
```bash
cd civicfix-client
npm install
npm run dev
```

## Security
Secrets should be managed via environment variables and User Secrets in development, not hardcoded.
