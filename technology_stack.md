# 🛠️ Technology Stack Specification (LocalCRM)

## 📊 I. Stack Summary Overview

| Layer | Component | Technology | Version | Purpose |
| :--- | :--- | :--- | :--- | :--- |
| **Backend (API)** | Core Runtime | C# / .NET | 10.0 | Business Logic Execution, API Hosting. |
| | Web Framework | ASP.NET Core | 10.0 | Exposing REST API endpoints. |
| | Documentation | Swagger (Swashbuckle) | 6.5.0 | (Optional/Dev only) API testing. |
| | Data Access (ORM) | Entity Framework Core | 10.0 | Managing domain models and migrations. |
| | Database Engine | SQLite | 3.x | Local file-based relational database. |
| **Testing** | Unit Testing | xUnit | 2.x | Testing service methods and domain logic. |
| | Mocking | Moq | 4.x | Simulating dependencies for unit tests. |
| **Blazor SPA** | Framework | Blazor WebAssembly | 10.0 | Component-based frontend (C#). |
| | Styling | MudBlazor | 7.0 | Material Design components. |
| **Angular SPA** | Framework | Angular | 18.0 | Standalone component-based frontend (TS). |
| | Styling | Tailwind CSS | 3.x | Utility-first styling framework. |

---

## ⚙️ II. Hardening & Security Standards (OWASP)

### 1. API Hardening
- **Rate Limiting:** Partitioned fixed-window limiter to prevent DoS.
- **Security Headers:** HSTS, CSP, X-Frame-Options (DENY), and nosniff enabled.
- **Error Handling:** Production middleware suppresses internal stack traces.

### 2. Identity & Access
- **Authentication:** Custom JWT implementation with PBKDF2 hashing.
- **Session:** Refresh Token rotation with sliding expiration.
- **RBAC:** Multi-level permission model integrated into API controllers.

---

## 🏗️ III. Project Isolation Pattern

The solution is partitioned into three dedicated Git branches for clean delivery:
- `backend`: Complete C# Clean Architecture API.
- `frontend-blazor`: Blazor WebAssembly project with parity UI.
- `frontend-angular`: Angular 18 project with parity UI.
