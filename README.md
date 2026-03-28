# OpsPilot

OpsPilot is a clean, layered **ASP.NET Core MVC** workflow application for request approvals with a multi-level flow:

**Employee → Manager → Admin**

It is designed for enterprise-style approval management and project/demo presentation.

---

## ✨ Key Features

- Multi-step approval workflow
  - Employee submits requests
  - Manager reviews and approves/rejects
  - Admin performs final approval
- Role-based flow visibility
- Dashboard-style UI with request status insights
- Clean Razor Views + Bootstrap 5 frontend
- Layered architecture for maintainability

---

## 🧱 Solution Architecture

This project follows a layered structure:

- **OpsPilot.Web** – ASP.NET Core MVC UI layer
- **OpsPilot.Application** – Application/business use-cases
- **OpsPilot.Domain** – Core entities and domain rules
- **OpsPilot.Infrastructure** – Data access and external integrations

---

## 🛠️ Tech Stack

- **.NET 8**
- **ASP.NET Core MVC**
- **Razor Views**
- **Bootstrap 5**
- **Entity Framework Core**

---

## 📁 Project Structure

```text
ASP/
├─ OpsPilot.Web/
├─ OpsPilot.Application/
├─ OpsPilot.Domain/
└─ OpsPilot.Infrastructure/
```

---

## ✅ Prerequisites

Before running the project, ensure you have:

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- SQL Server (or configured database provider)
- Visual Studio 2022 / VS Code

---

## 🚀 Getting Started

### 1) Clone the repository

```bash
git clone https://github.com/chandkavar127/OpsPilot.git
cd OpsPilot
```

### 2) Restore dependencies

```bash
dotnet restore
```

### 3) Configure database connection

Update your connection string in:

- `OpsPilot.Web/appsettings.json`

### 4) Apply migrations (if migrations are included)

```bash
dotnet ef database update --project .\OpsPilot.Infrastructure\OpsPilot.Infrastructure.csproj --startup-project .\OpsPilot.Web\OpsPilot.Web.csproj
```

### 5) Run the application

```bash
dotnet run --project .\OpsPilot.Web\OpsPilot.Web.csproj
```

Then open the local URL shown in terminal (example: `https://localhost:xxxx`).

---

## 🔄 Approval Status Flow

Typical request status states:

- `PendingManagerApproval`
- `PendingAdminApproval`
- `Approved`
- `Rejected`

---

## 🧪 Development Notes

- Use `dotnet build` to verify compilation
- Keep UI changes in `OpsPilot.Web/Views` and `OpsPilot.Web/wwwroot`
- Keep business rules in `Application`/`Domain` layers

---

## 🤝 Contributing

1. Create a feature branch
2. Commit your changes with clear messages
3. Push branch and create a Pull Request

---

## 📄 License

This project is for learning/demo/interview usage.  
Add a formal license (MIT/Apache-2.0) if you plan public/open-source distribution.