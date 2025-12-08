# ADR-0008: API Versioning Strategy

## Status

Accepted

## Date

2025-01

## Context

The API will evolve over time with:
- New features and endpoints
- Breaking changes to existing endpoints
- Multiple client versions (web, mobile) in use simultaneously

We need a versioning strategy that:
- Allows backward compatibility
- Is clear to API consumers
- Doesn't overcomplicate codebase
- Follows REST best practices

## Decision

We will use **URL versioning** with the format `/api/v{version}/{resource}`.

**Examples:**
```
GET /api/v1/products
POST /api/v1/orders
GET /api/v2/products  (when breaking changes needed)
```

**Version lifecycle:**
- New APIs start at `v1`
- Breaking changes require new version (v2, v3, etc.)
- Non-breaking changes added to current version
- Old versions deprecated after 6 months notice
- Maximum 2 versions supported simultaneously

**Breaking changes:**
- Removing fields from response
- Changing field types
- Changing HTTP status codes
- Renaming endpoints

**Non-breaking changes:**
- Adding new endpoints
- Adding optional request fields
- Adding fields to response

## Consequences

### Positive

- **Explicit**: Version is clearly visible in URL
- **Simple**: Easy to understand for API consumers
- **Cacheable**: Each version has distinct URLs
- **Testable**: Easy to test multiple versions
- **Documentation**: Swagger can show multiple versions

### Negative

- **URL pollution**: Multiple versions in URL space
- **Code duplication**: Some code duplicated between versions
- **Routing complexity**: Need to route different versions

### Neutral

- Requires discipline to avoid version sprawl
- Need clear deprecation policy
- API documentation must clarify what changed between versions

## Alternatives Considered

### Alternative 1: Header versioning

Version specified in custom header (e.g., `X-API-Version: 1`).

**Rejected because:**
- Not cacheable (HTTP caches ignore custom headers)
- Harder to test (need to set headers manually)
- Not visible in browser/Swagger UI
- Violates principle of least surprise

### Alternative 2: Query parameter versioning

Version as query param (e.g., `/api/products?version=1`).

**Rejected because:**
- Inconsistent (some endpoints might forget version)
- Pollutes query string
- Not RESTful (version is not a resource filter)

### Alternative 3: Content negotiation

Version in Accept header (e.g., `Accept: application/vnd.api.v1+json`).

**Rejected because:**
- Over-engineering for our use case
- Difficult for non-technical API consumers
- Poor tooling support (Swagger, Postman)
- Caching complications

### Alternative 4: No versioning

Just evolve API without versions.

**Rejected because:**
- Impossible to introduce breaking changes safely
- Forces mobile apps to update immediately
- No backward compatibility

## References

- [Microsoft API Versioning Guidelines](https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design#versioning-a-restful-web-api)
- [Stripe API Versioning](https://stripe.com/docs/api/versioning)
- [REST API Versioning Strategies](https://www.freecodecamp.org/news/how-to-version-a-rest-api/)
