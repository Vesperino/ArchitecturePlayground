# Architecture Overview

## Architecture Style

**Modular Monolith + Vertical Slice Architecture**

This project demonstrates a pragmatic approach to building enterprise applications in 2025, avoiding the over-engineering trap of microservices while maintaining clear module boundaries.

---

## Table of Contents

- [C4 Architecture](#c4-architecture)
  - [Context Diagram](#context-diagram)
  - [Container Diagram](#container-diagram)
  - [Component: Identity Module](#component-identity-module)
  - [Component: Ordering Module](#component-ordering-module)
- [Business Flows](#business-flows)
  - [Order Flow](#order-flow)
  - [Checkout Saga State Machine](#checkout-saga-state-machine)
  - [Authentication Flow](#authentication-flow)
  - [Payment Flow](#payment-flow)
- [Data Architecture](#data-architecture)
  - [Data Ownership Map](#data-ownership-map)
  - [PostgreSQL Schema (Identity & Ordering)](#postgresql-schema)
  - [MongoDB Schema (Catalog)](#mongodb-schema)
  - [Redis Data Structures](#redis-data-structures)
- [Communication Patterns](#communication-patterns)
  - [Module Communication Matrix](#module-communication-matrix)
  - [Outbox Pattern Flow](#outbox-pattern-flow)
  - [SignalR Real-time Flow](#signalr-real-time-flow)

---

## C4 Architecture

### Context Diagram

System context showing external actors and systems.

```mermaid
C4Context
    title System Context - ArchitecturePlayground E-Commerce

    Person(customer, "Customer", "Browses products and makes purchases")
    Person(admin, "Administrator", "Manages products, orders, and users")

    System(ecommerce, "E-Commerce Platform", "Allows customers to browse products, manage cart, and place orders")

    System_Ext(stripe, "Stripe", "Payment processing gateway")
    System_Ext(sendgrid, "SendGrid", "Transactional email service")
    System_Ext(oauth, "OAuth Providers", "Google, GitHub for social login")
    System_Ext(cloudamqp, "CloudAMQP", "Managed RabbitMQ for async messaging")

    Rel(customer, ecommerce, "Browses products, places orders", "HTTPS")
    Rel(admin, ecommerce, "Manages catalog, views reports", "HTTPS")

    Rel(ecommerce, stripe, "Processes payments", "HTTPS/API")
    Rel(ecommerce, sendgrid, "Sends emails", "HTTPS/API")
    Rel(ecommerce, oauth, "Authenticates users", "OAuth 2.0")
    Rel(ecommerce, cloudamqp, "Publishes/consumes events", "AMQP")

    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="1")
```

### Container Diagram

High-level technology choices and container interactions.

```mermaid
C4Container
    title Container Diagram - ArchitecturePlayground E-Commerce

    Person(customer, "Customer", "Browses and purchases products")

    System_Boundary(platform, "E-Commerce Platform") {
        Container(spa, "Vue SPA", "Vue 3, TypeScript, Pinia", "Single-page application for customers")
        Container(api, "API Application", ".NET 9, ASP.NET Core", "Modular monolith hosting all business modules")

        ContainerDb(postgres, "PostgreSQL", "Supabase", "Stores users, orders, payments")
        ContainerDb(mongodb, "MongoDB", "Atlas", "Stores product catalog with flexible schema")
        ContainerDb(redis, "Redis", "Upstash", "Session cache, shopping cart, rate limiting")
        ContainerQueue(rabbitmq, "RabbitMQ", "CloudAMQP", "Async event messaging between modules")
    }

    System_Ext(stripe, "Stripe", "Payment gateway")
    System_Ext(sendgrid, "SendGrid", "Email service")
    System_Ext(azure, "Azure Services", "Key Vault, Blob Storage, App Insights")

    Rel(customer, spa, "Uses", "HTTPS")
    Rel(spa, api, "Calls", "HTTPS/JSON")

    Rel(api, postgres, "Reads/Writes", "TCP/SSL")
    Rel(api, mongodb, "Reads/Writes", "TCP/SSL")
    Rel(api, redis, "Reads/Writes", "TCP/SSL")
    Rel(api, rabbitmq, "Publishes/Consumes", "AMQP")

    Rel(api, stripe, "Processes payments", "HTTPS")
    Rel(api, sendgrid, "Sends emails", "HTTPS")
    Rel(api, azure, "Secrets, Storage, Monitoring", "HTTPS")

    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="1")
```

### Component: Identity Module

Internal structure of the Identity module showing vertical slice architecture.

```mermaid
C4Component
    title Component Diagram - Identity Module

    Container_Boundary(identity, "Identity Module") {
        Component(endpoints, "Endpoints", "Minimal API", "HTTP endpoints for auth operations")
        Component(features, "Features", "Vertical Slices", "Register, Login, RefreshToken, ChangePassword")
        Component(domain, "Domain", "Entities & Value Objects", "User, Role, Email, Password")
        Component(services, "Services", "Infrastructure", "JwtTokenService, PasswordHasher, OAuthService")
        Component(persistence, "Persistence", "EF Core", "IdentityDbContext, UserConfiguration")
        Component(contracts, "Contracts", "Public API", "IIdentityModule, UserDto, UserCreatedEvent")
    }

    ContainerDb(postgres, "PostgreSQL", "Supabase", "identity schema")
    System_Ext(oauth, "OAuth Providers", "Google, GitHub")
    ContainerQueue(rabbitmq, "RabbitMQ", "Integration Events")

    Rel(endpoints, features, "Dispatches commands/queries")
    Rel(features, domain, "Uses domain logic")
    Rel(features, services, "Uses infrastructure")
    Rel(services, persistence, "Persists data")
    Rel(persistence, postgres, "Reads/Writes")
    Rel(services, oauth, "Authenticates via")
    Rel(features, rabbitmq, "Publishes events via Outbox")

    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="1")
```

### Component: Ordering Module

Internal structure of the Ordering module with CQRS and Saga patterns.

```mermaid
C4Component
    title Component Diagram - Ordering Module

    Container_Boundary(ordering, "Ordering Module") {
        Component(endpoints, "Endpoints", "Minimal API", "HTTP endpoints for order operations")
        Component(commands, "Commands", "CQRS Write Side", "CreateOrder, CancelOrder, CompleteOrder")
        Component(queries, "Queries", "CQRS Read Side", "GetOrder, GetUserOrders, GetOrderHistory")
        Component(saga, "Order Saga", "MassTransit State Machine", "Orchestrates order workflow")
        Component(domain, "Domain", "Aggregate Root", "Order, OrderItem, OrderStatus")
        Component(persistence, "Persistence", "EF Core + Outbox", "OrderingDbContext, OutboxMessage")
        Component(contracts, "Contracts", "Public API", "OrderDto, OrderCreatedEvent")
    }

    ContainerDb(postgres, "PostgreSQL", "Supabase", "ordering schema")
    ContainerDb(redis, "Redis", "Upstash", "Saga state storage")
    ContainerQueue(rabbitmq, "RabbitMQ", "Order events")

    Rel(endpoints, commands, "Write operations")
    Rel(endpoints, queries, "Read operations")
    Rel(commands, domain, "Domain logic")
    Rel(commands, persistence, "Persist + Outbox")
    Rel(saga, rabbitmq, "Consumes/Publishes events")
    Rel(saga, redis, "Stores saga state")
    Rel(persistence, postgres, "Reads/Writes")

    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="1")
```

---

## Business Flows

### Order Flow

Complete order lifecycle from cart to delivery.

```mermaid
sequenceDiagram
    autonumber
    participant C as Customer
    participant SPA as Vue SPA
    participant API as API Gateway
    participant B as Basket Module
    participant O as Ordering Module
    participant I as Inventory
    participant P as Payment Module
    participant N as Notification Module
    participant MQ as RabbitMQ

    C->>SPA: Click "Checkout"
    SPA->>API: POST /api/v1/orders
    API->>B: GetBasket(userId)
    B-->>API: BasketItems[]

    API->>O: CreateOrderCommand
    O->>O: Validate & Create Order
    O->>O: Save Order + OutboxMessage
    O-->>API: 202 Accepted {orderId}
    API-->>SPA: Order Created
    SPA-->>C: Show "Order Processing"

    Note over O,MQ: Background: Outbox Worker

    O->>MQ: Publish OrderCreatedEvent
    MQ->>I: OrderCreatedEvent
    I->>I: Reserve Stock

    alt Stock Available
        I->>MQ: InventoryReservedEvent
        MQ->>P: InventoryReservedEvent
        P->>P: Process Payment (Stripe)

        alt Payment Success
            P->>MQ: PaymentCompletedEvent
            MQ->>O: PaymentCompletedEvent
            O->>O: Update Status = Paid
            MQ->>N: OrderConfirmedEvent
            N->>C: Email: "Order Confirmed"
        else Payment Failed
            P->>MQ: PaymentFailedEvent
            MQ->>I: ReleaseInventory
            MQ->>O: Update Status = Cancelled
            MQ->>N: OrderCancelledEvent
            N->>C: Email: "Payment Failed"
        end
    else Out of Stock
        I->>MQ: InventoryReservationFailedEvent
        MQ->>O: Update Status = Cancelled
        MQ->>N: OrderCancelledEvent
        N->>C: Email: "Out of Stock"
    end
```

### Checkout Saga State Machine

MassTransit state machine managing order workflow with timeouts and compensation.

```mermaid
stateDiagram-v2
    [*] --> Created: OrderCreatedEvent

    Created --> InventoryReserving: ReserveInventoryCommand

    InventoryReserving --> InventoryReserved: InventoryReservedEvent
    InventoryReserving --> Cancelled: InventoryReservationFailedEvent

    InventoryReserved --> PaymentPending: ProcessPaymentCommand
    InventoryReserved --> Cancelled: PaymentTimeout (15min)

    PaymentPending --> Paid: PaymentCompletedEvent
    PaymentPending --> Cancelled: PaymentFailedEvent
    PaymentPending --> Cancelled: PaymentTimeout (15min)

    Paid --> Shipped: ShipmentCreatedEvent
    Paid --> Refunding: RefundRequestedEvent

    Shipped --> Delivered: DeliveryConfirmedEvent
    Shipped --> Returning: ReturnRequestedEvent

    Refunding --> Refunded: RefundCompletedEvent
    Returning --> Returned: ReturnConfirmedEvent

    Cancelled --> [*]
    Delivered --> [*]
    Refunded --> [*]
    Returned --> [*]

    note right of Cancelled
        Compensation Actions:
        - Release inventory
        - Refund payment (if charged)
        - Notify customer
    end note

    note right of PaymentPending
        Timeout: 15 minutes
        Auto-cancel if no payment
    end note
```

### Authentication Flow

JWT authentication with refresh token rotation.

```mermaid
sequenceDiagram
    autonumber
    participant C as Client (SPA)
    participant API as API Gateway
    participant I as Identity Module
    participant R as Redis
    participant DB as PostgreSQL

    Note over C,DB: Initial Login

    C->>API: POST /api/v1/identity/login {email, password}
    API->>I: LoginCommand
    I->>DB: Find User by Email
    DB-->>I: User
    I->>I: Verify Password (Argon2)

    alt Valid Credentials
        I->>I: Generate JWT (15min expiry)
        I->>I: Generate RefreshToken (7 days)
        I->>DB: Store RefreshToken (hashed)
        I->>R: Cache User Claims
        I-->>API: {accessToken, refreshToken}
        API-->>C: 200 OK + Tokens
    else Invalid Credentials
        I-->>API: Invalid credentials
        API-->>C: 401 Unauthorized
    end

    Note over C,DB: Token Refresh (before expiry)

    C->>API: POST /api/v1/identity/refresh {refreshToken}
    API->>I: RefreshTokenCommand
    I->>DB: Find RefreshToken (hashed)

    alt Valid & Not Expired
        I->>I: Rotate: Revoke old, Generate new
        I->>DB: Update RefreshToken
        I->>I: Generate new JWT
        I-->>API: {accessToken, newRefreshToken}
        API-->>C: 200 OK + New Tokens
    else Invalid/Expired
        I->>DB: Revoke all user tokens (security)
        I-->>API: Token invalid
        API-->>C: 401 - Re-login required
    end

    Note over C,DB: OAuth Login (Google)

    C->>API: GET /api/v1/identity/oauth/google
    API->>I: Redirect to Google
    I-->>C: Redirect to Google OAuth
    C->>C: Google Login
    C->>API: Callback with code
    API->>I: OAuthCallbackCommand
    I->>I: Exchange code for Google tokens
    I->>I: Get user info from Google
    I->>DB: Find or Create User
    I->>I: Generate JWT + RefreshToken
    I-->>API: {accessToken, refreshToken}
    API-->>C: Redirect to SPA with tokens
```

### Payment Flow

Stripe payment processing with idempotency.

```mermaid
sequenceDiagram
    autonumber
    participant O as Ordering Module
    participant MQ as RabbitMQ
    participant P as Payment Module
    participant S as Stripe API
    participant DB as PostgreSQL

    MQ->>P: InventoryReservedEvent {orderId, amount}
    P->>DB: Check existing payment (idempotency)

    alt Payment Already Processed
        DB-->>P: Payment record exists
        P-->>MQ: PaymentCompletedEvent (idempotent)
    else New Payment
        P->>P: Generate IdempotencyKey
        P->>S: Create PaymentIntent {amount, idempotencyKey}

        alt Stripe Success
            S-->>P: PaymentIntent {status: succeeded}
            P->>DB: Save Payment record
            P->>P: Add OutboxMessage
            P->>DB: SaveChanges (atomic)
            Note over P,DB: Outbox ensures event delivery
            P-->>MQ: PaymentCompletedEvent
        else Stripe Failure
            S-->>P: PaymentIntent {status: failed}
            P->>DB: Save Failed Payment
            P-->>MQ: PaymentFailedEvent {reason}
        else Stripe Timeout
            P->>P: Retry with same IdempotencyKey
            Note over P,S: Stripe deduplicates by key
        end
    end
```

---

## Data Architecture

### Data Ownership Map

Bounded contexts and their data stores.

```mermaid
flowchart TB
    subgraph "Identity Context"
        I[Identity Module]
        I --> PG1[(PostgreSQL<br/>identity schema)]
        I --> R1[(Redis<br/>sessions, rate-limit)]
    end

    subgraph "Catalog Context"
        CA[Catalog Module]
        CA --> MG[(MongoDB<br/>catalog DB)]
        CA --> R2[(Redis<br/>product cache)]
    end

    subgraph "Ordering Context"
        O[Ordering Module]
        O --> PG2[(PostgreSQL<br/>ordering schema)]
        O --> R3[(Redis<br/>saga state)]
    end

    subgraph "Basket Context"
        B[Basket Module]
        B --> R4[(Redis<br/>carts TTL=7d)]
    end

    subgraph "Payment Context"
        P[Payment Module]
        P --> PG3[(PostgreSQL<br/>payment schema)]
    end

    subgraph "Notification Context"
        N[Notification Module]
        N --> PG4[(PostgreSQL<br/>notification schema)]
    end

    subgraph "Analytics Context"
        A[Analytics Module]
        A -.->|Read Replica| PG2
        A -.->|Read Replica| PG3
    end

    subgraph "Shared Infrastructure"
        MQ[RabbitMQ<br/>CloudAMQP]
        AZ[Azure Key Vault<br/>Secrets]
        BL[Azure Blob<br/>Images]
    end

    I & CA & O & B & P & N --> MQ
    I & CA & O & P --> AZ
    CA --> BL

    style PG1 fill:#336791,color:#fff
    style PG2 fill:#336791,color:#fff
    style PG3 fill:#336791,color:#fff
    style PG4 fill:#336791,color:#fff
    style MG fill:#4DB33D,color:#fff
    style R1 fill:#DC382D,color:#fff
    style R2 fill:#DC382D,color:#fff
    style R3 fill:#DC382D,color:#fff
    style R4 fill:#DC382D,color:#fff
    style MQ fill:#FF6600,color:#fff
```

### PostgreSQL Schema

Entity-relationship diagram for Identity and Ordering schemas.

```mermaid
erDiagram
    %% Identity Schema
    Users ||--o{ UserRoles : has
    Users ||--o{ RefreshTokens : has
    Users ||--o{ UserClaims : has
    Roles ||--o{ UserRoles : has

    Users {
        uuid Id PK
        string Email UK
        string PasswordHash
        string FirstName
        string LastName
        boolean EmailConfirmed
        boolean TwoFactorEnabled
        datetime CreatedAt
        datetime LastLoginAt
    }

    Roles {
        uuid Id PK
        string Name UK
        string Description
    }

    UserRoles {
        uuid UserId FK
        uuid RoleId FK
    }

    RefreshTokens {
        uuid Id PK
        uuid UserId FK
        string TokenHash
        datetime ExpiresAt
        datetime CreatedAt
        string CreatedByIp
        datetime RevokedAt
        string RevokedByIp
    }

    UserClaims {
        uuid Id PK
        uuid UserId FK
        string ClaimType
        string ClaimValue
    }

    %% Ordering Schema
    Orders ||--|{ OrderItems : contains
    Orders ||--o{ OrderStatusHistory : has
    Orders }o--|| Users : "placed by"

    Orders {
        uuid Id PK
        uuid UserId FK
        string OrderNumber UK
        string Status
        decimal TotalAmount
        string Currency
        json ShippingAddress
        json BillingAddress
        datetime CreatedAt
        datetime UpdatedAt
    }

    OrderItems {
        uuid Id PK
        uuid OrderId FK
        uuid ProductId
        string ProductName
        decimal UnitPrice
        int Quantity
        decimal TotalPrice
    }

    OrderStatusHistory {
        uuid Id PK
        uuid OrderId FK
        string Status
        string Notes
        datetime ChangedAt
    }

    %% Payment Schema
    Payments }o--|| Orders : "for"

    Payments {
        uuid Id PK
        uuid OrderId FK
        string StripePaymentIntentId UK
        string IdempotencyKey UK
        decimal Amount
        string Currency
        string Status
        string FailureReason
        datetime CreatedAt
        datetime CompletedAt
    }

    %% Outbox
    OutboxMessages {
        uuid Id PK
        string Type
        json Content
        datetime OccurredOn
        datetime ProcessedOn
        string Error
        int RetryCount
    }
```

### MongoDB Schema

Document structure for product catalog.

```mermaid
classDiagram
    class Product {
        +ObjectId _id
        +String sku
        +String name
        +String slug
        +String description
        +Decimal price
        +String currency
        +Int stockQuantity
        +String[] images
        +String status
        +Object attributes
        +DateTime createdAt
        +DateTime updatedAt
    }

    class Category {
        +ObjectId _id
        +String name
        +String slug
        +String description
        +ObjectId parentId
        +String[] path
        +Int level
        +Int sortOrder
        +Boolean isActive
    }

    class ProductVariant {
        +ObjectId _id
        +ObjectId productId
        +String sku
        +String name
        +Object attributes
        +Decimal priceModifier
        +Int stockQuantity
    }

    class Review {
        +ObjectId _id
        +ObjectId productId
        +ObjectId userId
        +Int rating
        +String title
        +String comment
        +Boolean verified
        +DateTime createdAt
    }

    Product "1" --> "*" ProductVariant : variants
    Product "*" --> "*" Category : categories
    Product "1" --> "*" Review : reviews

    note for Product "Flexible attributes:<br/>Electronics: brand, specs<br/>Clothing: size, color<br/>Books: author, ISBN"
```

### Redis Data Structures

Redis usage patterns across modules.

```mermaid
flowchart LR
    subgraph "Session & Auth"
        S1["session:#123;userId#125;"]
        S2["rate-limit:#123;ip#125;:#123;endpoint#125;"]
        S3["blacklist:#123;tokenJti#125;"]
    end

    subgraph "Shopping Cart"
        C1["cart:#123;userId#125;"]
        C2["cart:#123;guestId#125;"]
    end

    subgraph "Caching"
        CA1["product:#123;id#125;"]
        CA2["category:#123;slug#125;"]
        CA3["user:#123;id#125;:profile"]
    end

    subgraph "Saga State"
        SA1["saga:order:#123;correlationId#125;"]
    end

    subgraph "Real-time"
        RT1["signalr:connections:#123;userId#125;"]
        RT2["notifications:#123;userId#125;"]
    end

    S1 -.->|"HASH<br/>TTL: 30min"| D1[Structure]
    S2 -.->|"STRING<br/>TTL: 1min"| D2[Counter]
    S3 -.->|"SET<br/>TTL: token exp"| D3[JTI Set]

    C1 -.->|"HASH<br/>TTL: 7 days"| D4[Cart Items]

    CA1 -.->|"JSON<br/>TTL: 1 hour"| D5[Product JSON]

    SA1 -.->|"JSON<br/>TTL: 24 hours"| D6[Saga State]

    RT2 -.->|"LIST<br/>LPUSH/BRPOP"| D7[Notification Queue]

    style S1 fill:#DC382D,color:#fff
    style S2 fill:#DC382D,color:#fff
    style S3 fill:#DC382D,color:#fff
    style C1 fill:#DC382D,color:#fff
    style C2 fill:#DC382D,color:#fff
    style CA1 fill:#DC382D,color:#fff
    style CA2 fill:#DC382D,color:#fff
    style CA3 fill:#DC382D,color:#fff
    style SA1 fill:#DC382D,color:#fff
    style RT1 fill:#DC382D,color:#fff
    style RT2 fill:#DC382D,color:#fff
```

---

## Communication Patterns

### Module Communication Matrix

How modules communicate with each other.

```mermaid
flowchart TB
    subgraph "API Layer"
        GW[API Gateway<br/>YARP]
    end

    subgraph "Modules"
        I[Identity]
        CA[Catalog]
        B[Basket]
        O[Ordering]
        P[Payment]
        N[Notification]
        A[Analytics]
    end

    subgraph "Message Broker"
        MQ[RabbitMQ]
    end

    GW -->|REST| I & CA & B & O

    %% Synchronous (via Contracts)
    O -.->|IIdentityModule| I
    O -.->|ICatalogModule| CA
    O -.->|IBasketModule| B

    %% Asynchronous (via Events)
    I ==>|UserCreatedEvent| MQ
    O ==>|OrderCreatedEvent| MQ
    O ==>|OrderCompletedEvent| MQ
    P ==>|PaymentCompletedEvent| MQ
    P ==>|PaymentFailedEvent| MQ

    MQ ==>|OrderCreatedEvent| P
    MQ ==>|PaymentCompletedEvent| O & N
    MQ ==>|PaymentFailedEvent| O & N
    MQ ==>|OrderCompletedEvent| N
    MQ ==>|UserCreatedEvent| N
    MQ ==>|All Events| A

    classDef sync stroke:#2196F3,stroke-width:2px
    classDef async stroke:#FF9800,stroke-width:2px

    linkStyle 4,5,6 stroke:#2196F3
    linkStyle 7,8,9,10,11,12,13,14,15,16,17 stroke:#FF9800
```

**Legend:**
- Solid arrows (→): HTTP/REST calls
- Dashed arrows (⇢): Synchronous module contracts (in-process)
- Double arrows (⇒): Asynchronous events via RabbitMQ

### Outbox Pattern Flow

Reliable message delivery with transactional outbox.

```mermaid
sequenceDiagram
    autonumber
    participant H as Handler
    participant DB as PostgreSQL
    participant W as Outbox Worker
    participant MQ as RabbitMQ
    participant C as Consumer

    Note over H,C: Step 1: Atomic Save (same transaction)

    H->>H: Process Command
    H->>H: Create Domain Event
    H->>DB: BEGIN TRANSACTION
    H->>DB: INSERT Order
    H->>DB: INSERT OutboxMessage {type, content, occurredOn}
    H->>DB: COMMIT
    H-->>H: Return Success

    Note over H,C: Step 2: Background Publishing

    loop Every 1 second
        W->>DB: SELECT * FROM OutboxMessages<br/>WHERE ProcessedOn IS NULL<br/>ORDER BY OccurredOn<br/>LIMIT 100
        DB-->>W: Unprocessed messages

        loop For each message
            W->>MQ: Publish event

            alt Success
                MQ-->>W: ACK
                W->>DB: UPDATE OutboxMessage<br/>SET ProcessedOn = NOW()
            else Failure
                W->>DB: UPDATE OutboxMessage<br/>SET RetryCount = RetryCount + 1,<br/>Error = 'message'
            end
        end
    end

    Note over H,C: Step 3: Consumer Processing

    MQ->>C: Deliver event
    C->>C: Process (idempotent)
    C-->>MQ: ACK

    Note over W,DB: Cleanup: Delete processed messages older than 7 days
```

### SignalR Real-time Flow

Real-time notifications to connected clients.

```mermaid
sequenceDiagram
    autonumber
    participant C as Client (SPA)
    participant SR as SignalR Hub
    participant R as Redis
    participant MQ as RabbitMQ
    participant N as Notification Module

    Note over C,N: Connection Setup

    C->>SR: Connect (JWT token)
    SR->>SR: Validate JWT
    SR->>R: Store connection {userId: connectionId}
    SR-->>C: Connected

    C->>SR: JoinGroup("orders:{userId}")
    SR->>R: Add to group

    Note over C,N: Real-time Order Update

    MQ->>N: OrderStatusChangedEvent
    N->>N: Create notification
    N->>R: GET connections for userId
    R-->>N: connectionIds[]

    alt User Online
        N->>SR: SendToGroup("orders:{userId}", notification)
        SR->>C: OrderStatusChanged {orderId, newStatus}
        C->>C: Update UI
    else User Offline
        N->>R: LPUSH notifications:{userId}
        Note over R: Stored for later delivery
    end

    Note over C,N: Reconnection - Fetch Missed

    C->>SR: Reconnect
    SR->>R: LRANGE notifications:{userId} 0 -1
    R-->>SR: Missed notifications[]
    SR->>C: Batch send missed notifications
    SR->>R: DEL notifications:{userId}
```

---

## Module Structure

Each business module follows this structure:

```
Module/
├── Module.Core/           # Domain + Application (Features)
│   ├── Features/          # Vertical slices
│   ├── Domain/            # Entities, Value Objects, Events
│   └── Exceptions/        # Domain exceptions
├── Module.Infrastructure/ # External concerns
│   ├── Persistence/       # DbContext, Configurations
│   └── Services/          # External service implementations
└── Module.Contracts/      # Public API
    ├── DTOs/              # Data transfer objects
    └── Events/            # Integration events
```

## Key Architectural Decisions

See [Architecture Decision Records](../adr/) for detailed reasoning behind:

- [ADR-0001: Modular Monolith Architecture](../adr/0001-modular-monolith-architecture.md)
- [ADR-0002: Vertical Slice Architecture](../adr/0002-vertical-slice-architecture.md)
- [ADR-0003: CQRS with MediatR](../adr/0003-cqrs-with-mediatr.md)
- [ADR-0004: Outbox Pattern for Messaging](../adr/0004-outbox-pattern.md)
- [ADR-0005: MongoDB for Catalog](../adr/0005-mongodb-for-catalog.md)

## Tech Stack

See [tech-stack.md](tech-stack.md) for detailed technology choices.
