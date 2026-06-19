# 🚀 Job Tracking System

A full-stack ASP.NET Core MVC application designed to help job seekers efficiently manage and track their job applications throughout the hiring process.

## 🌐 Live Demo

**Website:** http://vinayjobtracker.somee.com

---

# 📌 Project Overview

Job Tracking System is a centralized platform that enables users to manage job applications, track interview progress, monitor offers and rejections, and maintain a complete history of their job search journey.

Instead of managing applications in spreadsheets or notes, users can organize everything in a structured dashboard with search, filtering, auditing, and reporting capabilities.

---

# ✨ Key Features

## 🔐 Authentication & Authorization

* ASP.NET Core Identity Integration
* Secure User Registration & Login
* User-specific data isolation
* Protected routes and actions

## 📊 Dashboard Analytics

* Total Applications Count
* Applied Jobs Count
* Interview Count
* Offer Count
* Rejected Count
* Recent Activity Tracking

## 💼 Job Application Management

* Create Job Applications
* Edit Existing Applications
* View Application Details
* Soft Delete Applications
* Restore Deleted Records (future enhancement)

## 🏢 Company Management

* Add New Companies
* Update Company Information
* Delete Companies
* Associate Applications with Companies

## 🔎 Advanced Search & Filtering

Search applications by:

* Company Name
* Job Role
* Status

Filter by:

* Company
* Status
* Date Range

## 📄 Pagination

* Optimized application listing
* Efficient record navigation
* Better user experience for large datasets

## 📝 Audit Logging

Tracks important user actions including:

* Create Operations
* Update Operations
* Delete Operations

Audit information includes:

* Action Performed
* Entity Name
* Timestamp
* Description

---

# 🛠️ Technology Stack

## Backend

* ASP.NET Core MVC
* C#
* Entity Framework Core
* LINQ
* ASP.NET Identity

## Database

* Microsoft SQL Server

## Frontend

* Razor Views
* Bootstrap 5
* HTML5
* CSS3
* JavaScript

## Development Tools

* Visual Studio 2022
* SQL Server Management Studio
* Git
* GitHub

---

# 🏗️ Architecture

The project follows a layered architecture:

* Controllers
* Services
* ViewModels
* Models
* Data Access Layer
* Entity Framework Core

This separation improves:

* Maintainability
* Scalability
* Testability

---

# 🗄️ Database Design

Core Entities:

## Companies

Stores company information.

Fields:

* Id
* Name
* Location
* Website

## Job Applications

Stores job application records.

Fields:

* Company
* Role
* Status
* Date Applied
* Notes

## Audit Logs

Stores application activity history.

Fields:

* Action
* Entity Name
* Description
* Timestamp

## ASP.NET Identity Tables

Used for:

* Authentication
* Authorization
* User Management

---

# 🔒 Security Features

* ASP.NET Identity Authentication
* Authorization Policies
* User Data Isolation
* Anti-Forgery Protection
* Entity Framework Parameterized Queries

---

# 🎯 Challenges Solved

### Multi-User Data Security

Implemented user-specific filtering to ensure users can only access their own job applications.

### Audit Tracking

Built a custom audit logging system to track important CRUD operations.

### Soft Delete

Implemented soft delete functionality to prevent accidental data loss.

### Search & Pagination

Developed dynamic filtering and pagination for better performance and usability.

## Export to Excel/CSV: 
Save your project data instantly into Excel-compatible CSV format for easy reporting and analysis.

---

# 📸 Screenshots
* Login Page
![Alt Image](https://github.com/vinaysamanthula/JobTrackingSystem/blob/ae78efc8a6ba497dcec360ca07fffcd40b462152/Screenshot%202026-06-19%20210109.png)
* Dashboard
![Alt Image](https://github.com/vinaysamanthula/JobTrackingSystem/blob/5230ac20b5fce4969501421006fd9b43e2208ae1/Screenshot%202026-06-19%20211251.png)
* Job Applications
* Company Management
* Audit Logs

---

# 🚀 Future Enhancements
* Resume Upload
* Email Notifications
* Interview Scheduling
* Dashboard Charts
* Role-Based Authorization

---

# 👨‍💻 Author

### Vinay Samanthula

MCA Graduate | ASP.NET Core Developer

### Skills

* ASP.NET Core MVC
* C#
* Entity Framework Core
* SQL Server
* LINQ
* ASP.NET Identity
* Bootstrap

GitHub: https://github.com/vinaysamanthula

LinkedIn: https://www.linkedin.com/in/vinaysamanthula/

---

⭐ If you found this project useful, feel free to fork and explore the source code.
