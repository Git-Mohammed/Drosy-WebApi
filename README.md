# Drosy Web API

Drosy is a specialized educational management platform built with **ASP.NET Core**. It provides a comprehensive suite of tools for managing students, academic plans, attendance tracking, and financial payments.

## 🚀 Key Features

-   **Student Management**: Handle student registration, personal info, and academic tracking (`Drosy.Api/Controllers/StudentsController.cs`).
-   **Attendance System**: Real-time tracking of student presence in sessions (`Drosy.Api/Controllers/AttendencesController.cs`).
-   **Financial Records**: Comprehensive payment processing and history tracking for student plans (`Drosy.Api/Controllers/PaymentController.cs`).
-   **Plan & Session Scheduling**: Create academic plans and dynamic session schedules (`Drosy.Api/Controllers/PlansController.cs`, `Drosy.Api/Controllers/SessionsController.cs`).
-   **Dashboard Analytics**: Visualized stats for administrators (`Drosy.Api/Controllers/DashboardController.cs`).
-   **Region/Localization**: Organizational management of cities and regions (`Drosy.Api/Controllers/RegionController.cs`).

---

## 🏗️ Architecture

The project follows **Clean Architecture** principles to ensure scalability and maintainability:

-   **Drosy.Api**: The entry point. Handles HTTP requests, authentication, and API middleware.
-   **Drosy.Application**: Contains the business logic, interfaces, and DTOs. It uses the **Service Pattern** and **DTOs** to isolate business rules from the external layer (`Drosy.Application/UseCases/`).
-   **Drosy.Domain**: (Represented in the logic flow) Contains the core entities and enterprise logic.
-   **Drosy.Infrastructure**: Implements persistence, mail services, and logging (`Drosy.Api/Extensions/DependencyInjection/AddInfrastructureServicesRegistrationExtension.cs`).

---

## 🛠️ Tech Stack

-   **Framework**: ASP.NET Core 8.0/7.0
-   **Database**: SQL Server (configured via `DbInitializerExtention.cs`)
-   **Authentication**: JWT (JSON Web Tokens) (`Drosy.Application/Interfaces/Common/IJwtService.cs`)
-   **Validation**: FluentValidation (`Drosy.Api/Filters/FluentValidationFilter.cs`)
-   **Architecture Patterns**: 
    -   Repository Pattern
    -   Dependency Injection
    -   Unit of Work
    -   DTO (Data Transfer Objects)

---

## 📝 Getting Started

### Prerequisites
-   .NET SDK (Latest version)
-   SQL Server instance
-   Visual Studio 2022 or VS Code

### Installation

1.  **Clone the repository**:
    ```bash
    git clone https://github.com/Git-Mohammed/Drosy-WebApi.git
    cd Drosy-WebApi
    ```

2.  **Database Setup**:
    Update the connection string in `Drosy.Api/appsettings.json` (or the environment configuration). The system includes an automatic initializer found in `Drosy.Application/Interfaces/Common/IDbInitializer.cs`.

3.  **Run the application**:
    ```bash
    dotnet restore
    dotnet run --project Drosy.Api
    ```

4.  **API Documentation**:
    Once running, navigate to `/swagger` to view the Interactive API documentation.

---

## 📂 Project Structure Highlights

-   **Service Registration**: Services are modularly registered in `Drosy.Api/Extensions/DependencyInjection/`.
-   **API Responses**: Standardized response formats (Success/Failure) are implemented in `Drosy.Api/Commons/Responses/ApiResponse.cs`.
-   **Use Cases**: Logic is organized by feature (e.g., `Payments`, `Students`, `Sessions`) inside `Drosy.Application/UseCases/`.

---

## 🛠 Contributing

1. Create a Feature Branch (`git checkout -b feature/AmazingFeature`)
2. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
3. Push to the Branch (`git push origin feature/AmazingFeature`)
4. Open a Pull Request

---

### Suggested Follow-up Reading
- **FluentValidation**: Check `Drosy.Api/Filters/FluentValidationFilter.cs` to see how the project handles request validation automatically.
- **Dependency Injection**: Explore `AddApplicationServicesRegistration.cs` to understand how business services are injected into the container.
