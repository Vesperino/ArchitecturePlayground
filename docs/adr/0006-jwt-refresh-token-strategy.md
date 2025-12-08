# ADR-0006: JWT + Refresh Token Strategy

## Status

Accepted

## Date

2025-01

## Context

The application needs a secure authentication mechanism that:
- Allows stateless authentication for APIs
- Supports mobile and web clients
- Minimizes attack surface if tokens are compromised
- Provides good UX (users don't need to re-login frequently)
- Supports token revocation

JWT (JSON Web Tokens) are widely used for API authentication, but long-lived JWTs pose security risks if compromised.

## Decision

We will implement a **dual-token strategy**:

1. **Access Token (JWT)**:
   - Short-lived (15 minutes)
   - Contains user claims (id, email, roles)
   - Used for API requests
   - Stateless (not stored in database)

2. **Refresh Token**:
   - Long-lived (7 days)
   - Stored in database (can be revoked)
   - Used only to obtain new access tokens
   - Implements rotation (new refresh token on each use)

**Token Rotation Flow:**
```
1. Login → Issue Access Token (15min) + Refresh Token (7 days)
2. Access Token expires → Use Refresh Token
3. Refresh endpoint → Issue NEW Access Token + NEW Refresh Token
4. Old Refresh Token is invalidated
```

## Consequences

### Positive

- **Security**: Short-lived access tokens minimize damage if compromised
- **UX**: Users stay logged in for 7 days without re-entering credentials
- **Revocation**: Refresh tokens can be revoked (logout, suspicious activity)
- **Stateless API**: Access tokens don't require database lookups
- **Token reuse detection**: Refresh token rotation detects theft

### Negative

- **Database storage**: Refresh tokens must be persisted
- **Complexity**: Two token types to manage
- **Clock skew**: Requires synchronized clocks

### Neutral

- Requires refresh token cleanup job (delete expired tokens)
- Mobile apps need secure storage (Keychain/Keystore)

## Alternatives Considered

### Alternative 1: Long-lived JWT only

Single JWT valid for 7 days.

**Rejected because:**
- If token is stolen, attacker has access for 7 days
- No way to revoke without database lookups
- Violates principle of least privilege

### Alternative 2: Session-based authentication

Traditional server-side sessions with cookies.

**Rejected because:**
- Not suitable for mobile apps
- Requires sticky sessions in load-balanced setup
- Difficult to scale horizontally
- CSRF protection complexity

### Alternative 3: Short-lived JWT + sliding window

Extend token expiration on each use.

**Rejected because:**
- Requires state (tracking last use time)
- Loses stateless benefit of JWT
- Token can live forever with active use

## References

- [RFC 6749 - OAuth 2.0](https://datatracker.ietf.org/doc/html/rfc6749)
- [OWASP JWT Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/JSON_Web_Token_for_Java_Cheat_Sheet.html)
- [Auth0 - Refresh Token Rotation](https://auth0.com/docs/secure/tokens/refresh-tokens/refresh-token-rotation)
