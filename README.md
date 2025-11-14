# Microservices.ECommerce (.NET)

This project is an example of a microservice architecture for a basic E-Commerce application developed using .NET (ASP.NET Core).

The main focus of this project is to practically demonstrate the **asynchronous messaging** flow and event-driven communication between services.

## Project Structure and Services

The project consists of the following services, each responsible for a specific business domain:

* `/Order.API`: The service responsible for creating customer orders and managing order statuses (Confirmed, Canceled, etc.).
* `/StockAPI`: The service that manages product stock and updates stock levels based on orders.
* `/Payment.API`: The service that simulates/processes payment transactions.
* `/Shared`: A shared library containing common classes used by all services, especially the **message contracts (events/commands)** required for inter-service communication.
* **(New) `/Coordinator.Service`**: An experimental component added to centrally manage and orchestrate cross-service transactions (Sagas).

## Basic Flow (Messaging Scenario - Choreography)

The system uses events published between services to complete an order's lifecycle. This flow is an example of the **Choreography** pattern:

1.  **Order.API** publishes an `OrderCreatedEvent` when it receives a new order.
2.  **StockAPI** listens for this event. It checks the stock and, if successful, publishes a `StockReservedEvent`.
3.  **Payment.API** listens for the `StockReservedEvent`. It attempts to process the payment and, if successful, publishes a `PaymentCompletedEvent`.
4.  **Order.API** listens for the `PaymentCompletedEvent` and updates the order's final status to "Confirmed".

## Experiments During Development

This project also serves as a platform for learning and experimentation.

### 1. Saga Coordinator (Orchestration) Experiment

In addition to the event-driven flow, a `Coordinator` service was added to experiment with the **Saga Orchestration** pattern. This approach manages the order flow (stock reservation, payment) step-by-step from a central point, aiming to handle complex transactions and compensating actions (rollbacks) from a single location.

### 2. Data Synchronization Experiment

As part of the learning process, I also experimented with concepts related to **Data Synchronization** between microservices. This was a separate study to understand the challenges and patterns involved in keeping data consistent across distributed services.

## Core Technologies Used

* **Platform**: .NET (ASP.NET Core)
* **Architecture**: Microservices, Event-Driven Architecture
* **Communication**: Asynchronous Messaging (e.g., RabbitMQ, Kafka)
* **Patterns**: Saga (with experiments in both Choreography and Orchestration)
