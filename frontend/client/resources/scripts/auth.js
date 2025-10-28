// WasteNaut Authentication System (Frontend-Only Demo)
// This is a demo system for presentation purposes

class AuthManager {
    constructor() {
        this.currentUser = null;
        this.init();
    }

    init() {
        // Load user from localStorage on initialization
        this.loadUser();
    }

    // Load user data from localStorage
    loadUser() {
        try {
            const userData = localStorage.getItem('wastenaut_user');
            if (userData) {
                this.currentUser = JSON.parse(userData);
            }
        } catch (error) {
            console.error('Error loading user data:', error);
            this.currentUser = null;
        }
    }

    // Get current user info
    getCurrentUser() {
        return this.currentUser;
    }

    // Check if user is authenticated
    isAuthenticated() {
        return this.currentUser && this.currentUser.authenticated === true;
    }

    // Check if user has specific role
    hasRole(role) {
        return this.isAuthenticated() && this.currentUser.role === role;
    }

    // Check if user has any of the specified roles
    hasAnyRole(roles) {
        return this.isAuthenticated() && roles.includes(this.currentUser.role);
    }

    // Get user's role
    getUserRole() {
        return this.isAuthenticated() ? this.currentUser.role : 'guest';
    }

    // Get user's display name
    getUserDisplayName() {
        return this.isAuthenticated() ? this.currentUser.name : 'Guest';
    }

    // Login user with specific role (demo function)
    login(role, customName = null) {
        const userData = {
            authenticated: true,
            role: role,
            userId: `${role}_${Date.now()}`,
            name: customName || this.getRoleDisplayName(role),
            loginTime: new Date().toISOString()
        };

        this.currentUser = userData;
        localStorage.setItem('wastenaut_user', JSON.stringify(userData));
        localStorage.setItem('wastenaut_token', `demo_token_${role}_${Date.now()}`);
        
        return userData;
    }

    // Logout user
    logout() {
        this.currentUser = null;
        localStorage.removeItem('wastenaut_user');
        localStorage.removeItem('wastenaut_token');
    }

    // Get display name for role
    getRoleDisplayName(role) {
        switch(role) {
            case 'admin': return 'System Administrator';
            case 'org': return 'Organization Manager';
            case 'user': return 'Individual User';
            default: return 'User';
        }
    }

    // Redirect to appropriate dashboard based on role
    redirectToDashboard() {
        if (!this.isAuthenticated()) {
            window.location.href = 'login.html';
            return;
        }

        switch(this.currentUser.role) {
            case 'admin':
                window.location.href = 'admin-dashboard.html';
                break;
            case 'org':
                window.location.href = 'organization-foodbank-dashboard.html';
                break;
            case 'individual':
                window.location.href = 'individual-dashboard.html';
                break;
            case 'user':
                window.location.href = 'dashboard.html';
                break;
            default:
                window.location.href = 'index.html';
        }
    }

    // Check if user can access a specific page
    canAccessPage(pageRole) {
        if (!this.isAuthenticated()) {
            return pageRole === 'public';
        }

        // Admin can access everything
        if (this.currentUser.role === 'admin') {
            return true;
        }

        // Check specific role access
        return this.currentUser.role === pageRole;
    }

    // Protect a page - redirect if unauthorized
    protectPage(requiredRole) {
        if (!this.canAccessPage(requiredRole)) {
            if (!this.isAuthenticated()) {
                // Not logged in - redirect to login
                window.location.href = 'login.html';
            } else {
                // Wrong role - redirect to appropriate dashboard
                this.redirectToDashboard();
            }
            return false;
        }
        return true;
    }

    // Show access denied message
    showAccessDenied() {
        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-danger alert-dismissible fade show position-fixed';
        alertDiv.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        alertDiv.innerHTML = `
          <div class="d-flex align-items-center">
            <i class="bi bi-exclamation-triangle me-2"></i>
            <span>Access denied. You don't have permission to view this page.</span>
            <button type="button" class="btn-close ms-auto" data-bs-dismiss="alert"></button>
          </div>
        `;
        document.body.appendChild(alertDiv);

        setTimeout(() => {
          if (alertDiv.parentNode) {
            alertDiv.remove();
          }
        }, 5000);
    }
}

// Create global auth manager instance
window.authManager = new AuthManager();

// Utility functions for easy access
window.isAuthenticated = () => window.authManager.isAuthenticated();
window.hasRole = (role) => window.authManager.hasRole(role);
window.hasAnyRole = (roles) => window.authManager.hasAnyRole(roles);
window.getUserRole = () => window.authManager.getUserRole();
window.getUserDisplayName = () => window.authManager.getUserDisplayName();
window.logout = () => {
    window.authManager.logout();
    window.location.href = 'index.html';
};

// Navigation helper functions
window.getNavigationForRole = (role, currentPage = '') => {
    const baseNav = [];
    
    // Auto-detect current page if not provided
    if (!currentPage) {
        currentPage = window.location.pathname.split('/').pop() || 'index.html';
    }
    
    // Only add Home button if not on the home page
    if (currentPage !== 'index.html' && currentPage !== '') {
        baseNav.push({ text: 'Home', href: 'index.html', icon: 'bi-house' });
    }

    switch(role) {
        case 'admin':
            const nav = [...baseNav];
            // Only show Admin Hub button if not already on admin dashboard page
            if (currentPage !== 'admin-dashboard.html') {
                nav.push({ text: 'Admin Hub', href: 'admin-dashboard.html', icon: 'bi-shield-check' });
            }
            // Only show Reports button if not already on reports page
            if (currentPage !== 'reports.html') {
                nav.push({ text: 'Reports', href: 'reports.html', icon: 'bi-graph-up' });
            }
            // Add Logout button as the last item
            if (currentPage !== 'login.html') {
                nav.push({ text: 'Log Out', href: '#', icon: 'bi-box-arrow-right', onclick: 'confirmLogout()' });
            }
            return nav;
        case 'org':
            const orgNav = [...baseNav];
            // Only show Organization Hub button if not already on organization hub page
            if (currentPage !== 'organization-foodbank-dashboard.html') {
                orgNav.push({ text: 'Organization Hub', href: 'organization-foodbank-dashboard.html', icon: 'bi-building' });
            }
            // Only show Inventory button if not already on inventory page
            if (currentPage !== 'inventory-management.html') {
                orgNav.push({ text: 'Inventory', href: 'inventory-management.html', icon: 'bi-box-seam' });
            }
            // Only show Smart Matching button if not already on smart matching page
            if (currentPage !== 'smart-matching.html') {
                orgNav.push({ text: 'Smart Matching', href: 'smart-matching.html', icon: 'bi-diagram-3' });
            }
            // Only show Communication button if not already on communication page
            if (currentPage !== 'communication-hub.html') {
                orgNav.push({ text: 'Communication', href: 'communication-hub.html', icon: 'bi-chat-dots' });
            }
            // Add Logout button as the last item
            if (currentPage !== 'login.html') {
                orgNav.push({ text: 'Log Out', href: '#', icon: 'bi-box-arrow-right', onclick: 'confirmLogout()' });
            }
            return orgNav;
        case 'individual':
            const individualNav = [...baseNav];
            // Only show Inventory button if not already on inventory page
            if (currentPage !== 'inventory-management.html') {
                individualNav.push({ text: 'Inventory', href: 'inventory-management.html', icon: 'bi-box-seam' });
            }
            // Only show Smart Matching button if not already on smart matching page
            if (currentPage !== 'smart-matching.html') {
                individualNav.push({ text: 'Smart Matching', href: 'smart-matching.html', icon: 'bi-search' });
            }
            // Only show Communication button if not already on communication page
            if (currentPage !== 'communication-hub.html') {
                individualNav.push({ text: 'Communication', href: 'communication-hub.html', icon: 'bi-chat-dots' });
            }
            // Add Logout button as the last item
            if (currentPage !== 'login.html') {
                individualNav.push({ text: 'Log Out', href: '#', icon: 'bi-box-arrow-right', onclick: 'confirmLogout()' });
            }
            return individualNav;
        case 'user':
            const userNav = [...baseNav];
            // Add Logout button as the last item
            if (currentPage !== 'login.html') {
                userNav.push({ text: 'Log Out', href: '#', icon: 'bi-box-arrow-right', onclick: 'confirmLogout()' });
            }
            return userNav;
        default:
            const defaultNav = [...baseNav];
            // Add Logout button as the last item for authenticated users
            if (role && role !== 'guest' && currentPage !== 'login.html') {
                defaultNav.push({ text: 'Log Out', href: '#', icon: 'bi-box-arrow-right', onclick: 'confirmLogout()' });
            }
            return defaultNav;
    }
};
