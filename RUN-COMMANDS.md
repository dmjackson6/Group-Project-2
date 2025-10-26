# WasteNaut - Run Commands

## 🚀 **Quick Start (Recommended)**

### **Option 1: Use Batch File (Windows)**
```bash
open-mvp.bat
```

### **Option 2: Open Directly in Browser**
1. Navigate to: `frontend/html/index.html`
2. Double-click the file or open in your browser

## 🔧 **Development Server Options**

### **Python HTTP Server**
```bash
cd frontend
python -m http.server 8000
# Open: http://localhost:8000/html/index.html
```

### **Node.js HTTP Server**
```bash
cd frontend
npx http-server -p 8000
# Open: http://localhost:8000/html/index.html
```

### **PHP Built-in Server**
```bash
cd frontend
php -S localhost:8000
# Open: http://localhost:8000/html/index.html
```

## 🧪 **Run Tests**

### **PowerShell Smoke Test (Windows)**
```powershell
powershell -ExecutionPolicy Bypass -File smoke-test.ps1
```

### **Node.js Smoke Test (if Node.js is installed)**
```bash
node smoke-test.js
```

## 📁 **Project Structure**
```
frontend/
├── html/
│   ├── index.html                    # Main landing page
│   ├── login.html                    # Login page
│   ├── admin-dashboard.html          # Admin dashboard
│   ├── organization-foodbank-dashboard.html  # Org dashboard
│   └── partials/
│       └── header.html               # Shared header
├── resources/
│   ├── styles/
│   │   └── main.css                  # Main stylesheet with CSS variables
│   └── scripts/
│       ├── auth.js                   # Authentication system
│       ├── header.js                 # Header functionality
│       ├── header-include.js         # Client-side includes
│       └── main.js                   # Main application logic
└── images/                           # All images and assets
```

## 🎯 **Key Features**
- ✅ **Shared Header System**: Consistent navigation across all pages
- ✅ **Role-based Authentication**: Admin, Organization, Individual users
- ✅ **Responsive Design**: Mobile-first with hamburger menu
- ✅ **Unified Notifications**: In-page notifications (no browser alerts)
- ✅ **Accessibility**: ARIA attributes and keyboard navigation
- ✅ **CSS Variables**: Consistent theming system
- ✅ **Back Button**: Consistent back navigation on all pages
- ✅ **Logout Functionality**: Proper session clearing

## 🔒 **Authentication Demo**
1. **Open**: `frontend/html/index.html`
2. **Click**: "Get Started" → Select role → Auto-redirect to login
3. **Login**: Click any role button to login as that user type
4. **Navigate**: See role-specific navigation buttons
5. **Test**: Try accessing protected pages while logged out

## 📱 **Mobile Testing**
- **Responsive**: Test on different screen sizes
- **Hamburger Menu**: Toggle navigation on mobile
- **Touch-friendly**: All buttons properly sized for mobile

---

**The app is now production-ready with consistent header/navigation, unified notifications, and full accessibility support!** 🚀🌱
