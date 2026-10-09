# CivicFix — Development Instructions

## Project

CivicFix is a production-style full-stack civic issue reporting and resolution platform.

The application allows users to report real-world community problems such as:

- Garbage
- Broken roads
- Street lights
- Water leakage
- Drainage
- Damaged public infrastructure
- Illegal dumping
- Traffic/signage problems

Users can report an issue with a description, images and geographical location.

Administrators can verify issues, assign them to departments/staff, change status and monitor resolution through an analytics dashboard.

---

## Technology Stack

### Frontend

- React
- TypeScript
- Vite
- Tailwind CSS
- React Router
- Axios
- TanStack Query
- React Hook Form
- Zod
- Leaflet/OpenStreetMap
- Lucide React

### Backend

- C#
- ASP.NET Core Web API
- Entity Framework Core
- ASP.NET Core Identity
- JWT authentication
- SignalR
- FluentValidation where appropriate

### Database

- SQL Server

### External Services

- Cloudinary for image storage
- Gemini/OpenAI for AI-assisted issue categorization
- OpenStreetMap/Leaflet for maps
- Email provider for notifications

### Testing

- xUnit
- Moq
- FluentAssertions
- Vitest
- React Testing Library

### DevOps

- Docker
- Docker Compose
- GitHub Actions

---

## Architecture

Use a clean and maintainable architecture.

Backend:

CivicFix.API
CivicFix.Application
CivicFix.Domain
CivicFix.Infrastructure
CivicFix.Tests

Frontend:

civicfix-client/
├── src/
│   ├── components/
│   ├── pages/
│   ├── layouts/
│   ├── hooks/
│   ├── services/
│   ├── api/
│   ├── types/
│   ├── utils/
│   ├── contexts/
│   └── assets/

---

## Backend Rules

Use:

- RESTful API design
- DTOs instead of exposing EF entities directly
- Dependency injection
- async/await
- proper HTTP status codes
- centralized exception handling
- validation
- structured logging
- pagination
- filtering
- sorting
- authorization policies
- secure password handling
- JWT authentication
- refresh-token strategy
- audit logging

Do not put business logic directly inside controllers.

Controllers should call application services.

---

## Roles

Implement:

USER
STAFF
DEPARTMENT_ADMIN
SUPER_ADMIN

Authorization must be enforced on the backend.

Never rely only on frontend route protection.

---

## Core Entities

Users
Roles
Issues
IssueImages
Categories
Departments
Staff
IssueAssignments
Comments
Votes
Notifications
AuditLogs

---

## Issue Status

Use:

PENDING
VERIFIED
ASSIGNED
IN_PROGRESS
RESOLVED
REJECTED
CLOSED

---

## Issue Priority

LOW
MEDIUM
HIGH
CRITICAL

---

## Core Features

### User

- Register
- Login
- Email verification
- Forgot password
- Profile
- Create issue
- Upload images
- Select location
- View own issues
- Track issue status
- Comment
- Vote/support issue
- Receive notifications
- Verify resolution
- Submit feedback

### Admin

- Dashboard
- Manage issues
- Verify/reject issues
- Assign department
- Assign staff
- Change priority
- Change status
- Manage users
- Manage departments
- Manage categories
- View analytics
- View audit logs

### Staff

- View assigned issues
- Update progress
- Add comments
- Upload resolution evidence
- Mark issue as resolved

---

## AI Features

AI should assist the system rather than control it.

When a user uploads an issue:

AI may suggest:

- category
- severity
- department
- short summary

Example:

{
  "category": "Garbage",
  "severity": "HIGH",
  "department": "Sanitation",
  "confidence": 0.91
}

The user/admin must be able to override AI suggestions.

Never store API keys in source code.

Use environment variables.

---

## Duplicate Detection

When a user creates an issue:

1. Search nearby issues.
2. Compare category.
3. Compare geographical distance.
4. Compare text similarity.
5. Show possible duplicates.

The user should be able to support an existing issue instead of creating another one.

---

## Real-Time Updates

Use SignalR.

When an issue status changes:

User receives a real-time notification.

Example:

Issue #CF1024

PENDING
↓
VERIFIED
↓
ASSIGNED
↓
IN_PROGRESS
↓
RESOLVED
↓
CLOSED

---

## Security

Implement:

- password hashing
- JWT
- refresh tokens
- role-based authorization
- validation
- rate limiting
- secure CORS
- file validation
- file size limits
- SQL injection protection
- audit logs
- secure environment variables

Never expose secrets in frontend code or Git.

---

## Frontend Design

The UI must look like a real production SaaS application.

Do not make it look like an AI-generated template.

Use:

- clean typography
- consistent spacing
- responsive layout
- accessible components
- loading states
- skeleton states
- empty states
- error states
- confirmation dialogs
- toast notifications
- responsive tables
- professional dashboard cards

Avoid excessive gradients, excessive animations and unnecessary glassmorphism.

---

## Development Rules

Before implementing a feature:

1. Understand the existing architecture.
2. Inspect existing files.
3. Reuse existing components.
4. Do not duplicate code.
5. Keep modules small.
6. Update types when API contracts change.
7. Add validation.
8. Add tests for important business logic.
9. Run the application after significant changes.
10. Fix errors before moving to the next feature.

Never rewrite working code unnecessarily.

Never install a dependency without checking whether an existing dependency already provides the required functionality.

---

## Git Strategy

Use meaningful commits:

feat:
fix:
refactor:
test:
docs:
chore:

Example:

feat(auth): implement JWT authentication

feat(issues): add issue creation API

feat(admin): add issue assignment workflow

test(issues): add issue service tests

---

## Definition of Done

A feature is complete only when:

- frontend works
- backend works
- database integration works
- validation works
- authorization works
- errors are handled
- important tests exist
- UI has loading/error/empty states
- no secrets are committed
- code is formatted
- application builds successfully