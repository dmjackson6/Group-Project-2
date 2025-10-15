# WasteNaut - Space-Age Resource Management Platform

A modernized, eco-friendly resource sharing platform with a clean frontend/backend architecture built with HTML, CSS, JavaScript, Bootstrap, and ASP.NET Core.

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- MySQL 8.0+ (optional for full functionality)
- Modern web browser

### Installation & Setup

1. **Clone and navigate to the project:**
   ```bash
   cd Group-Project-2
   ```

2. **Start the backend server:**
   ```bash
   cd backend/server/aspnet/WasteNaut.Admin
   dotnet restore
   dotnet run
   ```

3. **Open your browser:**
   ```
   http://localhost:3000
   ```

The application will automatically serve the frontend and provide mock API endpoints for development.

## 📁 New Project Structure

```
WasteNaut/
├── frontend/                    # Frontend assets
│   ├── html/                   # All HTML pages
│   │   ├── index.html          # Main landing page
│   │   ├── role-selection.html # User role selection
│   │   ├── dashboard.html      # User dashboard
│   │   ├── organization-foodbank-dashboard.html # Organization hub
│   │   ├── admin-dashboard.html # Admin panel
│   │   └── ...                 # Other pages
│   └── resources/              # Static assets
│       ├── images/             # All images
│       ├── scripts/            # JavaScript files
│       │   ├── main.js         # Main application logic
│       │   ├── includes.js     # Client-side includes
│       │   ├── header.html     # Shared header component
│       │   └── footer.html     # Shared footer component
│       └── styles/             # CSS files
│           ├── main.css        # Main styles
│           └── admin.css       # Admin-specific styles
├── backend/                    # Backend services
│   ├── server/                 # ASP.NET Core server
│   │   └── aspnet/            # C# backend implementation
│   └── mocks/                 # Mock data for development
└── README.md                  # This file
```

## 🎨 Features

### Frontend Features
- **Responsive Design**: Mobile-first approach with Bootstrap 5.3.3
- **Shared Components**: Reusable header and footer with client-side includes
- **Unified Navigation**: Consistent navigation across all pages with hamburger menu
- **Space-age Theme**: Electric green (#00ff88) with dark mode throughout
- **Organization Dashboard**: Dedicated food bank management interface

### Backend Features
- **ASP.NET Core API**: RESTful endpoints with JWT authentication
- **Static File Serving**: Automatic frontend delivery
- **Mock API Endpoints**: Development-ready mock data
- **CORS Enabled**: Cross-origin requests supported
- **Swagger Documentation**: Available at `/swagger`

## 🔧 Development

### Available Endpoints

- **Frontend**: `http://localhost:3000` (serves HTML pages)
- **API**: `http://localhost:3000/api/*` (REST endpoints)
- **Swagger**: `http://localhost:3000/swagger` (API documentation)

### Mock API Endpoints

- `GET /api/inventory` - Returns mock donation/inventory data
- `GET /api/users` - Returns mock user data
- `GET /api/requests` - Returns mock request/match data

### Key Pages

| Page | Purpose | Path |
|------|---------|------|
| **Landing Page** | Main entry point | `/html/index.html` |
| **Role Selection** | Choose user type | `/html/role-selection.html` |
| **User Dashboard** | User control panel | `/html/dashboard.html` |
| **Organization Hub** | Food bank management | `/html/organization-foodbank-dashboard.html` |
| **Admin Dashboard** | System management | `/html/admin-dashboard.html` |

## 🧪 Testing the Application

### Manual Smoke Tests

1. **Navigation Test:**
   - Open `http://localhost:3000`
   - Click hamburger menu on mobile/tablet
   - Verify all navigation links work
   - Test responsive behavior

2. **Page Loading Test:**
   - Navigate to each major page
   - Verify shared header/footer load correctly
   - Check that all images and styles load

3. **Organization Dashboard Test:**
   - Navigate to Organization Hub
   - Verify inventory summary displays
   - Test quick action buttons
   - Check KPI cards

4. **API Test:**
   - Visit `http://localhost:3000/swagger`
   - Test mock API endpoints
   - Verify JSON responses

## 📝 Major Changes Made

### 1. **Project Reorganization**
- Created clear `frontend/` and `backend/` structure
- Moved all HTML files to `frontend/html/`
- Consolidated duplicate `index.html` files
- Organized resources in `frontend/resources/`

### 2. **Shared Components**
- Implemented client-side includes for header/footer
- Created reusable navigation with hamburger menu
- Unified styling and branding across all pages

### 3. **New Organization Dashboard**
- Built comprehensive food bank management interface
- Added inventory summary with low-stock alerts
- Included volunteer/contacts management
- Created pending requests queue
- Added KPI tracking and quick actions

### 4. **Backend Modernization**
- Updated ASP.NET Core to serve static files
- Added mock API endpoints for development
- Configured CORS for cross-origin requests
- Set up proper routing and port configuration

### 5. **Development Experience**
- Single command to start the application
- Automatic frontend serving
- Mock data integration
- Swagger API documentation

## 🔄 Migration Notes

- **No destructive changes**: All original files preserved in new structure
- **Backward compatibility**: Existing functionality maintained
- **Mock data preserved**: All mock data moved to `backend/mocks/`
- **Path updates**: All asset paths updated to new structure

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
2. **Review the mock data** in `backend/mocks/` for expected formats
3. **Test with the provided smoke tests** above
4. **Check browser console** for any JavaScript errors

---

**Ready to launch into the future of sustainable resource management!** 🚀🌱