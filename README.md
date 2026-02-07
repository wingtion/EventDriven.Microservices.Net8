# Event-Driven Microservices Architecture with .NET 8 & RabbitMQ

This project demonstrates a robust, production-ready microservices architecture built with **.NET 8**, implementing the **Database-per-Service** pattern and **Event-Driven Communication** using **RabbitMQ** (via MassTransit). It is fully containerized with **Docker** for easy deployment.

## Architecture Overview

The system is designed to handle high-throughput order processing asynchronously, ensuring data consistency and service decoupling.

*   **Order API (Producer):** Receives customer orders, persists them to its own `OrderDb` (SQL Server), and publishes an `OrderCreatedEvent` to the message broker.
*   **Stock API (Consumer):** Subscribes to the event queue, processes the business logic, and updates inventory in its isolated `StockDb`.
*   **RabbitMQ:** Acts as the reliable message broker, ensuring guaranteed delivery even if services are down.
*   **Docker Compose:** Orchestrates the entire ecosystem (SQL Server, RabbitMQ, and Microservices) with a single command.

## Tech Stack & Patterns

| Technology | Usage |
| :--- | :--- |
| **.NET 8 (ASP.NET Core)** | High-performance Web API framework |
| **RabbitMQ** | Message Broker for asynchronous communication |
| **MassTransit** | Distributed Application Framework for .NET |
| **Entity Framework Core** | ORM with Code-First approach |
| **MSSQL (SQL Server)** | Relational Database (Running in Docker) |
| **Docker & Docker Compose** | Containerization and Orchestration |
| **Swagger UI** | API Documentation and Testing |

### Design Patterns Implemented
*   **Microservices Architecture** (Decoupled Services)
*   **Event-Driven Architecture** (EDA)
*   **Database-per-Service** (Data Isolation)
*   **Publisher/Subscriber** (Pub/Sub)
*   **Repository Pattern** & **DTOs**

## Getting Started

### Prerequisites
*   [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Must be running)
*   .NET 8 SDK (Optional, for local development)

### 🐳 Run with Docker (Recommended)

You don't need to install SQL Server or RabbitMQ manually. Just use Docker Compose!

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/wingtion/EventDriven.Microservices.Net8.git
    cd EventDriven.Microservices.Net8
    ```

2.  **Start the infrastructure:**
    ```bash
    docker-compose up --build
    ```
    *(Wait for all containers to start healthy)*.

3.  **Create Databases (First Time Only):**
    Open **Package Manager Console** in Visual Studio or use CLI to update databases:
    ```powershell
    Update-Database -Project Order.API
    Update-Database -Project Stock.API
    ```

4.  **Seed Data (Optional):**
    Connect to `localhost,1433` (SA / My_Strong_Password_123!) and insert dummy stock data:
    ```sql
    INSERT INTO Stocks (ProductId, Quantity) VALUES (10, 100);
    ```

### 🧪 How to Test
1.  Open **Order API Swagger**: [http://localhost:5000/swagger](http://localhost:5000/swagger)
2.  Send a `POST` request to `/api/Orders` with `productId: 10` and `count: 5`.
3.  Check **Stock API logs** or Database. You will see the stock quantity decreased to **95** automatically!

## License
This project is licensed under the MIT License.
