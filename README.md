# WasteNaut - Space-Age Resource Management Platform

A modernized, eco-friendly resource sharing platform with a clean frontend/backend architecture built with HTML, CSS, JavaScript, Bootstrap, and ASP.NET Core.

## 📁 Project Structure

```
WasteNaut/
├── frontend/                    # Frontend assets
│   └── client/                  # Client-side files
│       ├── index.html          # Main landing page
│       ├── role-selection.html # User role selection
│       ├── dashboard.html      # User dashboard
│       ├── organization-foodbank-dashboard.html # Organization hub
│       ├── admin-dashboard.html # Admin panel
│       ├── partials/           # Shared components
│       │   └── header.html     # Shared header
│       └── resources/          # Static assets
│           ├── images/         # All images
│           ├── scripts/        # JavaScript files
│           └── styles/         # CSS files
├── backend/                     # Backend services
│   ├── api/                    # API implementation
│   │   ├── aspnet/            # C# backend implementation
│   │   └── db/                # Database scripts
│   │       └── mysql/         # MySQL DDL and seed data
├── README-RUN.md              # How to run and operate
├── README-FEATURES.md         # Features and capabilities
└── README.md                 # This file
```

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Modern web browser
- **MySQL Database**: Connected to AWS RDS instance (configured automatically)

### Installation & Setup

1. **Clone and navigate to the project:**
   ```bash
   cd Group-Project-2
   ```

2. **Start the backend server:**
   ```bash
   cd backend/api/aspnet/WasteNaut.Admin
   dotnet restore
   dotnet run
   ```

3. **Open your browser:**
   ```
   http://localhost:3000
   ```

## 📚 Documentation

- **[README-RUN.md](README-RUN.md)** - Complete guide on how to run and operate the project
- **[README-FEATURES.md](README-FEATURES.md)** - Detailed features and capabilities

## 🎯 Key Features

### Frontend Features
- **Responsive Design**: Mobile-first approach with Bootstrap 5.3.3
- **Shared Components**: Reusable header and footer with client-side includes
- **Unified Navigation**: Consistent navigation across all pages with hamburger menu
- **Space-age Theme**: Electric green (#00ff88) with dark mode throughout
- **Organization Dashboard**: Dedicated food bank management interface

### Backend Features
- **ASP.NET Core API**: RESTful endpoints with JWT authentication
- **MySQL Database**: Persistent data storage with AWS RDS
- **Entity Framework**: ORM for database operations
- **Static File Serving**: Automatic frontend delivery
- **CORS Enabled**: Cross-origin requests supported
- **Swagger Documentation**: Available at `/swagger`

## 🔧 Development

### Available Endpoints
- **Frontend**: `http://localhost:3000` (serves HTML pages)
- **API**: `http://localhost:3000/api/*` (REST endpoints)
- **Swagger**: `http://localhost:3000/swagger` (API documentation)

### Database-Powered API Endpoints
- `GET /api/users` - Returns user data from MySQL database
- `GET /api/organizations` - Returns organization data
- `GET /api/donations` - Returns donation/inventory data
- `GET /api/reports` - Returns report data
- `GET /api/matches` - Returns match data
- **Authentication endpoints** - Login/logout with JWT tokens

### Test Credentials
| Role | Email | Password |
|------|-------|----------|
| **Super Admin** | `admin@wastenaut.test` | `admin123` |
| **Moderator** | `moderator@wastenaut.test` | `admin123` |
| **Support** | `support@wastenaut.test` | `admin123` |

## 🧪 Testing

### Manual Smoke Tests
1. **Navigation Test:** Open `http://localhost:3000` and test navigation
2. **Page Loading Test:** Navigate to each major page and verify components
3. **Organization Dashboard Test:** Test the organization hub functionality
4. **API Test:** Visit `http://localhost:3000/swagger` and test endpoints

### Automated Testing
```bash
# PowerShell Smoke Test (Windows)
powershell -ExecutionPolicy Bypass -File smoke-test.ps1

# Node.js Smoke Test (if Node.js is installed)
node smoke-test.js
```

## 🚀 Production Deployment

For production deployment:
1. **Configure database connection** in `appsettings.json`
2. **Set up SSL certificates** for HTTPS
3. **Configure reverse proxy** (nginx/Apache)
4. **Set up monitoring and logging**
5. **Update CORS policies** for production domains

## 📞 Support

For questions or issues:
1. **Check the Swagger docs** at `/swagger` for API details
2. **Review the database schema** in `backend/api/db/mysql/` for data structure
3. **Test with the provided smoke tests** above
4. **Check browser console** for any JavaScript errors
5. **Verify database connection** in application logs

---

**Ready to launch into the future of sustainable resource management!** 🚀🌱