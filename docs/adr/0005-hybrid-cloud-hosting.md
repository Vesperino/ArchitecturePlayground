# ADR-0005: Hybrid Cloud Hosting Strategy

## Status

Accepted

## Date

2025-01

## Context

We need a hosting strategy that:
- Is cost-effective (portfolio project budget)
- Demonstrates cloud skills for interviews
- Provides production-like environment
- Doesn't sleep/shut down (unlike some free tiers)
- Shows multi-cloud experience

## Decision

We will use a **Hybrid Cloud** approach:

### VPS (Hetzner CX22 ~€5/month)
- .NET API (Docker container)
- Vue Frontend (Nginx static)
- Traefik (SSL/Ingress)

### Managed Cloud Services (Free Tiers)
| Service | Provider | Purpose |
|---------|----------|---------|
| PostgreSQL | Supabase | Main database |
| MongoDB | Atlas | Catalog (flexible schema) |
| Redis | Upstash | Cache, sessions |
| RabbitMQ | CloudAMQP | Message broker |
| Secrets | Azure Key Vault | Secrets management |
| Storage | Azure Blob | Product images |
| Monitoring | Azure App Insights | APM |
| Email | SendGrid | Transactional email |

## Consequences

### Positive

- **Cost-effective**: ~€5/month total
- **Multi-cloud experience**: Azure, Supabase, MongoDB Atlas
- **No sleeping**: VPS runs 24/7
- **Demonstrates skills**: Cloud integration, managed services
- **Production-like**: Real databases, real message broker

### Negative

- **Multiple accounts**: Need to manage several cloud accounts
- **Complexity**: Different connection strings per environment
- **Free tier limits**: May hit limits with heavy usage

### Neutral

- Good interview talking point
- Can migrate to full cloud if needed

## Alternatives Considered

### Alternative 1: Full Azure

Deploy everything to Azure (App Service, Azure SQL, etc.)

**Rejected because:**
- Expensive beyond free tier
- Less multi-cloud experience
- Free tier limits restrictive

### Alternative 2: Full VPS (Docker Compose)

Run all databases on VPS.

**Rejected because:**
- No cloud experience to show
- Must manage databases ourselves
- Resource constraints on cheap VPS

### Alternative 3: Serverless (AWS Lambda, Azure Functions)

Use serverless for API.

**Rejected because:**
- Cold starts impact UX
- Limited execution time
- Complex local development

## References

- [Hetzner Cloud Pricing](https://www.hetzner.com/cloud)
- [Supabase Free Tier](https://supabase.com/pricing)
- [MongoDB Atlas Free Tier](https://www.mongodb.com/pricing)
- [Azure Free Account](https://azure.microsoft.com/free/)
