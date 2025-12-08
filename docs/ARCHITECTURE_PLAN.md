# ArchitecturePlayground - E-commerce Platform

## Wizja Projektu
Kompleksowa platforma e-commerce demonstrująca umiejętności senior developera w .NET 9 + Vue 3.
**Architektura: Modular Monolith + Vertical Slice Architecture** (trend 2025)

---

## Dlaczego Modular Monolith zamiast Microservices?

| Aspekt | Microservices | Modular Monolith |
|--------|---------------|------------------|
| Kompleksność | Wysoka (sieć, discovery, orchestration) | Niska (jeden deployment) |
| Koszty DevOps | Wysokie | Niskie |
| Transakcje | Rozproszone (Saga, 2PC) | ACID w ramach modułów |
| Debugging | Trudne (distributed tracing) | Łatwe (jeden proces) |
| Skalowanie | Niezależne per service | Horyzontalne całej app |
| Ścieżka migracji | N/A | → Microservices gdy potrzeba |
| **Trend 2025** | ❌ Over-engineering hype | ✅ Pragmatyczne podejście |

> "Amazon Prime Video abandoned microservices, cutting costs by 90%"
> "Start with modular monolith, move to microservices when scaling pain is real"

---

## Tech Stack

| Warstwa | Technologia |
|---------|-------------|
| Backend | .NET 9, ASP.NET Core Minimal APIs |
| Architecture | Modular Monolith + Vertical Slice + DDD |
| Frontend | Vue 3 (Composition API), TypeScript, Pinia, TailwindCSS |
| Bazy danych | PostgreSQL (główna), MongoDB (katalog), Redis (cache) |
| ORM | Entity Framework Core 9, Dapper (raporty) |
| Messaging | MediatR (sync) + MassTransit/RabbitMQ (async) |
| Cloud | VPS + Cloud managed services (hybrid) |
| Container | Docker, Docker Compose |
| CI/CD | GitHub Actions |

---

## Architektura - Modular Monolith + Vertical Slice

### Struktura Solution (.sln)

```
ArchitecturePlayground.sln
│
├── src/
│   │
│   ├── Bootstrapper/
│   │   └── ArchitecturePlayground.API/           # 🚀 JEDEN HOST DLA WSZYSTKIEGO
│   │       ├── Program.cs                        # Composition root
│   │       ├── appsettings.json
│   │       ├── ArchitecturePlayground.API.csproj
│   │       └── Dockerfile
│   │
│   ├── Modules/                                  # 📦 MODUŁY BIZNESOWE
│   │   │
│   │   ├── Identity/
│   │   │   ├── Identity.Core/                    # Domain + Application (Vertical Slices)
│   │   │   │   ├── Features/
│   │   │   │   │   ├── Register/
│   │   │   │   │   │   ├── RegisterCommand.cs
│   │   │   │   │   │   ├── RegisterHandler.cs
│   │   │   │   │   │   ├── RegisterValidator.cs
│   │   │   │   │   │   └── RegisterEndpoint.cs
│   │   │   │   │   ├── Login/
│   │   │   │   │   │   ├── LoginCommand.cs
│   │   │   │   │   │   ├── LoginHandler.cs
│   │   │   │   │   │   └── LoginEndpoint.cs
│   │   │   │   │   ├── RefreshToken/
│   │   │   │   │   ├── ChangePassword/
│   │   │   │   │   └── GetUserProfile/
│   │   │   │   ├── Domain/
│   │   │   │   │   ├── User.cs                   # Aggregate Root
│   │   │   │   │   ├── Role.cs
│   │   │   │   │   ├── ValueObjects/
│   │   │   │   │   │   ├── Email.cs
│   │   │   │   │   │   ├── Password.cs
│   │   │   │   │   │   └── UserId.cs
│   │   │   │   │   └── Events/
│   │   │   │   │       ├── UserRegisteredEvent.cs
│   │   │   │   │       └── UserLoggedInEvent.cs
│   │   │   │   ├── Exceptions/
│   │   │   │   ├── Extensions/
│   │   │   │   │   └── IdentityModuleExtensions.cs
│   │   │   │   └── Identity.Core.csproj
│   │   │   │
│   │   │   ├── Identity.Infrastructure/
│   │   │   │   ├── Persistence/
│   │   │   │   │   ├── IdentityDbContext.cs
│   │   │   │   │   ├── Configurations/
│   │   │   │   │   └── Migrations/
│   │   │   │   ├── Services/
│   │   │   │   │   ├── JwtTokenService.cs
│   │   │   │   │   ├── PasswordHasher.cs
│   │   │   │   │   └── OAuthService.cs
│   │   │   │   └── Identity.Infrastructure.csproj
│   │   │   │
│   │   │   └── Identity.Contracts/               # 📋 PUBLIC API dla innych modułów
│   │   │       ├── IIdentityModule.cs
│   │   │       ├── DTOs/
│   │   │       │   └── UserDto.cs
│   │   │       ├── Events/
│   │   │       │   └── UserCreatedIntegrationEvent.cs
│   │   │       └── Identity.Contracts.csproj
│   │   │
│   │   ├── Catalog/
│   │   │   ├── Catalog.Core/
│   │   │   │   ├── Features/
│   │   │   │   │   ├── GetProducts/
│   │   │   │   │   ├── GetProductById/
│   │   │   │   │   ├── CreateProduct/
│   │   │   │   │   ├── UpdateProduct/
│   │   │   │   │   ├── DeleteProduct/
│   │   │   │   │   └── SearchProducts/
│   │   │   │   └── Domain/
│   │   │   │       ├── Product.cs
│   │   │   │       ├── Category.cs
│   │   │   │       └── ValueObjects/
│   │   │   ├── Catalog.Infrastructure/           # MongoDB
│   │   │   └── Catalog.Contracts/
│   │   │
│   │   ├── Ordering/
│   │   │   ├── Ordering.Core/
│   │   │   │   ├── Features/
│   │   │   │   │   ├── CreateOrder/
│   │   │   │   │   ├── GetOrder/
│   │   │   │   │   ├── GetUserOrders/
│   │   │   │   │   ├── CancelOrder/
│   │   │   │   │   └── CompleteOrder/
│   │   │   │   └── Domain/
│   │   │   │       ├── Order.cs                  # Aggregate Root
│   │   │   │       ├── OrderItem.cs
│   │   │   │       ├── OrderStatus.cs
│   │   │   │       └── Events/
│   │   │   ├── Ordering.Infrastructure/          # PostgreSQL + EF Core
│   │   │   └── Ordering.Contracts/
│   │   │
│   │   ├── Basket/
│   │   │   ├── Basket.Core/
│   │   │   │   ├── Features/
│   │   │   │   │   ├── GetBasket/
│   │   │   │   │   ├── AddItem/
│   │   │   │   │   ├── RemoveItem/
│   │   │   │   │   ├── UpdateQuantity/
│   │   │   │   │   └── Checkout/
│   │   │   │   └── Domain/
│   │   │   ├── Basket.Infrastructure/            # Redis
│   │   │   └── Basket.Contracts/
│   │   │
│   │   ├── Payment/
│   │   │   ├── Payment.Core/
│   │   │   ├── Payment.Infrastructure/
│   │   │   └── Payment.Contracts/
│   │   │
│   │   └── Notification/
│   │       ├── Notification.Core/
│   │       ├── Notification.Infrastructure/
│   │       └── Notification.Contracts/
│   │
│   ├── Shared/                                   # 🔧 SHARED KERNEL
│   │   ├── Shared.Abstractions/
│   │   │   ├── Domain/
│   │   │   │   ├── Entity.cs
│   │   │   │   ├── AggregateRoot.cs
│   │   │   │   ├── ValueObject.cs
│   │   │   │   ├── DomainEvent.cs
│   │   │   │   └── IDomainEventHandler.cs
│   │   │   ├── CQRS/
│   │   │   │   ├── ICommand.cs
│   │   │   │   ├── IQuery.cs
│   │   │   │   └── ICommandHandler.cs
│   │   │   ├── Results/
│   │   │   │   ├── Result.cs
│   │   │   │   └── Error.cs
│   │   │   ├── Exceptions/
│   │   │   └── Shared.Abstractions.csproj
│   │   │
│   │   ├── Shared.Infrastructure/
│   │   │   ├── Persistence/
│   │   │   │   ├── BaseDbContext.cs
│   │   │   │   └── UnitOfWork.cs
│   │   │   ├── Caching/
│   │   │   │   ├── ICacheService.cs
│   │   │   │   └── RedisCacheService.cs
│   │   │   ├── Messaging/
│   │   │   │   ├── IEventBus.cs
│   │   │   │   ├── InMemoryEventBus.cs
│   │   │   │   └── IntegrationEvent.cs
│   │   │   ├── Security/
│   │   │   │   └── CurrentUserService.cs
│   │   │   ├── Behaviors/                        # MediatR Pipelines
│   │   │   │   ├── ValidationBehavior.cs
│   │   │   │   ├── LoggingBehavior.cs
│   │   │   │   └── TransactionBehavior.cs
│   │   │   └── Shared.Infrastructure.csproj
│   │   │
│   │   └── Shared.Contracts/                     # Integration events między modułami
│   │       ├── Events/
│   │       │   ├── OrderCreatedIntegrationEvent.cs
│   │       │   └── PaymentCompletedIntegrationEvent.cs
│   │       └── Shared.Contracts.csproj
│   │
│   └── Web/
│       └── vue-storefront/                       # Vue 3 SPA
│           ├── src/
│           │   ├── modules/                      # Feature modules (mirror backend)
│           │   │   ├── identity/
│           │   │   ├── catalog/
│           │   │   ├── basket/
│           │   │   └── ordering/
│           │   ├── shared/
│           │   └── App.vue
│           ├── package.json
│           └── vite.config.ts
│
├── tests/
│   ├── Modules/
│   │   ├── Identity.Tests/
│   │   │   ├── Unit/
│   │   │   │   ├── Domain/
│   │   │   │   │   └── UserTests.cs
│   │   │   │   └── Features/
│   │   │   │       └── RegisterHandlerTests.cs
│   │   │   └── Integration/
│   │   │       └── IdentityEndpointsTests.cs
│   │   ├── Catalog.Tests/
│   │   ├── Ordering.Tests/
│   │   └── Basket.Tests/
│   ├── Architecture.Tests/
│   │   └── ModuleDependencyTests.cs              # NetArchTest - sprawdza granice modułów
│   └── E2E.Tests/
│       └── PlaywrightTests/
│
├── docs/
│   ├── architecture/
│   ├── adr/
│   ├── api/
│   └── diagrams/
│
├── infrastructure/
│   ├── docker/
│   │   ├── docker-compose.yml
│   │   ├── docker-compose.override.yml
│   │   └── Dockerfile
│   └── scripts/
│
├── .github/
│   └── workflows/
│
├── Directory.Build.props
├── Directory.Packages.props
├── .editorconfig
├── .gitignore
└── README.md
```

### Vertical Slice Architecture - Struktura Feature

```
Features/
└── Register/
    ├── RegisterCommand.cs        # Request DTO + Command
    ├── RegisterHandler.cs        # Business logic
    ├── RegisterValidator.cs      # FluentValidation
    ├── RegisterEndpoint.cs       # Minimal API endpoint
    └── RegisterResponse.cs       # Response DTO (opcjonalnie)
```

```csharp
// RegisterCommand.cs
public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : ICommand<Result<Guid>>;

// RegisterHandler.cs
public sealed class RegisterHandler : ICommandHandler<RegisterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterCommand command, CancellationToken ct)
    {
        // 1. Validate domain rules
        // 2. Create User aggregate
        // 3. Save to database
        // 4. Publish domain event
        // 5. Return Result
    }
}

// RegisterEndpoint.cs
public static class RegisterEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/identity/register", async (
            RegisterCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/v1/identity/users/{result.Value}", result.Value)
                : Results.BadRequest(result.Error);
        })
        .WithTags("Identity")
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
```

### Komunikacja między modułami

```
┌─────────────────────────────────────────────────────────────────────┐
│                         HOST (API)                                   │
│                    ArchitecturePlayground.API                        │
└─────────────────────────────────────────────────────────────────────┘
         │              │              │              │
         ▼              ▼              ▼              ▼
┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐
│  Identity   │ │   Catalog   │ │  Ordering   │ │   Basket    │
│   Module    │ │   Module    │ │   Module    │ │   Module    │
├─────────────┤ ├─────────────┤ ├─────────────┤ ├─────────────┤
│ .Core       │ │ .Core       │ │ .Core       │ │ .Core       │
│ .Infra      │ │ .Infra      │ │ .Infra      │ │ .Infra      │
│ .Contracts◄─┼──.Contracts◄─┼──.Contracts◄─┼──.Contracts  │
└──────┬──────┘ └──────┬──────┘ └──────┬──────┘ └──────┬──────┘
       │               │               │               │
       ▼               ▼               ▼               ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      SHARED INFRASTRUCTURE                           │
│   EventBus (in-process) │ Caching │ Logging │ Validation            │
└─────────────────────────────────────────────────────────────────────┘
       │                       │                       │
       ▼                       ▼                       ▼
┌─────────────┐         ┌─────────────┐         ┌─────────────┐
│ PostgreSQL  │         │   MongoDB   │         │    Redis    │
│ (Supabase)  │         │   (Atlas)   │         │  (Upstash)  │
└─────────────┘         └─────────────┘         └─────────────┘
```

**Zasady komunikacji:**
1. Moduły **NIE** mogą bezpośrednio referencjonować inne moduły (.Core, .Infrastructure)
2. Komunikacja tylko przez **Contracts** (interfejsy, DTOs, Integration Events)
3. **MediatR** dla synchronicznych operacji w ramach HTTP request
4. **MassTransit + RabbitMQ** dla asynchronicznych operacji (scalability ready)

---

## Messaging Strategy (Scalability: 10k concurrent users)

### Dual Messaging Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           HTTP REQUEST                                   │
│                                                                          │
│  POST /api/v1/orders                                                     │
│  ┌────────────────────────────────────────────────────────────────────┐ │
│  │  1. MediatR: CreateOrderCommand                                    │ │
│  │     - Validate                                                      │ │
│  │     - Create Order aggregate                                        │ │
│  │     - Save to DB + OutboxMessage (same transaction)                 │ │
│  │     - Return 202 Accepted                                           │ │
│  └────────────────────────────────────────────────────────────────────┘ │
│                                 │                                        │
│                                 │ ~50-100ms                              │
│                                 ▼                                        │
│                    ┌─────────────────────────┐                          │
│                    │  HTTP Response: 202     │                          │
│                    │  { orderId: "abc-123" } │                          │
│                    └─────────────────────────┘                          │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│                      BACKGROUND (Outbox Worker)                          │
│                                                                          │
│  ┌──────────────────┐     ┌──────────────────────────────────────────┐  │
│  │  Outbox Worker   │────▶│  CloudAMQP (RabbitMQ)                    │  │
│  │  (polls every 1s)│     │                                          │  │
│  └──────────────────┘     │  Exchanges:                              │  │
│                           │  - order-events                          │  │
│                           │  - payment-events                        │  │
│                           │  - notification-events                   │  │
│                           └──────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│                      ASYNC CONSUMERS (MassTransit)                       │
│                                                                          │
│  OrderCreatedEvent ──────▶ ┌─────────────────────────────────────────┐  │
│                            │ InventoryReservationConsumer            │  │
│                            │ - Check stock                           │  │
│                            │ - Reserve items                         │  │
│                            │ - Publish InventoryReservedEvent        │  │
│                            └─────────────────────────────────────────┘  │
│                                           │                              │
│                                           ▼                              │
│  InventoryReservedEvent ─▶ ┌─────────────────────────────────────────┐  │
│                            │ PaymentProcessingConsumer               │  │
│                            │ - Call Stripe API                       │  │
│                            │ - Publish PaymentCompletedEvent         │  │
│                            │   OR PaymentFailedEvent                 │  │
│                            └─────────────────────────────────────────┘  │
│                                           │                              │
│                                           ▼                              │
│  PaymentCompletedEvent ──▶ ┌─────────────────────────────────────────┐  │
│                            │ OrderCompletionConsumer                 │  │
│                            │ - Update Order status                   │  │
│                            │ - Publish OrderCompletedEvent           │  │
│                            └─────────────────────────────────────────┘  │
│                                           │                              │
│                                           ▼                              │
│  OrderCompletedEvent ────▶ ┌─────────────────────────────────────────┐  │
│                            │ NotificationConsumer                    │  │
│                            │ - Send confirmation email               │  │
│                            │ - Send push notification                │  │
│                            └─────────────────────────────────────────┘  │
│                                                                          │
│  PaymentFailedEvent ─────▶ ┌─────────────────────────────────────────┐  │
│                            │ CompensationConsumer                    │  │
│                            │ - Release inventory                     │  │
│                            │ - Update Order status = Cancelled       │  │
│                            │ - Notify user                           │  │
│                            └─────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────┘
```

### Kiedy MediatR vs RabbitMQ

| Operacja | Transport | Dlaczego |
|----------|-----------|----------|
| Query (GET) | MediatR | Sync, potrzebujemy response |
| Simple Command | MediatR | Szybkie, w ramach request |
| Long-running | RabbitMQ | Nie blokujemy HTTP |
| Cross-module side effects | RabbitMQ | Loose coupling |
| External API calls | RabbitMQ | Retry, timeout handling |
| Notifications | RabbitMQ | Fire-and-forget z durability |
| Analytics/Audit | RabbitMQ | Nie spowalniamy core flow |

### Outbox Pattern Implementation

```csharp
// Shared.Infrastructure/Outbox/OutboxMessage.cs
public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; }           // "OrderCreatedEvent"
    public string Content { get; set; }         // JSON payload
    public DateTime OccurredOn { get; set; }
    public DateTime? ProcessedOn { get; set; }
    public string? Error { get; set; }
    public int RetryCount { get; set; }
}

// W DbContext - zapisujemy razem z agregatem
public override async Task<int> SaveChangesAsync(CancellationToken ct)
{
    // 1. Zbierz domain events z agregatów
    var domainEvents = ChangeTracker.Entries<AggregateRoot>()
        .SelectMany(x => x.Entity.DomainEvents)
        .ToList();

    // 2. Konwertuj na OutboxMessages
    foreach (var @event in domainEvents)
    {
        OutboxMessages.Add(new OutboxMessage
        {
            Type = @event.GetType().Name,
            Content = JsonSerializer.Serialize(@event),
            OccurredOn = DateTime.UtcNow
        });
    }

    // 3. Zapisz wszystko w jednej transakcji
    return await base.SaveChangesAsync(ct);
}

// Background Worker - publikuje do RabbitMQ
public class OutboxProcessor : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var messages = await _db.OutboxMessages
                .Where(m => m.ProcessedOn == null)
                .Take(100)
                .ToListAsync(ct);

            foreach (var msg in messages)
            {
                await _bus.Publish(Deserialize(msg), ct);
                msg.ProcessedOn = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync(ct);
            await Task.Delay(1000, ct);  // Poll every 1s
        }
    }
}
```

### Order Process Manager (State Machine)

```csharp
// Ordering.Core/ProcessManagers/OrderStateMachine.cs
public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        // States
        State(() => Created);
        State(() => InventoryReserved);
        State(() => PaymentPending);
        State(() => Paid);
        State(() => Shipped);
        State(() => Delivered);
        State(() => Cancelled);

        // Initial
        Initially(
            When(OrderCreated)
                .Then(ctx => ctx.Saga.OrderId = ctx.Message.OrderId)
                .TransitionTo(Created)
                .Publish(ctx => new ReserveInventoryCommand(ctx.Message.OrderId))
        );

        // Created → InventoryReserved
        During(Created,
            When(InventoryReserved)
                .TransitionTo(InventoryReserved)
                .Publish(ctx => new ProcessPaymentCommand(ctx.Saga.OrderId)),
            When(InventoryReservationFailed)
                .TransitionTo(Cancelled)
                .Publish(ctx => new OrderCancelledEvent(ctx.Saga.OrderId, "Out of stock"))
        );

        // InventoryReserved → Paid
        During(InventoryReserved,
            When(PaymentCompleted)
                .TransitionTo(Paid)
                .Publish(ctx => new SendOrderConfirmationCommand(ctx.Saga.OrderId)),
            When(PaymentFailed)
                .TransitionTo(Cancelled)
                .Publish(ctx => new ReleaseInventoryCommand(ctx.Saga.OrderId))
        );

        // Timeout handling
        Schedule(() => PaymentTimeout, x => x.PaymentTimeoutToken, s =>
        {
            s.Delay = TimeSpan.FromMinutes(15);
            s.Received = x => x.CorrelateById(m => m.Message.OrderId);
        });

        During(InventoryReserved,
            When(PaymentTimeout.Received)
                .TransitionTo(Cancelled)
                .Publish(ctx => new ReleaseInventoryCommand(ctx.Saga.OrderId))
                .Publish(ctx => new OrderCancelledEvent(ctx.Saga.OrderId, "Payment timeout"))
        );
    }
}
```

### TDD Approach dla Messaging

```csharp
// 1. Unit Test - Handler (bez RabbitMQ)
[Fact]
public async Task CreateOrder_Should_SaveOrder_And_AddOutboxMessage()
{
    // Arrange
    var command = new CreateOrderCommand(UserId, Items);
    var handler = new CreateOrderHandler(_dbContext, _validator);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeTrue();
    _dbContext.Orders.Should().HaveCount(1);
    _dbContext.OutboxMessages.Should().HaveCount(1);
    _dbContext.OutboxMessages.First().Type.Should().Be("OrderCreatedEvent");
}

// 2. Integration Test - Consumer (z Testcontainers)
[Fact]
public async Task InventoryConsumer_Should_Reserve_Stock()
{
    // Arrange - Testcontainers RabbitMQ
    await using var harness = new InMemoryTestHarness();
    var consumer = harness.Consumer<InventoryReservationConsumer>();
    await harness.Start();

    // Act
    await harness.InputQueueSendEndpoint.Send(new OrderCreatedEvent(orderId));

    // Assert
    (await consumer.Consumed.Any<OrderCreatedEvent>()).Should().BeTrue();
    (await harness.Published.Any<InventoryReservedEvent>()).Should().BeTrue();
}

// 3. Saga Test
[Fact]
public async Task OrderStateMachine_Should_Transition_Through_States()
{
    var harness = new InMemoryTestHarness();
    var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(new OrderStateMachine());

    await harness.Start();

    // Create order
    await harness.Bus.Publish(new OrderCreatedEvent(orderId));
    var instance = saga.Created.Select(x => x.Saga.OrderId == orderId).FirstOrDefault();
    instance.Should().NotBeNull();
    instance.CurrentState.Should().Be("Created");

    // Inventory reserved
    await harness.Bus.Publish(new InventoryReservedEvent(orderId));
    instance = saga.Sagas.Select(x => x.Saga.OrderId == orderId).First();
    instance.CurrentState.Should().Be("InventoryReserved");
}
```

### CloudAMQP Configuration

```csharp
// Program.cs
services.AddMassTransit(x =>
{
    x.AddConsumer<InventoryReservationConsumer>();
    x.AddConsumer<PaymentProcessingConsumer>();
    x.AddConsumer<NotificationConsumer>();
    x.AddConsumer<CompensationConsumer>();

    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .RedisRepository(r => r.ConnectionFactory(...));  // Redis dla Saga state

    x.UsingRabbitMq((context, cfg) =>
    {
        // CloudAMQP connection string
        cfg.Host(new Uri(config["CloudAMQP:Url"]), h =>
        {
            h.Username(config["CloudAMQP:Username"]);
            h.Password(config["CloudAMQP:Password"]);
        });

        cfg.UseMessageRetry(r => r.Exponential(5,
            TimeSpan.FromSeconds(1),
            TimeSpan.FromMinutes(5),
            TimeSpan.FromSeconds(5)));

        cfg.ConfigureEndpoints(context);
    });
});
```

### Referencje między projektami

```xml
<!-- Identity.Core.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\..\Shared\Shared.Abstractions\Shared.Abstractions.csproj" />
    <ProjectReference Include="..\Identity.Contracts\Identity.Contracts.csproj" />
  </ItemGroup>
</Project>

<!-- Identity.Infrastructure.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\Identity.Core\Identity.Core.csproj" />
    <ProjectReference Include="..\..\Shared\Shared.Infrastructure\Shared.Infrastructure.csproj" />
  </ItemGroup>
</Project>

<!-- Ordering.Core.csproj - może używać TYLKO Contracts innych modułów -->
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\..\Shared\Shared.Abstractions\Shared.Abstractions.csproj" />
    <ProjectReference Include="..\Ordering.Contracts\Ordering.Contracts.csproj" />
    <!-- Dostęp do Identity tylko przez Contracts! -->
    <ProjectReference Include="..\..\Identity\Identity.Contracts\Identity.Contracts.csproj" />
  </ItemGroup>
</Project>

<!-- ArchitecturePlayground.API.csproj - Composition Root -->
<Project Sdk="Microsoft.NET.Sdk.Web">
  <ItemGroup>
    <!-- Wszystkie moduły Infrastructure (rejestrują się w DI) -->
    <ProjectReference Include="..\Modules\Identity\Identity.Infrastructure\Identity.Infrastructure.csproj" />
    <ProjectReference Include="..\Modules\Catalog\Catalog.Infrastructure\Catalog.Infrastructure.csproj" />
    <ProjectReference Include="..\Modules\Ordering\Ordering.Infrastructure\Ordering.Infrastructure.csproj" />
    <ProjectReference Include="..\Modules\Basket\Basket.Infrastructure\Basket.Infrastructure.csproj" />
    <ProjectReference Include="..\Modules\Payment\Payment.Infrastructure\Payment.Infrastructure.csproj" />
    <ProjectReference Include="..\Modules\Notification\Notification.Infrastructure\Notification.Infrastructure.csproj" />
  </ItemGroup>
</Project>
```

### Architecture Tests (NetArchTest)

```csharp
[Fact]
public void Modules_Should_Not_Reference_Other_Modules_Core()
{
    var result = Types.InAssembly(typeof(OrderingCore).Assembly)
        .Should()
        .NotHaveDependencyOn("Identity.Core")
        .And()
        .NotHaveDependencyOn("Catalog.Core")
        .GetResult();

    result.IsSuccessful.Should().BeTrue();
}

[Fact]
public void Modules_Can_Only_Reference_Other_Modules_Contracts()
{
    // Ordering może referencjonować Identity.Contracts, ale nie Identity.Core
}
```

### Shared Kernel - Abstrakcje

| Projekt | Zawartość |
|---------|-----------|
| `Shared.Abstractions` | Entity, AggregateRoot, ValueObject, DomainEvent, ICommand, IQuery, Result |
| `Shared.Infrastructure` | BaseDbContext, UnitOfWork, EventBus, Caching, Behaviors (MediatR) |
| `Shared.Contracts` | Integration events współdzielone między modułami |

---

## Bounded Contexts (DDD)

### 1. Identity Context
- User registration/login
- OAuth 2.0 (Google, GitHub)
- JWT + Refresh Tokens
- Role-based (RBAC) + Claims-based auth
- Two-Factor Authentication (2FA)

### 2. Catalog Context (MongoDB)
- Products z dynamicznymi atrybutami
- Categories, Tags
- Full-text search
- Product recommendations (algorytmy)

### 3. Ordering Context (PostgreSQL)
- Order Aggregate Root
- Order Saga (state machine)
- Event Sourcing (historia zmian)
- CQRS (Command/Query separation)

### 4. Basket Context (Redis)
- Shopping cart (TTL)
- Wishlist
- Recently viewed

### 5. Payment Context
- Payment Gateway integration (Stripe mock)
- Transaction handling
- Idempotency

### 6. Notification Context
- Email (SendGrid/SMTP)
- WebSocket (SignalR) - real-time
- Push notifications

### 7. Analytics Context
- Sales reports (Dapper - raw SQL)
- User behavior tracking
- Dashboard aggregations

---

## Fazy Implementacji

### FAZA 1: Fundament (Tydzień 1-2)
- [ ] Solution structure (Clean Architecture)
- [ ] Docker Compose (PostgreSQL, MongoDB, Redis, RabbitMQ)
- [ ] BuildingBlocks: Result pattern, Domain primitives
- [ ] Global exception handling
- [ ] Logging (Serilog + Seq)
- [ ] Health checks

### FAZA 2: Identity Service (Tydzień 3)
- [ ] User entity, Value Objects
- [ ] ASP.NET Core Identity + EF Core
- [ ] JWT generation/validation
- [ ] OAuth 2.0 (Google provider)
- [ ] Refresh token rotation
- [ ] Rate limiting (Redis)
- [ ] Unit tests (xUnit + Moq)

### FAZA 3: Catalog Service (Tydzień 4)
- [ ] MongoDB integration
- [ ] Product Aggregate
- [ ] Repository pattern
- [ ] Specification pattern (filtering)
- [ ] Full-text search
- [ ] Integration tests (Testcontainers)

### FAZA 4: Basket Service (Tydzień 5)
- [ ] Redis integration
- [ ] Basket aggregate
- [ ] Cache-aside pattern
- [ ] Distributed caching strategies

### FAZA 5: Ordering Service (Tydzień 6-7)
- [ ] Order Aggregate (DDD tactical patterns)
- [ ] Domain Events
- [ ] CQRS z MediatR
- [ ] Event Sourcing (opcjonalnie Marten)
- [ ] Saga pattern (order workflow)
- [ ] Outbox pattern (transactional messaging)

### FAZA 6: Payment & Notification (Tydzień 8)
- [ ] Payment processing (mock Stripe)
- [ ] Idempotency keys
- [ ] SignalR hub (real-time updates)
- [ ] Email service

### FAZA 7: Analytics (Tydzień 9)
- [ ] Dapper raw SQL queries
- [ ] Materialized views (PostgreSQL)
- [ ] Time-series aggregations
- [ ] Export to CSV/Excel

### FAZA 8: Vue Frontend (Tydzień 10-11)
- [ ] Vue 3 + Vite + TypeScript
- [ ] Pinia state management
- [ ] Vue Router (guards)
- [ ] Axios + interceptors
- [ ] Component library (PrimeVue/Naive UI)
- [ ] Form validation (VeeValidate + Zod)
- [ ] Real-time (SignalR client)

### FAZA 9: API Gateway & Security (Tydzień 12)
- [ ] YARP reverse proxy
- [ ] Request aggregation
- [ ] OWASP hardening
- [ ] Security headers
- [ ] CORS configuration
- [ ] API versioning

### FAZA 10: DevOps & Cloud (Tydzień 13-14)
- [ ] Multi-stage Dockerfiles
- [ ] Docker Compose (dev/prod)
- [ ] GitHub Actions CI/CD
- [ ] Render deployment
- [ ] K3s manifests (VPS ready)
- [ ] Terraform (IaC basics)

### FAZA 11: Advanced Testing (Tydzień 15)
- [ ] Architecture tests (NetArchTest)
- [ ] Integration tests (Testcontainers)
- [ ] E2E tests (Playwright)
- [ ] Load tests (k6)
- [ ] Mutation testing (Stryker)

### FAZA 12: Polish & Documentation (Tydzień 16)
- [ ] OpenAPI/Swagger docs
- [ ] README z diagramami
- [ ] Architecture Decision Records (ADRs)
- [ ] Performance tuning
- [ ] Code review checklist

---

## Patterns & Practices Showcase

### Design Patterns
- Repository, Unit of Work
- Specification
- Factory, Builder
- Strategy (payment providers)
- Observer (domain events)
- Decorator (caching, logging)
- CQRS, Mediator
- Saga, Outbox

### SOLID w praktyce
- **S**: Każdy service = 1 odpowiedzialność
- **O**: Strategy pattern dla płatności
- **L**: Proper inheritance w domain
- **I**: Segregated interfaces (IReadRepository, IWriteRepository)
- **D**: Dependency Injection everywhere

### Security (OWASP Top 10)
- SQL Injection prevention (parameterized queries)
- XSS protection (CSP headers, sanitization)
- CSRF tokens
- Secure headers (HSTS, X-Frame-Options)
- Input validation (FluentValidation)
- Rate limiting
- Secrets management (User Secrets, Azure Key Vault)
- Password hashing (Argon2)
- JWT best practices

### Algorithms
- Search: Binary search, Full-text search
- Sorting: Custom comparers
- Recommendation: Collaborative filtering (basic)
- Caching: LRU, Cache invalidation strategies

---

## Struktura pierwszych plików do utworzenia

```
ArchitecturePlayground.sln

src/
├── ApiGateway/
│   └── ApiGateway.csproj
├── Services/
│   └── Identity/
│       ├── Identity.API/
│       │   ├── Controllers/
│       │   ├── Program.cs
│       │   └── Identity.API.csproj
│       ├── Identity.Application/
│       ├── Identity.Domain/
│       └── Identity.Infrastructure/
├── BuildingBlocks/
│   ├── BuildingBlocks.Common/
│   │   ├── Result.cs
│   │   ├── Entity.cs
│   │   ├── ValueObject.cs
│   │   ├── AggregateRoot.cs
│   │   └── DomainEvent.cs
│   └── BuildingBlocks.EventBus/

tests/
├── Identity.UnitTests/
├── Identity.IntegrationTests/
└── Architecture.Tests/

docker/
├── docker-compose.yml
├── docker-compose.override.yml
└── .env.example

.github/
└── workflows/
    └── ci.yml
```

---

## Kluczowe pliki do implementacji (Faza 1)

1. `ArchitecturePlayground.sln` - Solution file
2. `src/BuildingBlocks/BuildingBlocks.Common/Result.cs` - Result pattern
3. `src/BuildingBlocks/BuildingBlocks.Common/Entity.cs` - Base entity
4. `src/BuildingBlocks/BuildingBlocks.Common/ValueObject.cs` - Value object base
5. `src/BuildingBlocks/BuildingBlocks.Common/AggregateRoot.cs` - Aggregate root
6. `src/BuildingBlocks/BuildingBlocks.Common/DomainEvent.cs` - Domain events
7. `docker/docker-compose.yml` - Infrastructure containers
8. `src/Services/Identity/Identity.Domain/` - First domain model
9. `.editorconfig` - Code style consistency
10. `Directory.Build.props` - Shared MSBuild properties

---

## Hosting Plan - Hybrid Cloud Architecture

### Filozofia
**VPS + Managed Cloud Services** = najlepsze z obu światów:
- Aplikacje na VPS (tanie, nie zasypia, pełna kontrola)
- Bazy/cache w cloud (managed, pokazuje cloud skills)
- Multi-cloud experience (Azure, Supabase, MongoDB Atlas, etc.)

### Development (Local)
- Docker Compose (wszystko lokalnie)
- Hot reload (.NET + Vite)
- LocalStack (AWS emulator - opcjonalnie)

### Production - Hybrid Setup

#### VPS (Hetzner CX22 ~€4/mies.)
| Komponent | Opis |
|-----------|------|
| .NET API Services | Docker containers |
| Vue Frontend | Nginx static |
| API Gateway (YARP) | Reverse proxy |
| Traefik | Ingress + SSL (Let's Encrypt) |
| K3s | Lightweight Kubernetes |
| GitHub Actions Runner | Self-hosted (opcjonalnie) |

#### Cloud Managed Services (Free Tiers)

| Usługa | Provider | Free Tier | Pokazuje |
|--------|----------|-----------|----------|
| **PostgreSQL** | Supabase | 500MB, 2 projects | Supabase ecosystem |
| **MongoDB** | MongoDB Atlas | 512MB | NoSQL, Atlas UI |
| **Redis** | Upstash | 10K cmd/day | Serverless Redis |
| **Redis (alt)** | Redis Cloud | 30MB | Redis Enterprise |
| **RabbitMQ** | CloudAMQP | 1M msg/mth | Message brokers |
| **Secrets** | Azure Key Vault | 10K ops/mth | Azure integration |
| **Blob Storage** | Azure Blob | 5GB | Cloud storage |
| **Email** | SendGrid | 100/day | Transactional email |
| **Monitoring** | Azure App Insights | 5GB/mth | APM, Azure Portal |
| **Logs** | Seq Cloud | 1GB/day | Structured logging |
| **CI/CD** | GitHub Actions | 2000 min/mth | DevOps |

#### Azure Free Tier (12 months + Always Free)
Wykorzystujemy Azure do pokazania enterprise cloud skills:
- **Azure Key Vault** - secrets management
- **Azure Blob Storage** - pliki, obrazy produktów
- **Azure Application Insights** - monitoring, APM
- **Azure Service Bus** - alternatywa dla RabbitMQ (opcjonalnie)

### Architektura połączeń

```
┌─────────────────────────────────────────────────────────────────┐
│                         INTERNET                                 │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    VPS (Hetzner CX22)                           │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐              │
│  │   Traefik   │  │  Vue SPA    │  │ API Gateway │              │
│  │  (Ingress)  │  │  (Nginx)    │  │   (YARP)    │              │
│  └─────────────┘  └─────────────┘  └──────┬──────┘              │
│                                           │                      │
│  ┌────────────────────────────────────────┼────────────────┐    │
│  │              K3s Cluster               │                │    │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐▼──────────┐    │    │
│  │  │ Identity │ │ Catalog  │ │ Ordering │ │ Basket  │    │    │
│  │  │ Service  │ │ Service  │ │ Service  │ │ Service │    │    │
│  │  └────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬────┘    │    │
│  └───────┼────────────┼────────────┼────────────┼─────────┘    │
└──────────┼────────────┼────────────┼────────────┼──────────────┘
           │            │            │            │
           ▼            ▼            ▼            ▼
┌──────────────────────────────────────────────────────────────────┐
│                      CLOUD SERVICES                               │
│                                                                   │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐               │
│  │  Supabase   │  │   MongoDB   │  │   Upstash   │               │
│  │ PostgreSQL  │  │   Atlas     │  │    Redis    │               │
│  └─────────────┘  └─────────────┘  └─────────────┘               │
│                                                                   │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐               │
│  │  CloudAMQP  │  │ Azure Key   │  │ Azure Blob  │               │
│  │  RabbitMQ   │  │   Vault     │  │  Storage    │               │
│  └─────────────┘  └─────────────┘  └─────────────┘               │
│                                                                   │
│  ┌─────────────┐  ┌─────────────┐                                │
│  │  Azure App  │  │  SendGrid   │                                │
│  │  Insights   │  │   Email     │                                │
│  └─────────────┘  └─────────────┘                                │
└──────────────────────────────────────────────────────────────────┘
```

### Koszt miesięczny (szacowany)
| Pozycja | Koszt |
|---------|-------|
| Hetzner VPS CX22 | ~€4.50 |
| Domena (.dev) | ~€1/mth |
| Cloud Services | €0 (free tiers) |
| **RAZEM** | **~€5.50/mth** |

---

## Dokumentacja (docs/)

### Struktura dokumentacji

```
docs/
├── README.md                        # Główny opis projektu
├── GETTING_STARTED.md               # Quick start guide
├── CONTRIBUTING.md                  # Jak kontrybuować
│
├── architecture/
│   ├── README.md                    # Przegląd architektury
│   ├── C4-Context.puml              # C4 Level 1 - System Context
│   ├── C4-Container.puml            # C4 Level 2 - Containers
│   ├── C4-Component-Identity.puml   # C4 Level 3 - Components
│   ├── C4-Component-Ordering.puml
│   ├── C4-Component-Catalog.puml
│   └── tech-stack.md                # Opis technologii
│
├── adr/                             # Architecture Decision Records
│   ├── README.md                    # Index ADRów
│   ├── template.md                  # Szablon ADR
│   ├── 0001-use-clean-architecture.md
│   ├── 0002-use-cqrs-for-ordering.md
│   ├── 0003-mongodb-for-catalog.md
│   ├── 0004-event-sourcing-orders.md
│   ├── 0005-jwt-refresh-token-rotation.md
│   ├── 0006-outbox-pattern.md
│   ├── 0007-hybrid-cloud-hosting.md
│   └── ...
│
├── diagrams/
│   ├── domain-model/
│   │   ├── identity-domain.puml
│   │   ├── ordering-domain.puml
│   │   ├── catalog-domain.puml
│   │   └── payment-domain.puml
│   ├── sequence/
│   │   ├── user-registration.puml
│   │   ├── order-flow.puml
│   │   ├── payment-flow.puml
│   │   └── checkout-saga.puml
│   ├── infrastructure/
│   │   ├── deployment.puml
│   │   ├── cloud-services.puml
│   │   └── ci-cd-pipeline.puml
│   └── erd/
│       ├── identity-erd.puml
│       └── ordering-erd.puml
│
├── api/
│   ├── openapi.yaml                 # OpenAPI 3.0 spec (generated)
│   ├── postman/
│   │   └── ArchitecturePlayground.postman_collection.json
│   └── examples/
│       ├── create-order.http
│       ├── login.http
│       └── ...
│
├── runbooks/                        # Operational guides
│   ├── deployment.md
│   ├── troubleshooting.md
│   ├── scaling.md
│   └── disaster-recovery.md
│
└── security/
    ├── threat-model.md
    ├── owasp-checklist.md
    └── security-headers.md
```

### C4 Model - Diagramy Architektury

#### Level 1: System Context
```plantuml
@startuml C4-Context
!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Context.puml

Person(customer, "Customer", "Kupuje produkty")
Person(admin, "Admin", "Zarządza sklepem")

System(ecommerce, "E-Commerce Platform", "Platforma zakupowa")

System_Ext(payment, "Payment Gateway", "Stripe")
System_Ext(email, "Email Service", "SendGrid")
System_Ext(oauth, "OAuth Providers", "Google, GitHub")

Rel(customer, ecommerce, "Przegląda, kupuje")
Rel(admin, ecommerce, "Zarządza")
Rel(ecommerce, payment, "Przetwarza płatności")
Rel(ecommerce, email, "Wysyła emaile")
Rel(ecommerce, oauth, "Autentykacja")
@enduml
```

#### Level 2: Container Diagram
```plantuml
@startuml C4-Container
!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Container.puml

Person(customer, "Customer")

System_Boundary(platform, "E-Commerce Platform") {
    Container(spa, "Vue SPA", "Vue 3, TypeScript", "Frontend aplikacji")
    Container(gateway, "API Gateway", "YARP", "Routing, Auth")

    Container(identity, "Identity Service", ".NET 9", "Auth, Users")
    Container(catalog, "Catalog Service", ".NET 9", "Products")
    Container(ordering, "Ordering Service", ".NET 9", "Orders")
    Container(basket, "Basket Service", ".NET 9", "Shopping Cart")
    Container(payment, "Payment Service", ".NET 9", "Payments")
    Container(notification, "Notification Service", ".NET 9", "Emails, Push")

    ContainerDb(postgres, "PostgreSQL", "Supabase", "Orders, Users")
    ContainerDb(mongodb, "MongoDB", "Atlas", "Products")
    ContainerDb(redis, "Redis", "Upstash", "Cache, Sessions")
    ContainerQueue(rabbitmq, "RabbitMQ", "CloudAMQP", "Events")
}

Rel(customer, spa, "HTTPS")
Rel(spa, gateway, "HTTPS/JSON")
Rel(gateway, identity, "gRPC/REST")
Rel(gateway, catalog, "gRPC/REST")
Rel(gateway, ordering, "gRPC/REST")
@enduml
```

### Architecture Decision Records (ADR)

#### Szablon ADR (template.md)
```markdown
# ADR-XXXX: [Tytuł decyzji]

## Status
[Proposed | Accepted | Deprecated | Superseded]

## Context
[Opis problemu i kontekstu]

## Decision
[Podjęta decyzja]

## Consequences
### Positive
- [Korzyść 1]
- [Korzyść 2]

### Negative
- [Wada 1]
- [Wada 2]

## Alternatives Considered
1. [Alternatywa 1] - odrzucona bo...
2. [Alternatywa 2] - odrzucona bo...
```

#### Przykładowe ADRs do utworzenia

| ADR | Tytuł | Decyzja |
|-----|-------|---------|
| 0001 | Clean Architecture | Separacja warstw Domain/Application/Infrastructure/API |
| 0002 | CQRS w Ordering | MediatR dla command/query separation |
| 0003 | MongoDB dla Catalog | Elastyczne atrybuty produktów |
| 0004 | Event Sourcing | Marten dla historii zamówień |
| 0005 | JWT Strategy | Access token 15min + Refresh token rotation |
| 0006 | Outbox Pattern | Transactional messaging via EF Core |
| 0007 | Hybrid Cloud | VPS + managed cloud services |
| 0008 | API Versioning | URL versioning (/api/v1/) |
| 0009 | Validation Strategy | FluentValidation + Domain validation |
| 0010 | Error Handling | Problem Details (RFC 7807) |

### OpenAPI / Swagger

Automatyczna generacja z kodem:
- **Swashbuckle** dla OpenAPI spec
- **Scalar** lub **Swagger UI** dla dokumentacji
- Export do `docs/api/openapi.yaml`
- Postman collection generation

### README.md - Główny plik

```markdown
# 🏗️ ArchitecturePlayground

> E-Commerce Platform demonstrating enterprise architecture patterns

## 🎯 What This Project Demonstrates

| Category | Technologies & Patterns |
|----------|------------------------|
| Architecture | Clean Architecture, DDD, CQRS, Event Sourcing |
| Backend | .NET 9, ASP.NET Core, MediatR, FluentValidation |
| Frontend | Vue 3, TypeScript, Pinia, TailwindCSS |
| Databases | PostgreSQL, MongoDB, Redis |
| Messaging | RabbitMQ, MassTransit |
| Security | OAuth 2.0, JWT, OWASP Top 10 |
| DevOps | Docker, K3s, GitHub Actions, Terraform |
| Cloud | Azure, Supabase, MongoDB Atlas, Upstash |
| Testing | xUnit, Testcontainers, Playwright |

## 🏛️ Architecture

[Diagram C4]

## 🚀 Quick Start

\`\`\`bash
# Clone
git clone https://github.com/user/ArchitecturePlayground.git

# Start infrastructure
docker-compose up -d

# Run API
dotnet run --project src/ApiGateway/ApiGateway

# Run Frontend
cd src/Web/vue-storefront && npm run dev
\`\`\`

## 📚 Documentation

- [Architecture Overview](docs/architecture/README.md)
- [API Documentation](docs/api/README.md)
- [ADRs](docs/adr/README.md)
- [Deployment Guide](docs/runbooks/deployment.md)

## 🧪 Testing

\`\`\`bash
dotnet test                           # All tests
dotnet test --filter Category=Unit    # Unit only
dotnet test --filter Category=Integration
\`\`\`

## 📊 Project Status

![Build](https://github.com/user/repo/actions/workflows/ci.yml/badge.svg)
![Coverage](https://img.shields.io/badge/coverage-90%25-green)
![License](https://img.shields.io/badge/license-MIT-blue)
```

---

## Metryki sukcesu projektu

- [ ] 90%+ code coverage w domain layer
- [ ] Wszystkie OWASP Top 10 zaadresowane
- [ ] < 200ms response time (P95)
- [ ] Pełna dokumentacja OpenAPI
- [ ] Architecture tests passing
- [ ] Zero critical security issues (SAST)
- [ ] Working CI/CD pipeline
- [ ] Kubernetes-ready deployment
- [ ] Kompletne diagramy C4 (wszystkie poziomy)
- [ ] Minimum 10 ADRs dokumentujących decyzje
- [ ] README z badges i quick start
- [ ] Postman collection dla wszystkich endpoints
