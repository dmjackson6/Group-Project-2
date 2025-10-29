// API Service for backend integration
class ApiService {
    constructor() {
        this.baseUrl = '/api';
    }

    // Generic API call method
    async apiCall(endpoint, options = {}) {
        const url = `${this.baseUrl}${endpoint}`;
        const defaultOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
        };

        const config = { ...defaultOptions, ...options };

        try {
            const response = await fetch(url, config);
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            // Handle empty responses (like DELETE)
            const text = await response.text();
            return text ? JSON.parse(text) : null;
        } catch (error) {
            console.error('API call failed:', error);
            throw error;
        }
    }

    // Organization Profile API
    async getOrganizationProfile(organizationId) {
        return await this.apiCall(`/OrganizationProfile/${organizationId}`);
    }

    async updateOrganizationProfile(organizationId, data) {
        return await this.apiCall(`/OrganizationProfile/${organizationId}`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    // Volunteers API
    async getVolunteers(organizationId = null) {
        const query = organizationId ? `?organizationId=${organizationId}` : '';
        return await this.apiCall(`/Volunteers${query}`);
    }

    async getVolunteer(volunteerId) {
        return await this.apiCall(`/Volunteers/${volunteerId}`);
    }

    async createVolunteer(data) {
        return await this.apiCall('/Volunteers', {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    async updateVolunteer(volunteerId, data) {
        return await this.apiCall(`/Volunteers/${volunteerId}`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    async deleteVolunteer(volunteerId) {
        return await this.apiCall(`/Volunteers/${volunteerId}`, {
            method: 'DELETE'
        });
    }

    // Requests API
    async getRequests(organizationId = null) {
        const query = organizationId ? `?organizationId=${organizationId}` : '';
        return await this.apiCall(`/Requests${query}`);
    }

    async getRequest(requestId) {
        return await this.apiCall(`/Requests/${requestId}`);
    }

    async createRequest(data) {
        return await this.apiCall('/Requests', {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    async updateRequest(requestId, data) {
        return await this.apiCall(`/Requests/${requestId}`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    async deleteRequest(requestId) {
        return await this.apiCall(`/Requests/${requestId}`, {
            method: 'DELETE'
        });
    }
}

// Create global API service instance
window.apiService = new ApiService();
