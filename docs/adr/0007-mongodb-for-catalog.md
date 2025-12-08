# ADR-0007: MongoDB for Product Catalog

## Status

Accepted

## Date

2025-01

## Context

The Product Catalog module needs to store products with:
- Dynamic attributes (different product types have different properties)
- Full-text search capabilities
- Frequent reads, infrequent writes
- Schema flexibility for future product types

Example: Electronics have "warranty period", "power consumption", while Clothing has "size", "material", "care instructions".

PostgreSQL requires either:
- EAV (Entity-Attribute-Value) pattern - poor performance
- JSONB columns - loses type safety and validation
- Multiple tables per product type - maintenance nightmare

## Decision

We will use **MongoDB** for the Catalog module.

**Schema design:**
```json
{
  "_id": "ObjectId",
  "name": "Product Name",
  "description": "...",
  "category": "Electronics",
  "price": 99.99,
  "attributes": {
    "warranty": "2 years",
    "power": "500W"
  },
  "tags": ["tag1", "tag2"],
  "images": ["url1", "url2"],
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}
```

**Indexes:**
- Text index on `name`, `description`, `tags`
- Compound index on `category + price`
- Index on `createdAt` (for sorting)

## Consequences

### Positive

- **Schema flexibility**: Add attributes without migrations
- **Full-text search**: Built-in text search with stemming
- **Performance**: Denormalized data = fewer joins
- **Horizontal scaling**: Sharding built-in (if needed later)
- **Developer experience**: JSON-first, natural for APIs

### Negative

- **Polyglot persistence**: Need to manage two database types
- **Transactions**: No distributed transactions with PostgreSQL
- **Learning curve**: Team needs MongoDB expertise
- **Consistency**: Eventual consistency in some scenarios

### Neutral

- MongoDB Atlas free tier sufficient for development (512MB)
- Need separate backup strategy for MongoDB
- Document size limit (16MB) - unlikely to hit for products

## Alternatives Considered

### Alternative 1: PostgreSQL with JSONB

Store dynamic attributes in JSONB column.

**Rejected because:**
- Poor query performance on nested JSON
- Difficult to maintain indexes on dynamic fields
- Type safety issues in application code
- No built-in full-text search on JSON fields

### Alternative 2: PostgreSQL with EAV pattern

Separate table for attributes (product_id, key, value).

**Rejected because:**
- Terrible query performance (multiple joins)
- Schema becomes unreadable
- Difficult to ensure data integrity
- Over-engineering for our use case

### Alternative 3: Elasticsearch

Use Elasticsearch as primary store for catalog.

**Rejected because:**
- Over-engineering - we don't need advanced search features yet
- Higher operational complexity
- Elasticsearch is better as secondary index, not primary store
- Cost (no good free tier)

## References

- [MongoDB Schema Design Best Practices](https://www.mongodb.com/developer/products/mongodb/schema-design-anti-pattern-summary/)
- [Polyglot Persistence - Martin Fowler](https://martinfowler.com/bliki/PolyglotPersistence.html)
- [MongoDB Atlas Free Tier](https://www.mongodb.com/cloud/atlas/pricing)
