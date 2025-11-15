# Microservices.ECommerce (.NET)

This project is an example of a microservice architecture for a basic E-Commerce application developed using .NET (ASP.NET Core).

This project serves as a platform for experimenting with different transaction management patterns in distributed systems.

## Project Structure and Services

The project consists of the following services, each responsible for a specific business domain:

* `/Order.API`: The service responsible for creating customer orders.
* `/StockAPI`: The service that manages product stock.
* `/Payment.API`: The service that simulates payment transactions.
* `/Shared`: A shared library containing message contracts (events/commands) required for inter-service communication.
* **(New) `/Coordinator.Service`**: A central coordinator added to implement the **Two-Phase Commit (2PC)** pattern for cross-service transactions.

## Patterns and Experiments

### 1. Event-Driven Architecture (Choreography)

The original foundation of the project is based on an **Eventual Consistency** model, where services communicate by publishing and subscribing to asynchronous events. There is no central coordinator in this flow (Choreography).

*Example Flow:*
1.  **Order.API** publishes an `OrderCreatedEvent`.
2.  **StockAPI** listens to the event and publishes `StockReservedEvent`.
3.  **Payment.API** listens to `StockReservedEvent` and publishes `PaymentCompletedEvent`.
4.  Each service updates its own state by reacting to the relevant events.

### 2. Two-Phase Commit (2PC) Experiment (New)

During development, unlike the Saga pattern, the **Two-Phase Commit (2PC)** pattern, which aims to achieve **Strong Consistency**, was also tested.

In this experiment, the `/Coordinator.Service` plays a central role, making the transaction atomic:

1.  **Phase 1 (Prepare/Vote):** The Coordinator sends a "prepare" request to the `Order`, `Stock`, and `Payment` services, asking them to prepare for the transaction (e.g., lock resources).
2.  If all services can perform the transaction, they vote "Yes".
3.  **Phase 2 (Commit/Abort):**
    * If *all* services vote "Yes," the Coordinator sends a "Commit" command to all of them, and the transaction is atomically completed.
    * If *any* service votes "No" (or fails), the Coordinator sends an "Abort" command to all services, and all prepared changes are rolled back.

This is an "all or nothing" approach that guarantees the transaction either succeeds across *all* services or fails across *all* services simultaneously.

### 3. Data Synchronization

Additionally, experiments were conducted on concepts related to **Data Synchronization** between microservices.

## Core Technologies Used

* **Platform**: .NET (ASP.NET Core)
* **Architecture**: Microservices
* **Patterns**: Event-Driven (Choreography) and Two-Phase Commit (2PC)
* **Communication**: Asynchronous Messaging (for Event-Driven) and Synchronous Communication (for 2PC)
