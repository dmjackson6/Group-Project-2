# WasteNaut Admin Backend

This directory contains the backend implementation for the WasteNaut Admin system, including API specifications, database schemas, and C# ASP.NET Core skeleton code.

## 📁 Directory Structure

```
server/
├── openapi/                 # OpenAPI specifications
│   └── admin-api.yaml      # Complete API specification
├── db/                     # Database scripts
│   └── mysql/             # MySQL DDL and seed data
│       ├── 001_create_tables.sql
│       └── 002_seed_sample_data.sql
└── aspnet/                # C# ASP.NET Core skeleton
    ├── WasteNaut.Admin/   # Main project
    └── tests/            # Unit tests
```

## 🚀 Quick Start

### 1. Database Setup

1. **Install MySQL** (8.0 or later)
2. **Create database:**
   ```sql
   CREATE DATABASE wastenaut_admin CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   ```
3. **Run DDL script:**
   ```bash
   mysql -u root -p wastenaut_admin < db/mysql/001_create_tables.sql
   ```
4. **Seed sample data:**
   ```bash
   mysql -u root -p wastenaut_admin < db/mysql/002_seed_sample_data.sql
   ```

### 2. C# Backend Setup

1. **Install .NET 8 SDK**
2. **Navigate to project:**
   ```bash
   cd aspnet/WasteNaut.Admin
   ```
3. **Copy configuration:**
   ```bash
   cp appsettings.Development.json.example appsettings.Development.json
   ```
4. **Update connection string** in `appsettings.Development.json`
5. **Install dependencies:**
   ```bash
   dotnet restore
   ```
6. **Run the application:**
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:5001` with Swagger UI at the root.

## 📋 API Documentation

### OpenAPI Specification

The complete API specification is available in `openapi/admin-api.yaml`. This includes:

- **Authentication endpoints** (login, logout)
- **User management** (list, verify, suspend, impersonate)
- **Organization management** (approve, reject, set capacity)
- **Donation management** (list, update status)
- **Report management** (triage, assign, resolve)
- **Match management** (AI matches, override, generate)
- **Dashboard data** (activity, alerts)
- **Settings** (roles, notifications, system config)
- **Audit logs** (activity tracking)

### Generate C# Client

To generate a C# client from the OpenAPI spec:

```bash
# Using OpenAPI Generator
openapi-generator generate -i openapi/admin-api.yaml -g csharp -o ./generated-client

# Using NSwag
nswag openapi2csclient /input:openapi/admin-api.yaml /output:GeneratedClient.cs
```

## 🗄️ Database Schema

### Core Tables

- **`admins`** - Admin users with roles and permissions
- **`users`** - Regular users (individuals, organizations, donors)
- **`organizations`** - Organization profiles with capacity settings
- **`donations`** - Food/equipment donations with location data
- **`reports`** - User reports with evidence and notes
- **`matches`** - AI-generated matches with confidence scores
- **`audit_logs`** - System activity tracking

### Key Features

- **JSON columns** for flexible data (preferences, service areas, factors)
- **Full-text search** indexes for efficient searching
- **Foreign key constraints** for data integrity
- **Audit trail** for all admin actions

## 🔧 C# Implementation

### Project Structure

```
WasteNaut.Admin/
├── Controllers/          # API controllers
├── Models/              # Entity models
├── DTOs/                # Data transfer objects
├── Services/            # Business logic
├── Data/                # Repository pattern
└── Program.cs           # Application startup
```

### Key Components

- **Entity Framework Core** with MySQL provider
- **JWT Authentication** with Bearer tokens
- **Repository Pattern** for data access
- **Service Layer** for business logic
- **Swagger/OpenAPI** documentation
- **xUnit Tests** with in-memory database

### Configuration

Update `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=wastenaut_admin;Uid=root;Pwd=your_password;"
  },
  "Jwt": {
    "Key": "your-super-secret-jwt-key-here",
    "Issuer": "WasteNaut.Admin",
    "Audience": "WasteNaut.Admin.Users"
  }
}
```

## 🧪 Testing

### Run Unit Tests

```bash
cd aspnet/tests
dotnet test
```

### Test Coverage

The test suite includes:

- **Controller tests** with mocked services
- **Service tests** with in-memory database
- **Authentication tests** for JWT validation
- **Repository tests** for data access

## 🔄 Migration from Mock to Real Backend

### Frontend Changes

1. **Remove mock API:**
   ```javascript
   // Remove this line from admin pages
   <script src="mock-api.js"></script>
   ```

2. **Update API base URL:**
   ```javascript
   // In admin-main.js, update baseURL
   this.baseURL = 'https://localhost:5001/api/admin';
   ```

3. **Handle CORS:**
   ```javascript
   // Add CORS headers in API calls
   headers: {
     'Content-Type': 'application/json',
     'Authorization': `Bearer ${this.token}`
   }
   ```

### Backend Deployment

1. **Configure production settings**
2. **Set up SSL certificates**
3. **Configure reverse proxy (nginx/Apache)**
4. **Set up database connection pooling**
5. **Configure logging and monitoring**

## 📊 Sample Data

The seed script includes:

- **3 admin users** with different roles
- **5 regular users** with various statuses
- **3 organizations** (approved, pending, rejected)
- **3 donations** with different statuses
- **3 reports** for testing triage workflow
- **3 AI matches** with confidence scores
- **Audit logs** for activity tracking

### Demo Credentials

- **Super Admin:** admin@wastenaut.test / admin123
- **Moderator:** moderator@wastenaut.test / admin123
- **Support:** support@wastenaut.test / admin123

## 🚀 Production Deployment

### Environment Setup

1. **Database:**
   - Use MySQL 8.0+ with proper configuration
   - Set up connection pooling
   - Configure backups

2. **Application:**
   - Use production appsettings
   - Configure JWT secrets
   - Set up SSL/TLS

3. **Infrastructure:**
   - Use reverse proxy (nginx)
   - Configure load balancing
   - Set up monitoring

### Security Considerations

- **JWT secrets** must be cryptographically secure
- **Database credentials** should use least privilege
- **CORS policies** should be restrictive
- **Rate limiting** should be implemented
- **Audit logging** should be comprehensive

## 📝 Development Notes

### Adding New Endpoints

1. **Update OpenAPI spec** in `openapi/admin-api.yaml`
2. **Add controller method** in appropriate controller
3. **Implement service method** in service layer
4. **Add repository method** if needed
5. **Write unit tests** for new functionality
6. **Update frontend** to use new endpoint

### Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

### Code Generation

The OpenAPI spec can be used to generate:

- **C# client libraries**
- **TypeScript interfaces**
- **API documentation**
- **Postman collections**

## 🤝 Contributing

1. **Follow C# coding standards**
2. **Write unit tests** for new features
3. **Update OpenAPI spec** for API changes
4. **Add database migrations** for schema changes
5. **Test with sample data** before committing

## 📞 Support

For questions or issues:

1. **Check the OpenAPI spec** for endpoint documentation
2. **Review the test suite** for usage examples
3. **Examine the sample data** for expected formats
4. **Check the audit logs** for debugging information
