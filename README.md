# LocalCRM

A professional-grade, multi-framework CRM system featuring a C# .NET 10 Clean Architecture Backend and two modern Frontends (Blazor WASM and Angular 18).

## 🚀 Branch Guide

This project is partitioned into three dedicated branches for isolation and clarity:

- **`backend`**: The .NET 10 REST API, including Domain, Infrastructure, and Application layers.
- **`frontend-blazor`**: The Blazor WebAssembly frontend built with MudBlazor.
- **`frontend-angular`**: The Angular 18 standalone frontend built with Tailwind CSS.

---

## 🛠️ Implementation Highlights

### Core Features
- **Full CRM Suite**: CRUD management for Companies, Contacts, Interactions, Engagements, Notes, and Documents.
- **Administrative Recycle Bin**: Built-in soft-delete mechanism with an integrated "Restore" functionality and "Show Deleted" filters.
- **First-Class Tags**: Dedicated management UI to maintain the global tag library.
- **System Settings**: Global parameter management for administrators.
- **Detailed Auditing**: Every data change captures who, when, and exactly what property changed (e.g., "City: London -> Paris").

### Security & Production Readiness (OWASP Hardened)
- **JWT Authentication**: Secure token-based auth with Refresh Token rotation and Password Change requirements.
- **RBAC**: Robust Role-Based Access Control protecting sensitive administrative actions.
- **Rate Limiting**: Integrated ASP.NET Core Rate Limiter to prevent brute-force and DoS attacks.
- **Security Headers**: HSTS, strict CSP, X-Frame-Options (Anti-Clickjacking), and X-Content-Type-Options enabled.
- **Environment-Specific CORS**: Specific origin allow-listing for production deployments.
- **Safe Error Handling**: Production middleware suppresses internal stack traces to prevent information leakage.

---

## 🏗️ Production Readiness Checklist

To take this application to a live internet-facing environment, consider the following:

1.  **Database Strategy**: Transition from **SQLite** (file-based) to a high-availability provider like **PostgreSQL** or **SQL Server**.
2.  **Secret Management**: Move JWT keys and connection strings from `appsettings.json` to **Azure Key Vault**, **AWS Secrets Manager**, or Environment Variables.
3.  **Frontend Deployment**: Build and serve the Blazor/Angular assets from a CDN or Static Web App service (e.g., Vercel, Netlify, Azure Static Web Apps) with HTTPS enforced.
4.  **Logging**: Integrate the internal Audit logs with a centralized logging provider (e.g., **Serilog**, **Application Insights**, or **ELK Stack**) for long-term retention and alerting.
5.  **CORS**: Update the `ProductionPolicy` in `Program.cs` with the specific final domain of your frontend applications.

---

## 🚦 Getting Started

Refer to the branch-specific READMEs and code comments for detailed setup instructions.
