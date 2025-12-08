# ADR-0001: Modular Monolith Architecture

## Status

Accepted

## Date

2025-01

## Context

We need to choose an architectural style for our e-commerce platform. The system needs to:
- Handle up to 10,000 concurrent users
- Be maintainable by a small team
- Demonstrate enterprise architecture patterns
- Be cost-effective to host and operate
- Allow future scaling if needed

The industry has seen a shift away from microservices for many use cases, with companies like Amazon Prime Video publicly sharing how they reduced costs by 90% by moving from microservices to a monolith.

## Decision

We will use **Modular Monolith** architecture with clear module boundaries.

Each module:
- Has its own `.Core`, `.Infrastructure`, and `.Contracts` projects
- Owns its data (separate DbContext per module)
- Communicates with other modules only through `.Contracts` (interfaces, DTOs, events)
- Can be extracted to a microservice if scaling demands

## Consequences

### Positive

- **Simpler deployment**: Single deployable unit
- **Easier debugging**: Single process, no distributed tracing needed
- **ACID transactions**: Within module boundaries
- **Lower operational costs**: No service mesh, no API gateway complexity
- **Faster development**: No network overhead between modules
- **Clear migration path**: Modules can become microservices later

### Negative

- **Single point of failure**: Entire app goes down together
- **Scaling limitations**: Must scale entire app, not individual modules
- **Shared runtime**: One module's memory leak affects all

### Neutral

- Requires discipline to maintain module boundaries
- Architecture tests needed to enforce boundaries

## Alternatives Considered

### Alternative 1: Microservices

Each bounded context as separate service with its own database.

**Rejected because:**
- Over-engineering for our scale
- Higher operational complexity
- Distributed transaction challenges
- Higher hosting costs

### Alternative 2: Traditional Layered Monolith

Single monolith with horizontal layers (Controllers, Services, Repositories).

**Rejected because:**
- Tight coupling between features
- Difficult to extract modules later
- Changes ripple across layers

## References

- [What Is a Modular Monolith? - Milan Jovanovic](https://www.milanjovanovic.tech/blog/what-is-a-modular-monolith)
- [Amazon Prime Video Monolith Case Study - The New Stack](https://thenewstack.io/return-of-the-monolith-amazon-dumps-microservices-for-video-monitoring/)
- [Amazon Prime Video Cost Reduction - DEV Community](https://dev.to/indika_wimalasuriya/amazon-prime-videos-90-cost-reduction-throuh-moving-to-monolithic-k4a)
