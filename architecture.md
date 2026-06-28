# 🏛️ System Architecture Specification (LocalCRM)

## 🏗️ I. Multi-Tier High-Level Overview

LocalCRM is built as a decoupled, multi-framework system consisting of a centralized C# backend and multiple frontend SPA implementations.

### 1. Backend (API Layer)
- **Framework:** .NET 10 ASP.NET Core Web API.
- **Pattern:** Clean Architecture (Onion Architecture).
- **Persistence:** Entity Framework Core with a local SQLite file database.
- **Security:** JWT Authentication with Refresh Token rotation and Role-Based Access Control (RBAC).

### 2. Frontend Implementations (Client Layer)
The system supports two independent frontend implementations to satisfy different framework requirements:
- **Stack A (Blazor):** Blazor WebAssembly 10.0 using MudBlazor component library.
- **Stack B (Angular):** Angular 18 using modern standalone components and Tailwind CSS.

---

## 📂 II. Backend: Clean Architecture Layers

### 1. Domain Layer (`LocalCRM.Domain`)
- Contains enterprise-level business entities (Company, Contact, Interaction, etc.).
- Includes base classes for common behaviors like `SoftDeletableEntity` and temporal audit fields (`CreatedAt`, `UpdatedAt`).
- Zero dependencies on external frameworks or other layers.

### 2. Application Layer (`LocalCRM.Application`)
- Contains business logic, interfaces, and DTOs.
- Orchestrates CRUD operations through service classes.
- Implements specialized logic for:
  - **Audit Logging:** Captures property-level change summaries for updates.
  - **Soft-Delete Management:** Handles entity restoration and inclusive filtering.
  - **AutoMapper Profiles:** Defines mappings between Domain entities and DTOs.

### 3. Infrastructure Layer (`LocalCRM.Infrastructure`)
- Implements external concerns like database persistence and security services.
- **Identity Service:** Manages JWT generation, password hashing (PBKDF2), and RBAC verification.
- **Persistence:** Configures `LocalCRMContext` with global query filters for soft-delete and automated audit field population.

### 4. Web API Layer (`LocalCRM.API`)
- Exposes RESTful endpoints for frontend consumption.
- **Middleware:** Includes OWASP-hardened security headers, Rate Limiting, and sanitized global exception handling.
- **CORS:** Configured for environment-specific origin allow-listing.

---

## 🎨 III. Frontend: Single Page Applications

### 1. Common Communication Pattern
- Both frontends communicate with the Backend via a unified REST API.
- **Authentication:** Persistent JWT storage in local storage with automatic inclusion in request headers via Interceptors (Angular) or custom API Services (Blazor).

### 2. Administrative Features (Parity)
- **Recycle Bin:** Both UIs feature an integrated 'Show Deleted' toggle on list views and administrative 'Restore' actions.
- **Metadata Management:** First-class UIs for managing the global Tag library and system Settings.
- **Dashboard:** Surfacing real-time metrics and detailed Audit Log summaries.
