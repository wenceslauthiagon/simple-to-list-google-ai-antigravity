# To-Do List Application - Clean Architecture & React Full-Stack

[![.NET 8](https://img.shields.io/badge/.NET_8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet)
[![MongoDB](https://img.shields.io/badge/MongoDB-47A248?style=for-the-badge&logo=mongodb&logoColor=white)](https://www.mongodb.com/)
[![React](https://img.shields.io/badge/React_18-61DAFB?style=for-the-badge&logo=react&logoColor=black)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript_5-3178C6?style=for-the-badge&logo=typescript&logoColor=white)](https://www.typescriptlang.org/)
[![Vite](https://img.shields.io/badge/Vite_5-646CFF?style=for-the-badge&logo=vite&logoColor=white)](https://vitejs.dev/)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![xUnit](https://img.shields.io/badge/xUnit-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://xunit.net/)
[![ESLint](https://img.shields.io/badge/ESLint-4B32C3?style=for-the-badge&logo=eslint&logoColor=white)](https://eslint.org/)

Professional, full-stack task management application engineered with **ASP.NET Core Web API (.NET 8)**, **MongoDB**, and **React + TypeScript (Vite)**, strictly adhering to **Clean Architecture**, **Domain-Driven Design (DDD)**, and **SOLID** principles.

> 💼 **Production-Grade Architecture** | Zero UI/State Library Dependencies | Invariant-Protected Domain | 100% In-Memory Testable

---

## 🌟 Implementation Highlights

### ✅ Enterprise Requirements & Architectural Patterns

- 🏛️ **Clean Architecture (4 Decoupled Layers)** - Domain depends on nothing; Application depends only on Domain; Infrastructure and Api depend inward.
- 🎯 **Rich Invariant-Safe Domain Model** - Business rules (`AdvanceStatus`, `ChangePriority`, `ChangeTitle`, title trimming, bounds check) live strictly inside the `TodoTask` entity.
- 🔄 **Deterministic Status Lifecycle** - Advancing status cycles deterministically (`pending` &rarr; `inProgress` &rarr; `completed` &rarr; `pending`) in the backend domain; frontend holds zero business logic.
- 💾 **Persistence Decoupling** - Separate MongoDB persistence model (`TaskDocument`) mapped cleanly to and from the domain entity, isolating BSON attributes from domain invariants.
- ⏱️ **Modern TimeProvider Abstraction** - Uses .NET 8 `System.TimeProvider` across application and domain services, allowing deterministic, instant time control in test suites.
- 🛡️ **RFC 7807 ProblemDetails Global Error Handling** - Handled via `IExceptionHandler` returning standardized status codes (400, 404, 500) without leaking stack traces.
- 🔍 **Safe Debounced Search & Compound Indexes** - 300ms client debouncing paired with regex-injection protection (`Regex.Escape`) and MongoDB compound indexing (`PriorityLevel` DESC, `CreatedAt` DESC).
- 📊 **Dynamic Progress Tracking with Zero-Division Guard** - Dynamic completion ratio and percentage (e.g. `"3 of 8 completed, 38%"`), guaranteed to return 0% safely on empty lists.
- 🎨 **Pure Accessible CSS Architecture** - Built with custom CSS custom properties (`:root`), visible focus rings, non-color-dependent priority badges (color + label), and zero third-party UI component bloat.
- 🧪 **Comprehensive Automated Testing** - 36 xUnit tests (100% pass) utilizing an in-memory repository fake with zero external mocking libraries, plus Vitest utility tests.

---

## 📋 About the Project

This To-Do List system demonstrates how small domain features should be engineered in enterprise environments:

- ✅ **Clean Architecture** with inward-pointing dependencies
- ✅ **Clean Code** with intention-revealing names and no duplication
- ✅ **SOLID Principles** applied concretely across all layers
- ✅ **Automated Testing** with 36 xUnit tests and 3 Vitest tests (100% passing)
- ✅ **Containerization Ready** with Docker Compose for local MongoDB 7.0
- ✅ **Zero Hardcoding** of connection strings, limits, routes, or UI strings
- ✅ **Strict Type Safety** (`<Nullable>enable</Nullable>`, `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`, `tsconfig strict`)

---

## 🚀 Technologies

### Core
- **Backend API**: ASP.NET Core Web API (.NET 8 LTS)
- **Language**: C# 12
- **Database**: MongoDB 7.0 via official `MongoDB.Driver` (2.28.0)
- **Frontend**: React 18.3 + TypeScript 5.5
- **Tooling & Bundler**: Vite 5.4
- **Documentation**: Swagger UI / OpenAPI via `Swashbuckle.AspNetCore` (6.6.2)

### Architecture & Patterns
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, Presentation/Api)
- **Design Pattern**: Domain-Driven Design (DDD), Repository Pattern, Options Pattern
- **Testing**: xUnit 2.9 (Backend) + Vitest 2.0 (Frontend)
- **Code Quality**: `.editorconfig` (C#), ESLint 9 (Flat config) + Prettier (TypeScript)

### Infrastructure & Runtime
- **Containerization**: Docker Compose (`mongo:7.0`)
- **Health Checks**: ASP.NET Core Health Checks (`/health`)
- **Environment**: Cross-platform (Windows, Linux, macOS)

---

## 📋 Functional Features

### ✅ Task Creation & Validation
- Title length constrained between 1 and 200 characters (trimmed)
- Priority selection: `Low`, `Medium` (default), or `High`
- Immediate format validation in DTOs and invariant protection in Domain

### ✅ Deterministic Status Cycle & Selection
- Statuses: `Pending`, `In Progress`, `Completed`
- **Quick Cycle Button**: Clicking the circular icon advances status:
  $$\text{Pending} \longrightarrow \text{In Progress} \longrightarrow \text{Completed} \longrightarrow \text{Pending}$$
- **Direct Selector**: Status can also be updated directly from the task card dropdown

### ✅ Task Editing
- Inline editing of title and priority
- Keyboard shortcuts: `Enter` to save, `Escape` to cancel
- Real-time client-side and backend length validation

### ✅ Status Filters & Real-Time Counters
- Four status views: `All`, `Pending`, `In Progress`, `Completed`
- Active numerical badge counter on every filter pill
- Real-time synchronization with current task dataset

### ✅ Debounced Real-Time Search
- Client-side 300ms debounce hook (`useDebounce`)
- Case-insensitive backend search escaping special regex characters (no regex injection)

### ✅ Dynamic Progress Tracking
- Live calculation of total vs completed tasks
- Formatted summary string: `"{completed} of {total} completed, {percentage}%"`
- Guard clause preventing division-by-zero on empty task lists (returns 0%)

### ✅ Task Deletion & Batch Clear
- Individual deletion via accessible trash action
- Batch deletion button (`Clear completed`) removing all completed tasks in one operation

### ✅ Accessible Semantic Styling
- Completed tasks display with strikethrough text and softer opacity
- Priority badges combine distinct background colors **and** textual labels (WCAG compliant)
- Fully accessible via keyboard with visible focus rings (`:focus-visible`) and ARIA labels

---

## 🏗️ Solution Architecture

### Clean Architecture - 4 Layers

```
┌─────────────────────────────────────────────────────────────────┐
│                    1-API (Controllers & Root)                   │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │             2-INFRASTRUCTURE (Mongo Persistence)          │  │
│  │  ┌─────────────────────────────────────────────────────┐  │  │
│  │  │           3-APPLICATION (Services & DTOs)           │  │  │
│  │  │  ┌───────────────────────────────────────────────┐  │  │  │
│  │  │  │            4-DOMAIN (Entities & Rules)        │  │  │  │
│  │  │  └───────────────────────────────────────────────┘  │  │  │
│  │  └─────────────────────────────────────────────────────┘  │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

#### Layer Responsibilities

**1-Api (Presentation & Composition Root)**
- Thin `TasksController` mapping HTTP requests to `ITaskService`
- Global error handling middleware via `IExceptionHandler` producing `ProblemDetails`
- Swagger UI generation (enabled in Development)
- Configurable CORS policy and `/health` probe

**2-Infrastructure (Data Access & Frameworks)**
- `MongoTaskRepository` implementing `ITaskRepository`
- `TaskDocument` persistence schema separate from domain entities
- `TaskDocumentMapper` handling conversions and enum mappings
- Compound index definitions (`PriorityLevel` DESC, `CreatedAt` DESC)
- Options pattern configuration via `MongoDbSettings`

**3-Application (Use Cases & Contracts)**
- `TaskService` orchestrating business operations
- `ITaskRepository` port abstracting storage
- In/Out DTOs (`CreateTaskRequest`, `UpdateTaskRequest`, `TaskResponse`, `TaskSummaryResponse`)
- Application exceptions (`TaskNotFoundException`, `InvalidTaskIdException`, `ValidationException`)
- Dependency injection via `TimeProvider`

**4-Domain (Core Business Logic)**
- `TodoTask` entity with encapsulated state and validation invariants
- `TaskStatus` (`Pending`, `InProgress`, `Completed`) and `TaskPriority` (`Low`, `Medium`, `High`)
- `TaskConstants` specifying domain limits (1 to 200 characters)
- Zero external package dependencies

---

### Directory Structure

```
simple-to-do-list/
├── .editorconfig                   # C# and web formatting guidelines
├── .env.example                    # Environment variables template
├── .gitignore                      # Git ignore rules (.NET, node_modules, dist)
├── docker-compose.yml              # Local MongoDB 7.0 container configuration
├── README.md                       # Complete solution documentation
├── backend/
│   ├── TodoList.sln                # Visual Studio / .NET solution file
│   ├── TodoList.Domain/            # [Layer 4] Domain invariants, entities, enums
│   │   ├── Constants/              # TaskConstants (title length limits)
│   │   ├── Entities/               # TodoTask entity and Task alias
│   │   ├── Enums/                  # TaskStatus and TaskPriority
│   │   └── Exceptions/             # DomainValidationException
│   ├── TodoList.Application/       # [Layer 3] Application contracts and workflows
│   │   ├── DTOs/                   # Request and response models
│   │   ├── Exceptions/             # Application exceptions
│   │   ├── Interfaces/             # ITaskRepository and ITaskService contracts
│   │   └── Services/               # TaskService workflow implementation
│   ├── TodoList.Infrastructure/    # [Layer 2] MongoDB data access implementation
│   │   ├── Configuration/          # MongoDbSettings Options pattern
│   │   └── Persistence/            # MongoTaskRepository, TaskDocument, Mapper
│   ├── TodoList.Api/               # [Layer 1] ASP.NET Core presentation layer
│   │   ├── Configuration/          # CorsSettings Options pattern
│   │   ├── Controllers/            # Thin TasksController
│   │   ├── Middleware/             # GlobalExceptionHandler (RFC 7807)
│   │   └── Program.cs              # Dependency injection and HTTP pipeline
│   └── TodoList.Tests/             # Unit tests using in-memory fakes
│       ├── Application/            # TaskService test suite
│       ├── Domain/                 # TodoTask invariant and status cycle tests
│       └── Fakes/                  # InMemoryTaskRepository & TestTimeProvider
└── frontend/
    ├── package.json                # Frontend scripts and dependencies
    ├── tsconfig.json               # Strict TypeScript configuration
    ├── vite.config.ts              # Vite and Vitest configuration
    ├── eslint.config.js            # ESLint flat config
    ├── index.html                  # Single Page Application HTML root
    └── src/
        ├── api/                    # Typed fetch API client (taskApi)
        ├── components/             # Single-responsibility React components
        ├── constants/              # Centralized UI strings (STRINGS) and config
        ├── hooks/                  # Custom hooks (useTasks, useDebounce)
        ├── styles/                 # CSS custom properties (:root) & layout styles
        ├── types/                  # TypeScript domain models and DTO interfaces
        └── utils/                  # Formatting helpers and Vitest unit tests
```

---

## 🔄 Request Flow & Sequence

The diagram below illustrates the end-to-end request flow when advancing a task's status:

```mermaid
sequenceDiagram
    autonumber
    actor User as User
    participant UI as React UI (TaskItem)
    participant API as TasksController
    participant SVC as TaskService
    participant ENT as TodoTask (Domain)
    participant DB as MongoTaskRepository

    User->>UI: Clicks status cycle button
    UI->>API: POST /api/tasks/{id}/advance-status
    API->>SVC: AdvanceTaskStatusAsync(id)
    SVC->>DB: GetByIdAsync(id)
    DB-->>SVC: TodoTask entity instance
    SVC->>ENT: task.AdvanceStatus(currentTime)
    Note over ENT: Evaluates invariant cycle:<br/>Pending -> InProgress -> Completed -> Pending
    SVC->>DB: UpdateAsync(task)
    DB-->>SVC: Persisted entity
    SVC-->>API: TaskResponse DTO
    API-->>UI: 200 OK (TaskResponse JSON)
    UI-->>User: Optimistically updates task card & progress bar
```

---

## 📚 API Documentation

### Interactive API Explorer
- **Swagger UI**: `http://localhost:5000/swagger` (available when running in `Development` mode)
- **OpenAPI JSON**: `http://localhost:5000/swagger/v1/swagger.json`
- **Health Check Probe**: `http://localhost:5000/health`

### Endpoints Reference

All requests and responses use **camelCase JSON**. Enums are serialized as strings:
- `TaskStatus`: `"pending"`, `"inProgress"`, `"completed"`
- `TaskPriority`: `"low"`, `"medium"`, `"high"`

| Method | Endpoint | Description | Status Codes |
|:-------|:---------|:------------|:-------------|
| `GET` | `/api/tasks` | List tasks filtered by status and/or search (ordered High priority first, then CreatedAt) | `200 OK` |
| `GET` | `/api/tasks/summary` | Retrieve total, count per status, and progress percentage | `200 OK` |
| `GET` | `/api/tasks/{id}` | Get single task by 24-character hexadecimal ID | `200 OK`, `400 Bad Request`, `404 Not Found` |
| `POST` | `/api/tasks` | Create a new task | `201 Created`, `400 Bad Request` |
| `PATCH` | `/api/tasks/{id}` | Update title, priority, and/or status | `200 OK`, `400 Bad Request`, `404 Not Found` |
| `POST` | `/api/tasks/{id}/advance-status` | Advance status according to the domain lifecycle | `200 OK`, `400 Bad Request`, `404 Not Found` |
| `DELETE` | `/api/tasks/completed` | Batch remove all completed tasks | `200 OK` |
| `DELETE` | `/api/tasks/{id}` | Delete a single task by ID | `204 NoContent`, `400 Bad Request`, `404 Not Found` |
| `GET` | `/health` | Health check probe | `200 OK` |

---

### Request & Response Examples

#### 1. Create Task
```http
POST /api/tasks
Content-Type: application/json

{
  "title": "Review MongoDB replica set indexing",
  "priority": "high"
}
```
**Response (201 Created):**
```json
{
  "id": "672199b0c6a5833055998f41",
  "title": "Review MongoDB replica set indexing",
  "status": "pending",
  "priority": "high",
  "createdAt": "2026-10-30T00:00:00+00:00",
  "updatedAt": null
}
```

#### 2. Get Task Summary
```http
GET /api/tasks/summary
```
**Response (200 OK):**
```json
{
  "total": 8,
  "pending": 3,
  "inProgress": 2,
  "completed": 3,
  "progressPercentage": 38
}
```

#### 3. Advance Status
```http
POST /api/tasks/672199b0c6a5833055998f41/advance-status
```
**Response (200 OK):**
```json
{
  "id": "672199b0c6a5833055998f41",
  "title": "Review MongoDB replica set indexing",
  "status": "inProgress",
  "priority": "high",
  "createdAt": "2026-10-30T00:00:00+00:00",
  "updatedAt": "2026-10-30T00:05:00+00:00"
}
```

#### 4. RFC 7807 ProblemDetails Error Format
When an invalid non-hexadecimal ID is passed (e.g. `invalid-id`):
```json
{
  "type": "about:blank",
  "title": "Invalid Task ID",
  "status": 400,
  "detail": "Task ID must be a 24-character hexadecimal string.",
  "instance": "/api/tasks/invalid-id"
}
```

When a task does not exist (404 Not Found):
```json
{
  "type": "about:blank",
  "title": "Task Not Found",
  "status": 404,
  "detail": "Task with ID '672199b0c6a5833055998f99' was not found.",
  "instance": "/api/tasks/672199b0c6a5833055998f99"
}
```

---

## ⚙️ Configuration Reference

| Setting / Variable | Location / Key | Description | Default | Required |
|:-------------------|:---------------|:------------|:--------|:---------|
| `MongoDbSettings:ConnectionString` | `appsettings.json` / `MONGODB_CONNECTION_STRING` | MongoDB connection URI | `mongodb://localhost:27017` | Yes (in Prod) |
| `MongoDbSettings:DatabaseName` | `appsettings.json` / `MONGODB_DATABASE_NAME` | Database name | `TodoListDb` | No |
| `MongoDbSettings:CollectionName` | `appsettings.json` / `MONGODB_COLLECTION_NAME` | Tasks collection name | `Tasks` | No |
| `Cors:AllowedOrigins` | `appsettings.json` / `CORS_ALLOWED_ORIGINS` | Permitted browser origins | `["http://localhost:5173"]` | No |
| `VITE_API_URL` | Frontend `.env` / Environment | Backend API host URL | `http://localhost:5000` | No |

### Overriding Configuration per Environment
ASP.NET Core allows overriding keys via environment variables using double underscores:
```bash
export MongoDbSettings__ConnectionString="mongodb://root:example@mongo.internal:27017"
export Cors__AllowedOrigins__0="https://mytodolist.app"
```

---

## 🚀 Getting Started

### Prerequisites
- **.NET 8 SDK LTS** (8.0.400+)
- **Node.js LTS** (v20+ or v22+) & **npm**
- **Docker & Docker Compose** (for running local MongoDB)

### 1. Start MongoDB via Docker Compose
To start a MongoDB container in the background:
```bash
docker compose up -d
```
*(Verify container status using `docker ps`)*

### 2. Run the Backend API
Navigate to the API folder and run:
```bash
cd backend/TodoList.Api
dotnet run
```
- The API will listen on `http://localhost:5000`
- Access Swagger UI at `http://localhost:5000/swagger`
- Access Health Check at `http://localhost:5000/health`

### 3. Run the Frontend (React + Vite)
In a new terminal window:
```bash
cd frontend
npm install
npm run dev
```
- Open `http://localhost:5173` in your browser.

---

## 🧪 Testing & Code Quality

### Backend Unit Tests (xUnit)
The backend test suite executes 36 tests testing the Domain entity invariants and the Application service using the in-memory fake (`InMemoryTaskRepository`):
```bash
cd backend/TodoList.Tests
dotnet test --logger "console;verbosity=normal"
```

### Frontend Utility Tests (Vitest)
Executes pure unit tests for formatting and bounds clamp logic:
```bash
cd frontend
npm run test
```

### Frontend Linting & Production Build
```bash
cd frontend
npm run lint    # ESLint flat configuration check (0 warnings, 0 errors)
npm run build   # TypeScript strict type check (tsc) + Vite production bundling
```

---

## 🏗️ Code Patterns & SOLID

### Error Handling

The application uses modern .NET 8 `IExceptionHandler` (`GlobalExceptionHandler`) to centralize error processing:
- Normalizes all domain and application exceptions into standard RFC 7807 `ProblemDetails`
- Categorizes client errors (`400 Bad Request`, `404 Not Found`) and server errors (`500 Internal Server Error`)
- Logs structured diagnostic messages with severity levels (`LogWarning` vs `LogError`)
- Strips stack traces in production to prevent information disclosure

### Constants & Zero Hardcoding

All business rules, limits, and UI strings are isolated into dedicated constant modules:
- `TaskConstants.MinTitleLength` / `TaskConstants.MaxTitleLength`: 1 to 200 characters
- `STRINGS` in `frontend/src/constants/strings.ts`: Centralizes every UI label, button, error message, and placeholder for seamless internationalization (i18n).
- `SEARCH_DEBOUNCE_MS`: 300ms debounce constant.

### SOLID Principles Mapping

| Principle | Concrete Code Application |
|:----------|:--------------------------|
| **Single Responsibility (SRP)** | `TodoTask` strictly governs business invariants; `TaskService` orchestrates application workflows; `MongoTaskRepository` handles MongoDB persistence; `ProgressBar.tsx` handles only progress display. |
| **Open/Closed (OCP)** | New persistence providers (e.g. `PostgresTaskRepository`) can be introduced by implementing `ITaskRepository` without modifying `TaskService` or API controllers. |
| **Liskov Substitution (LSP)** | `InMemoryTaskRepository` acts as a direct, seamless substitute for `MongoTaskRepository` throughout test suites without altering contract expectations. |
| **Interface Segregation (ISP)** | `ITaskRepository` and `ITaskService` expose only cohesive, purpose-built operations. Request DTOs isolate inputs to only what is needed. |
| **Dependency Inversion (DIP)** | `TaskService` depends exclusively on abstractions (`ITaskRepository` and `TimeProvider`), resolved via ASP.NET Core dependency injection. |

---

## 🔒 Security & Accessibility

- **No Regex Injection**: User search terms in `MongoTaskRepository` are escaped with `Regex.Escape(search)` before querying MongoDB.
- **XSS Prevention**: Safe rendering through React JSX text bindings; zero use of `dangerouslySetInnerHTML`.
- **Accessible Indicators**:
  - Focus rings (`:focus-visible`) for complete keyboard navigability.
  - Priority badges provide both colored indicators and text labels (`Low`, `Medium`, `High`) ensuring compliance for color-blind users.
  - Screen reader attributes: `role="progressbar"`, `aria-valuenow`, `aria-valuemin`, `aria-valuemax`, and dynamic `aria-label`s.

---

## 📊 Project Metrics

| Metric | Value |
|:-------|:------|
| **Backend Tests** | 36 passing (xUnit) |
| **Frontend Tests** | 3 passing (Vitest) |
| **Compiler Warnings** | 0 warnings (`TreatWarningsAsErrors` enabled) |
| **Linter Warnings** | 0 warnings (`ESLint` strict) |
| **Architecture Layers** | 4 (Domain, Application, Infrastructure, Api) |
| **Endpoints** | 9 REST endpoints + OpenAPI + Health |
| **Bundle Size** | ~49 kB gzip (React + app) |

---

## 🎓 Concepts Applied

- **Clean Architecture**: Inward dependency rule separating business rules from infrastructure
- **Domain-Driven Design (DDD)**: Entities encapsulating state, invariants, and behaviors
- **Repository Pattern**: Abstracted data persistence decoupling database technology
- **Dependency Injection**: First-class DI container management in both Api and Application layers
- **TimeProvider Pattern**: Time abstraction enabling reliable, non-flaky test execution
- **Options Pattern**: Strongly typed configuration bindings (`MongoDbSettings`, `CorsSettings`)
- **12-Factor App**: Configuration through environment variables and portable containers

---

## 📝 License

This project is licensed under the MIT License.

---

Developed with ❤️ following Clean Code, SOLID, and Clean Architecture best practices.
