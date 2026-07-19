# Student Management System

A comprehensive, role-based Student Management System built with **.NET Core MVC** and **MySQL**. This application is designed to handle administrative and academic tasks, providing unique, secure portals for Students, Staff, and Administrators.

## 🚀 Features

### Role-Based Access Control
- **Admin**: Full system control. Can create/edit staff, manage departments, and oversee all students.
- **Staff**: Department-level control. Can manage their assigned students, add attendance, and record exam marks.
- **Student**: Read-only access to their own profile, attendance records, and academic performance.

### Core Functionality
- **Dynamic Dashboards**: Custom interfaces tailored to the user's role (Admin Dashboard vs. Staff Dashboard).
- **Data Filtering & Search**: Instantly filter student and staff directories by department or search queries.
- **Academic Tracking**: Comprehensive relational database structure to track student marks, exam types, and daily attendance.
- **Secure Authentication**: Passwords are securely hashed using BCrypt.

## 🛠️ Technology Stack
- **Backend**: C#, ASP.NET Core MVC, .NET 8
- **Database**: MySQL, Entity Framework Core (Code-First Approach)
- **Frontend**: Razor Pages (.cshtml), HTML5, CSS3, FontAwesome Icons
- **Security**: BCrypt Password Hashing, Session Management, Anti-Forgery Tokens

## 📂 Project Structure (MVC)
The application adheres strictly to the Model-View-Controller architectural pattern:
- **Models**: Defines the database schema (Users, Students, Departments, Attendance, Marks).
- **Views**: Dynamic, server-rendered HTML pages using Razor syntax.
- **Controllers**: Handles routing, database transactions, and business logic.
- **Data**: Contains `ApplicationDbContext` for Entity Framework database translation.

## ⚙️ Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- MySQL Server

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/prem-arvinth-g/Student_Management_System_.NET.git
   ```
2. Update the database connection string in `appsettings.json` with your MySQL credentials.
3. Open your terminal in the project directory and run the Entity Framework migrations to build the database:
   ```bash
   dotnet ef database update
   ```
4. Start the application:
   ```bash
   dotnet run
   ```

*(Note: The system automatically seeds an Admin account on the first run so you can log in immediately.)*
