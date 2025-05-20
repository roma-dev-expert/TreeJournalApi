# TreeJournalApi

**Version:** v1.0

## Overview

TreeJournalApi is a web API built with ASP.NET Core designed to manage hierarchical tree data and log exceptions. The project offers endpoints for logging exceptions and for managing tree-structured data, making it easy to create, delete, rename, and retrieve tree nodes.

## Features

- **Exception Logging:**  
  Provides endpoints to log exceptions and retrieve logs for monitoring and troubleshooting.
  
- **Tree Management:**  
  - Create or retrieve a tree by name.
  - Add child nodes.
  - Delete nodes.
  - Rename nodes.
  
- **Error Handling Middleware:**  
  Custom middleware intercepts errors (such as SecureException or standard exceptions) and returns detailed JSON responses with error information.
  
- **Testing:**  
  The project employs integration and unit tests using xUnit, in-memory SQLite databases, and a custom `CustomWebApplicationFactory` to ensure reliable, isolated testing.

## Technologies Used

- **ASP.NET Core**
- **Entity Framework Core**
- **SQLite** (in-memory for tests)
- **PostgreSQL** (for production or additional environments)
- **xUnit** for testing
- **Git & GitHub** for version control and collaboration

## Getting Started

### Prerequisites

- [.NET 6 SDK (or later)](https://dotnet.microsoft.com/download)
- A code editor such as Visual Studio, Visual Studio Code, or Rider

### Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/roma-dev-expert/TreeJournalApi.git
   cd TreeJournalApi
