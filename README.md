# FleetBook 🚚

A modern, lightweight fleet management system that brings clarity and efficiency to vehicle operations, maintenance, and logistics coordination.

## Overview

**FleetBook** is a web application for fleet operators, logistics companies, and service providers. It transforms scattered fleet data into actionable insights through an intuitive, responsive dashboard. Built with .NET and Blazor technologies, FleetBook combines powerful backend capabilities with a clean, user-friendly interface designed for teams who depend on their vehicles.

## ✨ Key Features

**Vehicle Management**
- Centralized registry of all fleet assets with detailed specifications and historical data
- Real-time mileage and service hour tracking
- Asset lifecycle management and depreciation monitoring

**Maintenance & Service Tracking**
- Automated maintenance interval reminders to prevent costly downtime
- Complete service history for compliance and warranty documentation
- Scheduled vs. unscheduled maintenance workflows

**Operations Overview**
- Driver assignment and utilization dashboards
- Fleet utilization metrics and capacity planning
- Cost tracking by vehicle, department, or project

**Reporting & Analytics**
- Customizable reports on fleet performance, expenses, and maintenance trends
- Export capabilities for further analysis and auditing
- Visual dashboards for executive decision-making

**Search & Filtering**
- Advanced filtering across vehicles, maintenance records, and assignments
- Quick access to critical fleet information when you need it

## 🛠 Technology Stack

| Component | Technology |
|-----------|-----------|
| **Frontend** | Blazor WebAssembly |
| **Backend** | ASP.NET Core Web API (.NET 10) |
| **Language** | C# |
| **Database** | SQLite |
| **Authentication** | ASP.NET Identity |

## 🏗 Architecture & Design Principles

FleetBook follows modern development practices:

- **API-First Architecture** – RESTful API enables future integrations with external systems
- **Clean Code Patterns** – Repository and Unit of Work patterns ensure maintainability and testability
- **Dependency Injection** – Loose coupling between components for flexibility and unit testing
- **Lightweight & Portable** – SQLite database makes deployment simple and dependency-free
- **Security First** – Role-based access control and data encryption

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK or later
- Git

### Installation

1. **Clone the repository:**
  ```
git clone https://github.com/janjedrzejak/fleetbook.git
cd fleetbook
  ```
3. **Set up the database:**
- The SQLite database is automatically created on first run
- To reset the database, delete `fleetbook.db` and restart the application
- Run Entity Framework migrations if needed:
  ```
  dotnet ef database update
  ```

3. **Run the application:**
  ```
  dotnet run
  ```

4. **Access the application:**
- Open your browser and navigate to `https://localhost:5132`
- Default credentials are available in the documentation

## 📈 Roadmap

**Near-term**
- [ ] Enhanced role-based access control (RBAC)
- [ ] SMS and email alert system for maintenance events
- [ ] Advanced search and filtering capabilities

**Medium-term**
- [ ] Real-time GPS/telematics integration
- [ ] Mobile companion app for drivers and field technicians
- [ ] Integration with external systems via webhooks

**Future Enhancements**
- [ ] Docker containerization for easier deployment
- [ ] Cloud deployment options
- [ ] Advanced analytics with predictive maintenance insights

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add your feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Open a Pull Request with a clear description of your changes

## 📝 License

This project is licensed under the MIT License — see the [LICENSE](./LICENSE) file for details.

## 👨‍💻 Author

**FleetBook** is developed by a full-stack software developer passionate about digital transformation and business automation. Built with a focus on solving real-world fleet management challenges through clean code and user-centered design.

---

**Questions or Feedback?** Open an issue on GitHub or reach out via LinkedIn.
