/**
 * WasteNaut API Service
 * Handles all API communication with the backend
 */

class ApiService {
    constructor() {
        this.baseUrl = window.location.origin + '/api';
        this.token = localStorage.getItem('authToken');
    }

    // Set authentication token
    setToken(token) {
        this.token = token;
        if (token) {
            localStorage.setItem('authToken', token);
        } else {
            localStorage.removeItem('authToken');
        }
    }

    // Get authorization headers
    getHeaders() {
        const headers = {
            'Content-Type': 'application/json',
        };
        
        if (this.token) {
            headers['Authorization'] = `Bearer ${this.token}`;
        }
        
        return headers;
    }

    // Generic API request method
    async request(endpoint, options = {}) {
        const url = `${this.baseUrl}${endpoint}`;
        const config = {
            headers: this.getHeaders(),
            ...options
        };

        try {
            const response = await fetch(url, config);
            
            if (!response.ok) {
                const errorData = await response.json().catch(() => ({ message: 'Unknown error' }));
                throw new Error(errorData.message || `HTTP ${response.status}`);
            }

            return await response.json();
        } catch (error) {
            console.error(`API Error (${endpoint}):`, error);
            throw error;
        }
    }

    // Authentication API methods
    async login(email, password) {
        return this.request('/auth/login', {
            method: 'POST',
            body: JSON.stringify({ email, password })
        });
    }

    async register(userData) {
        return this.request('/auth/register', {
            method: 'POST',
            body: JSON.stringify(userData)
        });
    }

    async verifyToken() {
        return this.request('/auth/verify-token', {
            method: 'POST'
        });
    }

    async logout() {
        const result = await this.request('/auth/logout', {
            method: 'POST'
        });
        this.setToken(null);
        return result;
    }

    // User API methods
    async getUsers() {
        return this.request('/users');
    }

    async getUser(id) {
        return this.request(`/users/${id}`);
    }

    async createUser(userData) {
        return this.request('/users', {
            method: 'POST',
            body: JSON.stringify(userData)
        });
    }

    async updateUser(id, userData) {
        return this.request(`/users/${id}`, {
            method: 'PUT',
            body: JSON.stringify(userData)
        });
    }

    async deleteUser(id) {
        return this.request(`/users/${id}`, {
            method: 'DELETE'
        });
    }

    async suspendUser(id, reason) {
        return this.request(`/users/${id}/suspend`, {
            method: 'POST',
            body: JSON.stringify({ reason })
        });
    }

    async activateUser(id) {
        return this.request(`/users/${id}/activate`, {
            method: 'POST'
        });
    }

    async getUserStats() {
        return this.request('/users/stats');
    }

    // Donation API methods
    async getDonations() {
        return this.request('/donations');
    }

    async getDonation(id) {
        return this.request(`/donations/${id}`);
    }

    async createDonation(donationData) {
        return this.request('/donations', {
            method: 'POST',
            body: JSON.stringify(donationData)
        });
    }

    async updateDonation(id, donationData) {
        return this.request(`/donations/${id}`, {
            method: 'PUT',
            body: JSON.stringify(donationData)
        });
    }

    async deleteDonation(id) {
        return this.request(`/donations/${id}`, {
            method: 'DELETE'
        });
    }

    async claimDonation(id, claimedById) {
        return this.request(`/donations/${id}/claim`, {
            method: 'POST',
            body: JSON.stringify({ claimedById })
        });
    }

    async getDonationStats() {
        return this.request('/donations/stats');
    }

    // Organization API methods
    async getOrganizations() {
        return this.request('/organizations');
    }

    async getOrganization(id) {
        return this.request(`/organizations/${id}`);
    }

    async createOrganization(orgData) {
        return this.request('/organizations', {
            method: 'POST',
            body: JSON.stringify(orgData)
        });
    }

    async updateOrganization(id, orgData) {
        return this.request(`/organizations/${id}`, {
            method: 'PUT',
            body: JSON.stringify(orgData)
        });
    }

    async deleteOrganization(id) {
        return this.request(`/organizations/${id}`, {
            method: 'DELETE'
        });
    }

    // Report API methods
    async getReports() {
        return this.request('/reports');
    }

    async getReport(id) {
        return this.request(`/reports/${id}`);
    }

    async createReport(reportData) {
        return this.request('/reports', {
            method: 'POST',
            body: JSON.stringify(reportData)
        });
    }

    async updateReport(id, reportData) {
        return this.request(`/reports/${id}`, {
            method: 'PUT',
            body: JSON.stringify(reportData)
        });
    }

    async updateReportStatus(id, status, notes) {
        return this.request(`/reports/${id}/status`, {
            method: 'PUT',
            body: JSON.stringify({ status, notes })
        });
    }

    // Match API methods
    async getMatches() {
        return this.request('/matches');
    }

    async getMatch(id) {
        return this.request(`/matches/${id}`);
    }

    async createMatch(matchData) {
        return this.request('/matches', {
            method: 'POST',
            body: JSON.stringify(matchData)
        });
    }

    async updateMatch(id, matchData) {
        return this.request(`/matches/${id}`, {
            method: 'PUT',
            body: JSON.stringify(matchData)
        });
    }

    // Smart matching AI methods
    async generateMatches(criteria) {
        return this.request('/smart-matching/generate', {
            method: 'POST',
            body: JSON.stringify(criteria)
        });
    }

    async getAISuggestions(query) {
        return this.request('/smart-matching/suggestions', {
            method: 'POST',
            body: JSON.stringify({ query })
        });
    }

    // Utility methods
    async healthCheck() {
        try {
            const response = await fetch(`${this.baseUrl}/health`);
            return response.ok;
        } catch {
            return false;
        }
    }

    // Error handling wrapper
    async handleApiCall(apiCall, errorMessage = 'An error occurred') {
        try {
            return await apiCall();
        } catch (error) {
            console.error(errorMessage, error);
            if (window.showNotification) {
                window.showNotification(`${errorMessage}: ${error.message}`, 'error');
            } else {
                alert(`${errorMessage}: ${error.message}`);
            }
            throw error;
        }
    }
}

// Create global instance
window.apiService = new ApiService();

// Export for module systems
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ApiService;
}