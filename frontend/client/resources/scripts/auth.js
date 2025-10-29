/**
 * WasteNaut Authentication Manager
 * Handles user authentication and session management
 */

class AuthManager {
    constructor() {
        this.currentUser = null;
        this.isAuthenticated = false;
        this.init();
    }

    init() {
        // Check for existing token on page load
        const token = localStorage.getItem('authToken');
        if (token) {
            this.verifyToken();
        }
    }

    // Login user
    async login(email, password) {
        try {
            const response = await window.apiService.login(email, password);
            
            if (response.token) {
                window.apiService.setToken(response.token);
                this.currentUser = response.user;
                this.isAuthenticated = true;
                
                // Store user data
                localStorage.setItem('currentUser', JSON.stringify(response.user));
                
                if (window.showNotification) {
                    window.showNotification(`Welcome back, ${response.user.name}!`, 'success');
                }
                
                return response;
            } else {
                throw new Error('Invalid response from server');
            }
        } catch (error) {
            console.error('Login error:', error);
            if (window.showNotification) {
                window.showNotification(`Login failed: ${error.message}`, 'error');
            } else {
                alert(`Login failed: ${error.message}`);
            }
            throw error;
        }
    }

    // Register new user
    async register(userData) {
        try {
            const response = await window.apiService.register(userData);
            
            if (response.token) {
                window.apiService.setToken(response.token);
                this.currentUser = response.user;
                this.isAuthenticated = true;
                
                // Store user data
                localStorage.setItem('currentUser', JSON.stringify(response.user));
                
                if (window.showNotification) {
                    window.showNotification(`Welcome to WasteNaut, ${response.user.name}!`, 'success');
                }
                
                return response;
            } else {
                throw new Error('Invalid response from server');
            }
        } catch (error) {
            console.error('Registration error:', error);
            if (window.showNotification) {
                window.showNotification(`Registration failed: ${error.message}`, 'error');
            } else {
                alert(`Registration failed: ${error.message}`);
            }
            throw error;
        }
    }

    // Verify current token
    async verifyToken() {
        try {
            const response = await window.apiService.verifyToken();
            this.currentUser = response;
            this.isAuthenticated = true;
            return response;
        } catch (error) {
            console.error('Token verification failed:', error);
            this.logout();
            return null;
        }
    }

    // Logout user
    async logout() {
        try {
            if (this.isAuthenticated) {
                await window.apiService.logout();
            }
        } catch (error) {
            console.error('Logout error:', error);
        } finally {
            // Clear local data
            this.currentUser = null;
            this.isAuthenticated = false;
            window.apiService.setToken(null);
            localStorage.removeItem('currentUser');
            
            // Redirect to home page
            window.location.href = '/';
        }
    }

    // Check if user is authenticated
    isLoggedIn() {
        return this.isAuthenticated && this.currentUser !== null;
    }

    // Get current user
    getCurrentUser() {
        return this.currentUser;
    }

    // Check if user has specific role
    hasRole(role) {
        return this.isAuthenticated && this.currentUser && this.currentUser.role === role;
    }

    // Check if user has any of the specified roles
    hasAnyRole(roles) {
        return this.isAuthenticated && this.currentUser && roles.includes(this.currentUser.role);
    }

    // Check if user is admin
    isAdmin() {
        return this.hasRole('admin');
    }

    // Check if user is organization
    isOrganization() {
        return this.hasRole('organization');
    }

    // Check if user is individual
    isIndividual() {
        return this.hasRole('individual');
    }

    // Protect page - redirect if not authenticated or wrong role
    protectPage(requiredRole = null) {
        if (!this.isLoggedIn()) {
            window.location.href = '/login.html';
            return false;
        }

        if (requiredRole && !this.hasRole(requiredRole)) {
            if (window.showNotification) {
                window.showNotification('You do not have permission to access this page', 'error');
            } else {
                alert('You do not have permission to access this page');
            }
            window.location.href = '/';
            return false;
        }

        return true;
    }

    // Get user dashboard URL based on role
    getDashboardUrl() {
        if (!this.isLoggedIn()) {
            return '/';
        }

        switch (this.currentUser.role) {
            case 'admin':
                return '/admin-dashboard.html';
            case 'organization':
                return '/organization-foodbank-dashboard.html';
            case 'individual':
                return '/individual-dashboard.html';
            default:
                return '/';
        }
    }

    // Update user profile
    async updateProfile(profileData) {
        try {
            if (!this.isAuthenticated) {
                throw new Error('User not authenticated');
            }

            const response = await window.apiService.updateUser(this.currentUser.id, profileData);
            
            // Update local user data
            this.currentUser = { ...this.currentUser, ...profileData };
            localStorage.setItem('currentUser', JSON.stringify(this.currentUser));
            
            if (window.showNotification) {
                window.showNotification('Profile updated successfully', 'success');
            }
            
            return response;
        } catch (error) {
            console.error('Profile update error:', error);
            if (window.showNotification) {
                window.showNotification(`Profile update failed: ${error.message}`, 'error');
            } else {
                alert(`Profile update failed: ${error.message}`);
            }
            throw error;
        }
    }

    // Check if user account is active
    isAccountActive() {
        return this.isAuthenticated && this.currentUser && this.currentUser.status === 'active';
    }

    // Get account status message
    getAccountStatusMessage() {
        if (!this.isAuthenticated) {
            return 'Not logged in';
        }

        switch (this.currentUser.status) {
            case 'active':
                return 'Account is active';
            case 'pending':
                return 'Account is pending approval';
            case 'suspended':
                return 'Account is suspended';
            default:
                return 'Unknown account status';
        }
    }
}

// Create global instance
window.authManager = new AuthManager();

// Export for module systems
if (typeof module !== 'undefined' && module.exports) {
    module.exports = AuthManager;
}
