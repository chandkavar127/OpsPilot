# OpsPilot Web Application

## Overview
OpsPilot is a web application built using ASP.NET Core MVC that provides a modern and interactive dashboard for managing requests. This application features a sleek 3D dashboard UI, allowing users to easily navigate and manage their requests.

## Features
- **Dashboard Overview**: Displays key metrics such as total requests, pending requests, approved requests, and rejected requests.
- **Interactive Cards**: Each metric is represented in a card format, providing a visually appealing way to present data.
- **Responsive Design**: The application is designed to be responsive, ensuring a seamless experience across devices.
- **User-Friendly Forms**: Easy-to-use forms for submitting requests with clear labels and placeholders.

## Technologies Used
- ASP.NET Core MVC
- Entity Framework Core
- HTML5, CSS3, JavaScript
- Bootstrap for responsive design

## Installation
1. Clone the repository:
   ```
   git clone <repository-url>
   ```
2. Navigate to the project directory:
   ```
   cd OpsPilot.Web
   ```
3. Restore the dependencies:
   ```
   dotnet restore
   ```
4. Run the application:
   ```
   dotnet run
   ```

## Custom CSS
The application includes a custom CSS file located at `wwwroot/css/dashboard-3d.css` that styles the dashboard with a modern look and feel. Key styles include:
- Gradient backgrounds
- Card hover effects
- Responsive tables and buttons

## Layout
The main layout of the application is defined in `Views/Shared/_Layout.cshtml`, which includes links to the custom CSS files and a structured layout for the sidebar and navbar.

## Dashboard View
The dashboard view is located at `Views/Dashboard/Index.cshtml`, showcasing various metrics and a table for detailed request information. The view is designed to be intuitive and user-friendly.

## Contribution
Contributions are welcome! Please submit a pull request or open an issue for any enhancements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.