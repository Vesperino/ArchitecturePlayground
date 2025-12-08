# Mermaid Diagrams Migration Design

## Date
2025-01-08

## Status
Approved

## Decision
Migrate all PlantUML diagrams to Mermaid and create comprehensive documentation.

## Scope

### C4 Architecture (convert from PlantUML)
1. Context Diagram
2. Container Diagram
3. Component Diagram (Identity)
4. Component Diagram (Ordering)

### Business Flows (new)
5. Order Flow
6. Checkout Saga State Machine
7. Authentication Flow
8. Payment Flow

### Data Architecture (new)
9. Data Ownership Map
10. PostgreSQL Schema (ER)
11. MongoDB Schema
12. Redis Data Structures

### Communication (new)
13. Module Communication Matrix
14. Outbox Pattern Flow
15. SignalR Real-time Flow

## File Organization
- All diagrams inline in `docs/architecture/README.md`
- Logical sections with headers
- If too large, split by category

## Implementation
- Delete old `.puml` files after migration
- Update all references
- Verify GitHub rendering
