# Microservices.ECommerce (.NET)

This project is an example of a microservice architecture for a basic E-Commerce application developed using .NET (ASP.NET Core).

The main focus of this project is to practically demonstrate the **asynchronous messaging** flow and event-driven communication between services, rather than focusing on complex architectural patterns.

## Project Structure and Services

The project consists of the following services, each responsible for a specific business domain:

* `/Order.API`: The service responsible for creating customer orders and managing order statuses (Confirmed, Canceled, etc.).
* `/StockAPI`: The service that manages product stock and updates stock levels based on orders.
* `/Payment.API`: The service that simulates/processes payment transactions.
* `/Shared`: A shared library containing common classes used by all services, especially the **message contracts (events/commands)** required for inter-service communication.

## Basic Flow (Example Messaging Scenario)

The system uses events published between services to complete an order's lifecycle. An example flow is as follows:

1.  **Order.API** publishes an `OrderCreatedEvent` when it receives a new order.
2.  **StockAPI** listens for this event. It checks the stock and, if successful, publishes a `StockReservedEvent`. (If stock is insufficient, it publishes a `StockFailedEvent`.)
3.  **Payment.API** listens for the `StockReservedEvent`. It attempts to process the payment and, if successful, publishes a `PaymentCompletedEvent`. (If payment fails, it publishes a `PaymentFailedEvent`.)
4.  **Order.API** listens for the `PaymentCompletedEvent` (or potential failure events) and updates the order's final status to "Confirmed" or "Canceled".

## Core Technologies Used

* **Platform**: .NET (ASP.NET Core)
* **Architecture**: Microservices, Event-Driven Architecture
* **Communication**: Asynchronous Messaging (This pattern typically uses a message broker like RabbitMQ or Kafka and a library like MassTransit.)
