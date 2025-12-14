# Architecture Decision Records

This directory contains Architecture Decision Records (ADRs) for the ArchitecturePlayground project.

## What is an ADR?

An Architecture Decision Record captures an important architectural decision made along with its context and consequences.

## ADR Index

| ADR | Title | Status | Date |
|-----|-------|--------|------|
| [0001](0001-modular-monolith-architecture.md) | Modular Monolith Architecture | Accepted | 2025-01 |
| [0002](0002-vertical-slice-architecture.md) | Vertical Slice Architecture | Accepted | 2025-01 |
| [0003](0003-cqrs-with-mediatr.md) | CQRS with MediatR | Accepted | 2025-01 |
| [0004](0004-outbox-pattern-for-messaging.md) | Outbox Pattern for Messaging | Accepted | 2025-01 |
| [0005](0005-hybrid-cloud-hosting.md) | Hybrid Cloud Hosting | Accepted | 2025-01 |
| [0006](0006-jwt-refresh-token-strategy.md) | JWT + Refresh Token Strategy | Accepted | 2025-01 |
| [0007](0007-mongodb-for-catalog.md) | MongoDB for Product Catalog | Accepted | 2025-01 |
| [0008](0008-api-versioning-strategy.md) | API Versioning Strategy | Accepted | 2025-01 |
| [0009](0009-validation-strategy.md) | Validation Strategy | Accepted | 2025-01 |
| [0010](0010-error-handling-strategy.md) | Error Handling Strategy | Accepted | 2025-01 |
| [0011](0011-pure-ddd-identity-module.md) | Pure DDD for Identity Module | Accepted | 2025-12 |

## Template

See [template.md](template.md) for the ADR template.

## How to Create a New ADR

1. Copy `template.md` to `XXXX-title-with-dashes.md`
2. Fill in all sections
3. Update this README with the new ADR
4. Commit with message: `docs: add ADR-XXXX title`
