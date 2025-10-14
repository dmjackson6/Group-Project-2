// Admin API Client for WasteNaut
// This file provides the API client functions for admin operations

class AdminAPI {
    constructor() {
        this.baseURL = '/api/admin';
        this.token = localStorage.getItem('adminToken');
    }

    // Helper method to make API calls
    async makeRequest(endpoint, options = {}) {
        const url = `${this.baseURL}${endpoint}`;
        const config = {
            headers: {
                'Content-Type': 'application/json',
                ...(this.token && { 'Authorization': `Bearer ${this.token}` })
            },
            ...options
        };

        try {
            const response = await fetch(url, config);
            const data = await response.json();
            
            if (!response.ok) {
                throw new Error(data.message || 'API request failed');
            }
            
            return data;
        } catch (error) {
            console.error('API request failed:', error);
            throw error;
        }
    }

    // Authentication
    async login(credentials) {
        return this.makeRequest('/login', {
            method: 'POST',
            body: JSON.stringify(credentials)
        });
    }

    async logout() {
        localStorage.removeItem('adminToken');
        localStorage.removeItem('adminUser');
        window.location.href = 'login.html';
    }

    // User Management
    async getUsers() {
        return this.makeRequest('/users');
    }

    async getUser(userId) {
        return this.makeRequest(`/users/${userId}`);
    }

    async verifyUser(userId) {
        return this.makeRequest(`/users/${userId}/verify`, {
            method: 'POST'
        });
    }

    async suspendUser(userId) {
        return this.makeRequest(`/users/${userId}/suspend`, {
            method: 'POST'
        });
    }

    async unsuspendUser(userId) {
        return this.makeRequest(`/users/${userId}/unsuspend`, {
            method: 'POST'
        });
    }

    async impersonateUser(userId) {
        return this.makeRequest(`/users/${userId}/impersonate`, {
            method: 'POST'
        });
    }

    // Organization Management
    async getOrganizations() {
        return this.makeRequest('/organizations');
    }

    async getOrganization(orgId) {
        return this.makeRequest(`/organizations/${orgId}`);
    }

    async approveOrganization(orgId) {
        return this.makeRequest(`/organizations/${orgId}/approve`, {
            method: 'POST'
        });
    }

    async rejectOrganization(orgId, reason) {
        return this.makeRequest(`/organizations/${orgId}/reject`, {
            method: 'POST',
            body: JSON.stringify({ reason })
        });
    }

    async setOrganizationCapacity(orgId, capacity) {
        return this.makeRequest(`/organizations/${orgId}/capacity`, {
            method: 'PUT',
            body: JSON.stringify(capacity)
        });
    }

    // Donation Management
    async getDonations() {
        return this.makeRequest('/donations');
    }

    async getDonation(donationId) {
        return this.makeRequest(`/donations/${donationId}`);
    }

    async updateDonationStatus(donationId, status) {
        return this.makeRequest(`/donations/${donationId}/status`, {
            method: 'PUT',
            body: JSON.stringify({ status })
        });
    }

    // Report Management
    async getReports() {
        return this.makeRequest('/reports');
    }

    async getReport(reportId) {
        return this.makeRequest(`/reports/${reportId}`);
    }

    async updateReportStatus(reportId, statusData) {
        return this.makeRequest(`/reports/${reportId}/status`, {
            method: 'PUT',
            body: JSON.stringify(statusData)
        });
    }

    async addReportNote(reportId, noteData) {
        return this.makeRequest(`/reports/${reportId}/notes`, {
            method: 'POST',
            body: JSON.stringify(noteData)
        });
    }

    async resolveReport(reportId, resolution) {
        return this.makeRequest(`/reports/${reportId}/resolve`, {
            method: 'POST',
            body: JSON.stringify({ resolution })
        });
    }

    async assignReportsToMe(reportIds) {
        return this.makeRequest('/reports/assign', {
            method: 'POST',
            body: JSON.stringify({ reportIds })
        });
    }

    // Match Management
    async getMatches() {
        return this.makeRequest('/matches');
    }

    async getMatch(matchId) {
        return this.makeRequest(`/matches/${matchId}`);
    }

    async acceptMatch(matchId) {
        return this.makeRequest(`/matches/${matchId}/accept`, {
            method: 'POST'
        });
    }

    async rejectMatch(matchId, reason) {
        return this.makeRequest(`/matches/${matchId}/reject`, {
            method: 'POST',
            body: JSON.stringify({ reason })
        });
    }

    async overrideMatch(matchId, overrideData) {
        return this.makeRequest(`/matches/${matchId}/override`, {
            method: 'POST',
            body: JSON.stringify(overrideData)
        });
    }

    async generateMatches() {
        return this.makeRequest('/matches/generate', {
            method: 'POST'
        });
    }

    // Dashboard Data
    async getRecentActivity() {
        return this.makeRequest('/dashboard/activity');
    }

    async getSystemAlerts() {
        return this.makeRequest('/dashboard/alerts');
    }

    // Settings Management
    async getRoles() {
        return this.makeRequest('/settings/roles');
    }

    async getNotificationTemplates() {
        return this.makeRequest('/settings/notifications/templates');
    }

    async getSystemSettings() {
        return this.makeRequest('/settings/system');
    }

    async updateSystemSettings(settings) {
        return this.makeRequest('/settings/system', {
            method: 'PUT',
            body: JSON.stringify(settings)
        });
    }

    async updateAISettings(settings) {
        return this.makeRequest('/settings/ai', {
            method: 'PUT',
            body: JSON.stringify(settings)
        });
    }

    async updateNotificationSettings(settings) {
        return this.makeRequest('/settings/notifications', {
            method: 'PUT',
            body: JSON.stringify(settings)
        });
    }

    // Audit Logs
    async getAuditLogs() {
        return this.makeRequest('/audit');
    }

    async getAuditLog(logId) {
        return this.makeRequest(`/audit/${logId}`);
    }
}

// Global admin API instance
const adminAPI = new AdminAPI();

// Utility functions
function showAlert(message, type = 'info') {
    // Create alert element
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    // Insert at top of page
    const container = document.querySelector('.container-fluid') || document.body;
    container.insertBefore(alertDiv, container.firstChild);
    
    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        if (alertDiv.parentNode) {
            alertDiv.remove();
        }
    }, 5000);
}

// Format date for display
function formatDate(dateString) {
    return new Date(dateString).toLocaleString();
}

// Format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(amount);
}

// Format percentage
function formatPercentage(value) {
    return `${Math.round(value)}%`;
}

// Debounce function for search
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Check if user is authenticated
function checkAuth() {
    if (!localStorage.getItem('adminToken')) {
        window.location.href = 'login.html';
        return false;
    }
    return true;
}

// Initialize admin pages
document.addEventListener('DOMContentLoaded', function() {
    // Check authentication on admin pages (except login)
    if (!window.location.pathname.includes('login.html')) {
        checkAuth();
    }
    
    // Add loading states to buttons
    const buttons = document.querySelectorAll('button[type="submit"], .btn-primary, .btn-success, .btn-danger');
    buttons.forEach(button => {
        button.addEventListener('click', function() {
            if (this.type === 'submit' || this.classList.contains('btn-primary') || 
                this.classList.contains('btn-success') || this.classList.contains('btn-danger')) {
                this.classList.add('loading');
                this.disabled = true;
                
                // Re-enable after 2 seconds (in case of errors)
                setTimeout(() => {
                    this.classList.remove('loading');
                    this.disabled = false;
                }, 2000);
            }
        });
    });
});
