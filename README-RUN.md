# WasteNaut - How to Run and Operate

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

The application will automatically serve the frontend and connect to the MySQL database for persistent data storage.

## 🔧 Development Server Options

### Option 1: Use Batch File (Windows)
```bash
open-mvp.bat
```

### Option 2: Open Directly in Browser
1. Navigate to: `frontend/client/index.html`
2. Double-click the file or open in your browser

### Option 3: Simple HTTP Server (Optional)
If you want to avoid CORS issues with some browsers:

1. **Using Python (if installed):**
   ```bash
   cd frontend
   python -m http.server 8000
   ```
   Then open: `http://localhost:8000/client/index.html`

2. **Using Node.js (if installed):**
   ```bash
   cd frontend
   npx http-server -p 8000
   ```
   Then open: `http://localhost:8000/client/index.html`

3. **Using VS Code Live Server extension:**
   - Install "Live Server" extension
   - Right-click on `index.html` → "Open with Live Server"

## 🧪 Run Tests

### PowerShell Smoke Test (Windows)
```powershell
powershell -ExecutionPolicy Bypass -File smoke-test.ps1
```

### Node.js Smoke Test (if Node.js is installed)
```bash
node smoke-test.js
```

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
│       │   └── header.html      # Shared header
│       └── resources/           # Static assets
│           ├── images/         # All images
│           ├── scripts/        # JavaScript files
│           │   ├── main.js     # Main application logic
│           │   ├── auth.js     # Authentication system
│           │   ├── header.js   # Header functionality
│           │   └── header-include.js # Client-side includes
│           └── styles/         # CSS files
│               ├── main.css    # Main styles
│               └── admin.css   # Admin-specific styles
├── backend/                     # Backend services
│   ├── api/                    # API implementation
│   │   ├── aspnet/            # C# backend implementation
│   │   └── db/                # Database scripts
│   │       └── mysql/         # MySQL DDL and seed data
└── README.md                  # This file
```

## 🎯 Available Endpoints

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
   - Test database-powered API endpoints
   - Verify JSON responses from MySQL database

## 🔒 Authentication & Test Credentials

### Test Admin Accounts
The database includes pre-configured admin accounts for testing:

| Role | Email | Password | Access Level |
|------|-------|----------|--------------|
| **Super Admin** | `admin@wastenaut.test` | `admin123` | Full system access |
| **Moderator** | `moderator@wastenaut.test` | `admin123` | User & content moderation |
| **Support** | `support@wastenaut.test` | `admin123` | Customer support access |

### Authentication Demo
1. **Open**: `http://localhost:3000`
2. **Click**: "Get Started" → Select role → Auto-redirect to login
3. **Login**: Use the test credentials above or click role buttons for demo login
4. **Navigate**: See role-specific navigation buttons
5. **Test**: Try accessing protected pages while logged out

## 🗄️ Database Information

### Database Connection
- **Type**: MySQL 8.0+ on AWS RDS
- **Host**: Automatically configured in `appsettings.Development.json`
- **Database**: `p7on9pts3ap3rhin`
- **Tables**: 12 tables with full relationships and sample data

### Sample Data Included
- **Users**: 5 test users (individual, organization, donor roles)
- **Organizations**: 3 organizations (charity, foodbank, restaurant)
- **Donations**: 3 donation records with different statuses
- **Reports**: 3 sample reports with evidence and notes
- **Matches**: 3 AI-generated matches with confidence scores
- **Audit Logs**: Complete audit trail of system activities

## 📱 Mobile Testing

- **Responsive**: Test on different screen sizes
- **Hamburger Menu**: Toggle navigation on mobile
- **Touch-friendly**: All buttons properly sized for mobile

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
