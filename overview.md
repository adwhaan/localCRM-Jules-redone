# 🎯 localCRM Application Development Specification

## 📚 I. Core System Summary

localCRM is a modular CRM system centered around:
- Authenticated multi-user access (JWT + Refresh Tokens).
- Role-based authorization (RBAC).
- A dashboard-first workflow with real-time metrics.
- Soft-delete persistence and administrative restoration.
- Detailed, field-level audit logging.
- Clean Architecture backend with multiple framework frontends.

---

## 📂 II. Implementation Status

| Feature | Status | Branch |
| :--- | :--- | :--- |
| **Core API** | Complete (Hardened) | `backend` |
| **Blazor UI** | Complete (Feature Parity) | `frontend-blazor` |
| **Angular UI** | Complete (Feature Parity) | `frontend-angular` |
| **Soft-Delete** | Implemented (All Entities) | All |
| **Restore UI** | Implemented (Admin Only) | Both Frontends |
| **Tag Management** | First-Class Citizen | All |

---

## 🏗️ III. Technical Architecture

The application follows **Clean Architecture** and **SOLID** principles.

### Backend
- **Domain:** Business entities and soft-delete logic.
- **Application:** Services with property-level audit generation.
- **Infrastructure:** EF Core, Identity, and OWASP hardening.
- **API:** RESTful endpoints with secure headers and rate limiting.

### Frontends
- **Blazor:** Built with MudBlazor components for a consistent Material design.
- **Angular:** Modern standalone components with Tailwind CSS for high performance.
- **Parity:** Both frontends implement identical CRM and Administrative features.
