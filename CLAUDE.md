# CLAUDE.md - AI Instructions

## Project

E-Commerce platform: Modular Monolith + Vertical Slice Architecture (.NET 9 + Vue 3).

## Codebase Map

```
src/
├── Bootstrapper/           # API entry point
├── Modules/
│   ├── Catalog/            # Products, categories (MongoDB)
│   ├── Identity/           # Auth, users (PostgreSQL)
│   ├── Orders/             # Order processing (PostgreSQL)
│   └── Shared/             # Cross-cutting concerns.
└── Web/vue-storefront/     # Frontend (Vue 3)
```

Each module: `Core/` (domain+features), `Infrastructure/` (persistence), `Contracts/` (public API).

## Commands

```bash
dotnet run --project src/Bootstrapper/ArchitecturePlayground.API
dotnet test
docker-compose up -d
```

## Documentation

- Architecture details → `docs/architecture/README.md`
- ADRs → `docs/adr/`
- API specs → OpenAPI/Swagger (runtime)

---

## CRITICAL: Skills Workflow

YOU MUST use skills. Check before ANY task.

| Context | Skill |
|---------|-------|
| Before writing code | `superpowers:brainstorming` |
| Implementing .NET code | `superpowers:test-driven-development` + `dotnet-coding-standards` |
| Debugging | `superpowers:systematic-debugging` |
| Before claiming done | `superpowers:verification-before-completion` |
| After tests pass | `post-implementation-docs` |
| Creating plans | `superpowers:writing-plans` |
| Code review | `superpowers:requesting-code-review` |

**If you think "skill is overkill" → WRONG. Use it.**

---

## Module Boundaries (ENFORCE)

```
ALLOWED:    Core → Contracts, Shared.Abstractions
FORBIDDEN:  Core → OtherModule.Core/Infrastructure
```

## File Organization (ENFORCE)

**One class per file.** Name file = class name.
Exception: small tightly-coupled helper/data classes only.

---

## When Uncertain

1. Stop
2. Present options with pros/cons
3. Ask for decision
4. Document in ADR

**Don't blindly agree.**
