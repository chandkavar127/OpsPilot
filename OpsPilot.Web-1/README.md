# README.md

# OpsPilot Web Application

Welcome to the OpsPilot Web Application! This project is designed to provide a modern, premium dashboard experience for managing employee requests and approvals. The application utilizes ASP.NET Core MVC with Razor Views and incorporates a unique design featuring 3D card styles, glassmorphism effects, and smooth animations.

## Features

- **Dashboard Overview**: A comprehensive dashboard that displays key metrics and statuses.
- **Approval Workflow**: A streamlined process for employees to submit requests, which can be approved or rejected by managers and admins.
- **Responsive Design**: The application is fully responsive, ensuring a seamless experience across devices.

## Project Structure

```
OpsPilot.Web
├── Controllers
│   ├── DashboardController.cs
│   └── HomeController.cs
├── Models
│   └── ViewModels
│       └── DashboardViewModel.cs
├── Views
│   ├── Dashboard
│   │   ├── Index.cshtml
│   │   ├── _DashboardCards.cshtml
│   │   ├── _TableExample.cshtml
│   │   └── _FormExample.cshtml
│   └── Shared
│       └── _Layout.cshtml
├── wwwroot
│   └── css
│       ├── site.css
│       └── dashboard-premium.css
├── Program.cs
├── OpsPilot.Web.csproj
└── README.md
```

## Installation

1. Clone the repository to your local machine.
2. Navigate to the project directory.
3. Restore the NuGet packages by running:
   ```
   dotnet restore
   ```
4. Run the application using:
   ```
   dotnet run
   ```
5. Open your browser and navigate to `http://localhost:5000` to view the application.

## Customization

The dashboard's appearance can be customized by modifying the CSS located in `wwwroot/css/dashboard-premium.css`. This file contains styles for the 3D cards, glassmorphism effects, and other UI components.

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any enhancements or bug fixes.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.