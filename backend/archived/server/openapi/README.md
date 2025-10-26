# OpenAPI Specification

This directory contains the OpenAPI (Swagger) specification for the WasteNaut Admin API.

## 📄 Files

- **`admin-api.yaml`** - Complete API specification with all endpoints, schemas, and examples

## 🔧 Code Generation

### Generate C# Client

```bash
# Using OpenAPI Generator
openapi-generator generate \
  -i admin-api.yaml \
  -g csharp \
  -o ./generated-client \
  --additional-properties=packageName=WasteNaut.Admin.Client

# Using NSwag
nswag openapi2csclient \
  /input:admin-api.yaml \
  /output:GeneratedClient.cs \
  /namespace:WasteNaut.Admin.Client
```

### Generate TypeScript Client

```bash
# Using OpenAPI Generator
openapi-generator generate \
  -i admin-api.yaml \
  -g typescript-fetch \
  -o ./generated-client-ts

# Using NSwag
nswag openapi2tsclient \
  /input:admin-api.yaml \
  /output:GeneratedClient.ts
```

### Generate Documentation

```bash
# Generate HTML documentation
redoc-cli build admin-api.yaml -o api-docs.html

# Generate Postman collection
openapi2postmanv2 -s admin-api.yaml -o postman-collection.json
```

## 📋 API Overview

### Authentication
- `POST /login` - Admin login with JWT token
- `GET /profile` - Get current admin profile
- `POST /logout` - Logout (token invalidation)

### User Management
- `GET /users` - List users with filtering
- `GET /users/{id}` - Get user details
- `POST /users/{id}/verify` - Verify user account
- `POST /users/{id}/suspend` - Suspend user
- `POST /users/{id}/unsuspend` - Unsuspend user
- `POST /users/{id}/impersonate` - Start impersonation

### Organization Management
- `GET /organizations` - List organizations
- `GET /organizations/{id}` - Get organization details
- `POST /organizations/{id}/approve` - Approve organization
- `POST /organizations/{id}/reject` - Reject organization
- `PUT /organizations/{id}/capacity` - Set capacity

### Report Management
- `GET /reports` - List reports with filtering
- `GET /reports/{id}` - Get report details
- `PUT /reports/{id}/status` - Update report status
- `POST /reports/{id}/notes` - Add note to report
- `POST /reports/{id}/resolve` - Resolve report
- `POST /reports/assign` - Assign reports to admin

### Match Management
- `GET /matches` - List AI matches
- `GET /matches/{id}` - Get match details
- `POST /matches/{id}/accept` - Accept match
- `POST /matches/{id}/reject` - Reject match
- `POST /matches/{id}/override` - Override match
- `POST /matches/generate` - Generate new matches

### Dashboard
- `GET /dashboard/activity` - Recent activity
- `GET /dashboard/alerts` - System alerts

### Settings
- `GET /settings/roles` - Admin roles
- `GET /settings/notifications/templates` - Notification templates
- `GET /settings/system` - System settings
- `PUT /settings/system` - Update system settings
- `PUT /settings/ai` - Update AI settings
- `PUT /settings/notifications` - Update notification settings

### Audit
- `GET /audit` - Audit logs

## 🔐 Security

All endpoints (except login) require JWT authentication:

```http
Authorization: Bearer <jwt-token>
```

### JWT Token Structure

```json
{
  "sub": "admin-id",
  "email": "admin@wastenaut.test",
  "role": "super_admin",
  "exp": 1640995200
}
```

## 📊 Data Models

### Core Entities

- **Admin** - Admin users with roles
- **User** - Regular users (individuals, organizations, donors)
- **Organization** - Organization profiles
- **Donation** - Food/equipment donations
- **Report** - User reports with evidence
- **Match** - AI-generated matches
- **AuditLog** - System activity tracking

### Request/Response DTOs

- **LoginRequestDto** - Login credentials
- **LoginResponseDto** - JWT token and admin info
- **StatusUpdateDto** - Status change requests
- **NoteRequestDto** - Add note requests
- **MatchOverrideDto** - Match override requests

## 🧪 Testing

### Example Requests

#### Login
```bash
curl -X POST "https://localhost:5001/api/admin/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@wastenaut.test",
    "password": "admin123",
    "rememberMe": true
  }'
```

#### Get Users
```bash
curl -X GET "https://localhost:5001/api/admin/users?page=1&limit=20&status=verified" \
  -H "Authorization: Bearer <jwt-token>"
```

#### Verify User
```bash
curl -X POST "https://localhost:5001/api/admin/users/1/verify" \
  -H "Authorization: Bearer <jwt-token>"
```

### Mock Limitations

When using the mock API (browser-side), note that:

- **curl requests won't work** - mocks only work in browser
- **Real authentication** is not implemented
- **Data persistence** is session-only
- **File uploads** are not supported
- **WebSocket connections** are not supported

## 🔄 Versioning

The API uses semantic versioning:

- **v1.0.0** - Initial release with all core features
- **Future versions** will maintain backward compatibility

## 📝 Documentation

The OpenAPI spec includes:

- **Complete endpoint documentation**
- **Request/response schemas**
- **Authentication requirements**
- **Error response formats**
- **Example payloads**
- **Security schemes**

## 🚀 Integration

### Frontend Integration

1. **Generate TypeScript client** from OpenAPI spec
2. **Replace mock API calls** with generated client
3. **Handle authentication** with JWT tokens
4. **Implement error handling** for API responses

### Backend Integration

1. **Implement controllers** based on OpenAPI spec
2. **Add service layer** for business logic
3. **Configure database** with Entity Framework
4. **Set up authentication** with JWT middleware

## 📞 Support

For API questions:

1. **Check the OpenAPI spec** for endpoint details
2. **Review example requests** in this README
3. **Test with sample data** from database seed
4. **Check audit logs** for debugging
