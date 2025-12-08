# ADR-0004: Outbox Pattern for Messaging

## Status

Accepted

## Date

2025-01

## Context

We need reliable messaging between modules and to external systems (RabbitMQ). The challenge:

```csharp
// Without Outbox - DANGEROUS!
await _dbContext.SaveChangesAsync();  // 1. Save order ✅
await _bus.Publish(orderCreatedEvent); // 2. Publish event - what if this fails? ❌
```

If step 2 fails, we have an order in the database but no event published. This leads to inconsistent state.

## Decision

We will implement the **Outbox Pattern**:

1. Save entity AND outbox message in the same transaction
2. Background worker polls outbox and publishes to RabbitMQ
3. Mark messages as processed after successful publish

```csharp
// With Outbox - SAFE!
// In DbContext.SaveChangesAsync():
// - Collect domain events from aggregates
// - Convert to OutboxMessage entities
// - Save everything in one transaction

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Content { get; set; }  // JSON
    public DateTime OccurredOn { get; set; }
    public DateTime? ProcessedOn { get; set; }
}
```

## Consequences

### Positive

- **Atomic operations**: Entity + Event saved together or not at all
- **At-least-once delivery**: Messages will eventually be published
- **Resilience**: RabbitMQ downtime doesn't affect writes
- **Auditability**: Outbox table is an event log

### Negative

- **Eventual consistency**: Events are published with delay (1s polling)
- **Increased storage**: Outbox table grows (needs cleanup)
- **Complexity**: Additional background worker

### Neutral

- Consumers must be idempotent (may receive duplicates)
- Need to handle outbox cleanup (archive old messages)

## Alternatives Considered

### Alternative 1: Distributed Transactions (2PC)

Use database + message broker transaction.

**Rejected because:**
- RabbitMQ doesn't support 2PC
- Complex and slow
- Single point of failure

### Alternative 2: Publish Before Commit

Publish event, then commit database.

**Rejected because:**
- Event published but DB commit fails = phantom event
- Worse than our problem

### Alternative 3: Change Data Capture (CDC)

Use Debezium to capture database changes.

**Rejected because:**
- Additional infrastructure
- Overkill for our scale
- Harder to debug

## References

- [Outbox Pattern - microservices.io](https://microservices.io/patterns/data/transactional-outbox.html)
- [Reliable Messaging with Outbox - Chris Richardson](https://chrisrichardson.net/post/microservices/2019/07/09/developing-sagas-part-1.html)
