// Mock API for WasteNaut Admin
// This file provides browser-side mocking for all admin API endpoints
// To replace with real API, simply remove this file and update admin-main.js

class MockAPI {
    constructor() {
        this.baseURL = '/api/admin';
        this.delay = 200; // Simulate network delay
        this.mockData = this.loadMockData();
    }

    // Load mock data from JSON fixtures
    loadMockData() {
        return {
            users: this.getMockUsers(),
            organizations: this.getMockOrganizations(),
            donations: this.getMockDonations(),
            reports: this.getMockReports(),
            matches: this.getMockMatches(),
            auditLogs: this.getMockAuditLogs(),
            roles: this.getMockRoles(),
            templates: this.getMockTemplates(),
            settings: this.getMockSettings()
        };
    }

    // Simulate network delay
    async delayResponse() {
        return new Promise(resolve => setTimeout(resolve, this.delay + Math.random() * 400));
    }

    // Mock authentication
    async login(credentials) {
        await this.delayResponse();
        
        if (credentials.email === 'admin@wastenaut.test' && credentials.password === 'admin123') {
            const token = this.generateMockToken();
            const user = {
                id: 1,
                name: 'Admin User',
                email: 'admin@wastenaut.test',
                role: 'super_admin',
                permissions: ['all']
            };
            
            return {
                success: true,
                token,
                user
            };
        } else {
            throw new Error('Invalid credentials');
        }
    }

    // Generate mock JWT token
    generateMockToken() {
        const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
        const payload = btoa(JSON.stringify({
            sub: '1',
            email: 'admin@wastenaut.test',
            role: 'super_admin',
            exp: Math.floor(Date.now() / 1000) + (24 * 60 * 60) // 24 hours
        }));
        const signature = btoa('mock-signature');
        return `${header}.${payload}.${signature}`;
    }

    // Mock data generators
    getMockUsers() {
        return [
            {
                id: 1,
                name: 'John Doe',
                email: 'john@example.com',
                role: 'individual',
                status: 'verified',
                createdAt: '2024-01-15T10:30:00Z',
                lastActive: '2024-01-20T14:22:00Z',
                organization: null
            },
            {
                id: 2,
                name: 'Jane Smith',
                email: 'jane@example.com',
                role: 'organization',
                status: 'pending',
                createdAt: '2024-01-18T09:15:00Z',
                lastActive: '2024-01-19T16:45:00Z',
                organization: {
                    name: 'Green Earth Foundation',
                    type: 'charity'
                }
            },
            {
                id: 3,
                name: 'Mike Johnson',
                email: 'mike@example.com',
                role: 'donor',
                status: 'verified',
                createdAt: '2024-01-10T08:20:00Z',
                lastActive: '2024-01-20T11:30:00Z',
                organization: null
            },
            {
                id: 4,
                name: 'Sarah Wilson',
                email: 'sarah@example.com',
                role: 'individual',
                status: 'suspended',
                createdAt: '2024-01-12T13:45:00Z',
                lastActive: '2024-01-18T09:20:00Z',
                organization: null
            }
        ];
    }

    getMockOrganizations() {
        return [
            {
                id: 1,
                name: 'Green Earth Foundation',
                type: 'charity',
                status: 'approved',
                registrationNumber: 'REG-2024-001',
                address: '123 Green St, Eco City, EC 12345',
                contact: {
                    name: 'Jane Smith',
                    email: 'jane@greenearth.org',
                    phone: '+1-555-0123'
                },
                capacity: {
                    max: 100,
                    used: 75,
                    notes: 'Operating at 75% capacity'
                },
                serviceAreas: ['Downtown', 'Westside', 'North District'],
                createdAt: '2024-01-15T10:30:00Z'
            },
            {
                id: 2,
                name: 'Community Food Bank',
                type: 'foodbank',
                status: 'pending',
                registrationNumber: 'REG-2024-002',
                address: '456 Food Ave, Hunger City, HC 67890',
                contact: {
                    name: 'Bob Brown',
                    email: 'bob@foodbank.org',
                    phone: '+1-555-0456'
                },
                capacity: {
                    max: 200,
                    used: 120,
                    notes: 'Expanding operations'
                },
                serviceAreas: ['Eastside', 'South District'],
                createdAt: '2024-01-18T14:20:00Z'
            },
            {
                id: 3,
                name: 'Local Restaurant Chain',
                type: 'restaurant',
                status: 'approved',
                registrationNumber: 'REG-2024-003',
                address: '789 Main St, Food City, FC 54321',
                contact: {
                    name: 'Alice Green',
                    email: 'alice@restaurant.com',
                    phone: '+1-555-0789'
                },
                capacity: {
                    max: 50,
                    used: 30,
                    notes: 'Regular food donations'
                },
                serviceAreas: ['Central', 'Downtown'],
                createdAt: '2024-01-12T11:15:00Z'
            }
        ];
    }

    getMockDonations() {
        return [
            {
                id: 1,
                title: 'Fresh Vegetables',
                description: 'Assorted fresh vegetables from local farm',
                type: 'food',
                status: 'active',
                quantity: 50,
                unit: 'lbs',
                donor: {
                    name: 'Mike Johnson',
                    email: 'mike@example.com'
                },
                recipient: {
                    name: 'Green Earth Foundation',
                    email: 'jane@greenearth.org'
                },
                createdAt: '2024-01-20T09:30:00Z',
                expiresAt: '2024-01-25T18:00:00Z'
            },
            {
                id: 2,
                title: 'Bread and Pastries',
                description: 'Day-old bread and pastries from bakery',
                type: 'food',
                status: 'completed',
                quantity: 25,
                unit: 'items',
                donor: {
                    name: 'Local Bakery',
                    email: 'bakery@local.com'
                },
                recipient: {
                    name: 'Community Food Bank',
                    email: 'bob@foodbank.org'
                },
                createdAt: '2024-01-19T14:20:00Z',
                completedAt: '2024-01-20T16:45:00Z'
            }
        ];
    }

    getMockReports() {
        return [
            {
                id: 1,
                title: 'Suspicious Activity Report',
                description: 'User reported suspicious behavior from another user during donation pickup',
                type: 'safety',
                priority: 'high',
                status: 'pending',
                reporter: {
                    name: 'John Doe',
                    email: 'john@example.com',
                    phone: '+1-555-0101'
                },
                assignedTo: null,
                createdAt: '2024-01-20T10:15:00Z',
                notes: []
            },
            {
                id: 2,
                title: 'Fraudulent Organization',
                description: 'Report of organization providing false information',
                type: 'fraud',
                priority: 'critical',
                status: 'investigating',
                reporter: {
                    name: 'Sarah Wilson',
                    email: 'sarah@example.com',
                    phone: '+1-555-0202'
                },
                assignedTo: {
                    name: 'Admin User',
                    email: 'admin@wastenaut.test'
                },
                createdAt: '2024-01-19T16:30:00Z',
                notes: [
                    {
                        author: 'Admin User',
                        text: 'Initial investigation started',
                        createdAt: '2024-01-19T17:00:00Z'
                    }
                ]
            }
        ];
    }

    getMockMatches() {
        return [
            {
                id: 1,
                title: 'Food Donation Match',
                type: 'donation',
                confidence: 92,
                status: 'pending',
                from: {
                    name: 'Mike Johnson',
                    type: 'Individual',
                    location: 'Downtown',
                    contact: 'mike@example.com'
                },
                to: {
                    name: 'Green Earth Foundation',
                    type: 'Organization',
                    location: 'Westside',
                    contact: 'jane@greenearth.org'
                },
                factors: [
                    { weight: 40, description: 'Geographic proximity' },
                    { weight: 30, description: 'Food type compatibility' },
                    { weight: 22, description: 'Timing alignment' }
                ],
                aiNotes: 'High confidence match based on location and food preferences',
                createdAt: '2024-01-20T11:00:00Z'
            },
            {
                id: 2,
                title: 'Volunteer Request Match',
                type: 'volunteer',
                confidence: 78,
                status: 'accepted',
                from: {
                    name: 'John Doe',
                    type: 'Individual',
                    location: 'Central',
                    contact: 'john@example.com'
                },
                to: {
                    name: 'Community Food Bank',
                    type: 'Organization',
                    location: 'Eastside',
                    contact: 'bob@foodbank.org'
                },
                factors: [
                    { weight: 35, description: 'Skill match' },
                    { weight: 25, description: 'Availability' },
                    { weight: 18, description: 'Location preference' }
                ],
                aiNotes: 'Good match for volunteer opportunity',
                createdAt: '2024-01-19T14:20:00Z'
            }
        ];
    }

    getMockAuditLogs() {
        return [
            {
                id: 1,
                action: 'user_verified',
                user: 'Admin User',
                target: 'John Doe',
                timestamp: '2024-01-20T10:30:00Z',
                details: 'User account verified successfully'
            },
            {
                id: 2,
                action: 'organization_approved',
                user: 'Admin User',
                target: 'Green Earth Foundation',
                timestamp: '2024-01-19T15:45:00Z',
                details: 'Organization approval completed'
            }
        ];
    }

    getMockRoles() {
        return [
            {
                id: 1,
                name: 'Super Admin',
                description: 'Full system access',
                userCount: 1,
                permissions: ['all']
            },
            {
                id: 2,
                name: 'Moderator',
                description: 'User and content moderation',
                userCount: 3,
                permissions: ['users', 'reports', 'content']
            },
            {
                id: 3,
                name: 'Support',
                description: 'Customer support access',
                userCount: 5,
                permissions: ['users', 'tickets']
            }
        ];
    }

    getMockTemplates() {
        return [
            {
                id: 1,
                name: 'Welcome Email',
                description: 'Welcome message for new users',
                type: 'email',
                status: 'active',
                subject: 'Welcome to WasteNaut!',
                content: 'Welcome to our platform...'
            },
            {
                id: 2,
                name: 'Verification SMS',
                description: 'SMS for account verification',
                type: 'sms',
                status: 'active',
                content: 'Your verification code is: {code}'
            },
            {
                id: 3,
                name: 'Match Notification',
                description: 'Notification for new matches',
                type: 'push',
                status: 'inactive',
                content: 'You have a new match!'
            }
        ];
    }

    getMockSettings() {
        return {
            siteName: 'WasteNaut',
            maintenanceMode: false,
            maxUsers: 10000,
            sessionTimeout: 30,
            aiEnabled: true,
            confidenceThreshold: 70,
            maxMatches: 5,
            aiModel: 'gpt-3.5-turbo',
            emailEnabled: true,
            smsEnabled: false,
            pushEnabled: true
        };
    }

    // API endpoint handlers
    async handleRequest(url, options = {}) {
        await this.delayResponse();
        
        const path = url.replace(this.baseURL, '');
        
        // Authentication endpoints
        if (path === '/login' && options.method === 'POST') {
            const body = JSON.parse(options.body);
            return this.login(body);
        }
        
        // User endpoints
        if (path === '/users' && options.method === 'GET') {
            return this.mockData.users;
        }
        
        if (path.startsWith('/users/') && path.endsWith('/verify') && options.method === 'POST') {
            const userId = parseInt(path.split('/')[2]);
            const user = this.mockData.users.find(u => u.id === userId);
            if (user) {
                user.status = 'verified';
                return { success: true };
            }
            throw new Error('User not found');
        }
        
        if (path.startsWith('/users/') && path.endsWith('/suspend') && options.method === 'POST') {
            const userId = parseInt(path.split('/')[2]);
            const user = this.mockData.users.find(u => u.id === userId);
            if (user) {
                user.status = 'suspended';
                return { success: true };
            }
            throw new Error('User not found');
        }
        
        if (path.startsWith('/users/') && path.endsWith('/unsuspend') && options.method === 'POST') {
            const userId = parseInt(path.split('/')[2]);
            const user = this.mockData.users.find(u => u.id === userId);
            if (user) {
                user.status = 'verified';
                return { success: true };
            }
            throw new Error('User not found');
        }
        
        // Organization endpoints
        if (path === '/organizations' && options.method === 'GET') {
            return this.mockData.organizations;
        }
        
        if (path.startsWith('/organizations/') && path.endsWith('/approve') && options.method === 'POST') {
            const orgId = parseInt(path.split('/')[2]);
            const org = this.mockData.organizations.find(o => o.id === orgId);
            if (org) {
                org.status = 'approved';
                return { success: true };
            }
            throw new Error('Organization not found');
        }
        
        if (path.startsWith('/organizations/') && path.endsWith('/reject') && options.method === 'POST') {
            const orgId = parseInt(path.split('/')[2]);
            const org = this.mockData.organizations.find(o => o.id === orgId);
            if (org) {
                org.status = 'rejected';
                return { success: true };
            }
            throw new Error('Organization not found');
        }
        
        // Report endpoints
        if (path === '/reports' && options.method === 'GET') {
            return this.mockData.reports;
        }
        
        if (path.startsWith('/reports/') && path.endsWith('/status') && options.method === 'PUT') {
            const reportId = parseInt(path.split('/')[2]);
            const report = this.mockData.reports.find(r => r.id === reportId);
            if (report) {
                const body = JSON.parse(options.body);
                report.status = body.status;
                return { success: true };
            }
            throw new Error('Report not found');
        }
        
        // Match endpoints
        if (path === '/matches' && options.method === 'GET') {
            return this.mockData.matches;
        }
        
        if (path.startsWith('/matches/') && path.endsWith('/accept') && options.method === 'POST') {
            const matchId = parseInt(path.split('/')[2]);
            const match = this.mockData.matches.find(m => m.id === matchId);
            if (match) {
                match.status = 'accepted';
                return { success: true };
            }
            throw new Error('Match not found');
        }
        
        if (path.startsWith('/matches/') && path.endsWith('/reject') && options.method === 'POST') {
            const matchId = parseInt(path.split('/')[2]);
            const match = this.mockData.matches.find(m => m.id === matchId);
            if (match) {
                match.status = 'rejected';
                return { success: true };
            }
            throw new Error('Match not found');
        }
        
        // Dashboard endpoints
        if (path === '/dashboard/activity' && options.method === 'GET') {
            return this.mockData.auditLogs.map(log => ({
                timestamp: log.timestamp,
                userName: log.user,
                action: log.action,
                status: 'success'
            }));
        }
        
        if (path === '/dashboard/alerts' && options.method === 'GET') {
            return [
                {
                    severity: 'warning',
                    icon: 'exclamation-triangle',
                    message: 'High number of pending reports'
                },
                {
                    severity: 'info',
                    icon: 'info-circle',
                    message: 'System maintenance scheduled for tonight'
                }
            ];
        }
        
        // Settings endpoints
        if (path === '/settings/roles' && options.method === 'GET') {
            return this.mockData.roles;
        }
        
        if (path === '/settings/notifications/templates' && options.method === 'GET') {
            return this.mockData.templates;
        }
        
        if (path === '/settings/system' && options.method === 'GET') {
            return this.mockData.settings;
        }
        
        if (path === '/settings/system' && options.method === 'PUT') {
            const body = JSON.parse(options.body);
            Object.assign(this.mockData.settings, body);
            return { success: true };
        }
        
        if (path === '/settings/ai' && options.method === 'PUT') {
            const body = JSON.parse(options.body);
            Object.assign(this.mockData.settings, body);
            return { success: true };
        }
        
        if (path === '/settings/notifications' && options.method === 'PUT') {
            const body = JSON.parse(options.body);
            Object.assign(this.mockData.settings, body);
            return { success: true };
        }
        
        // Default response for unmatched endpoints
        return { success: true, message: 'Mock response' };
    }
}

// Override fetch to use mock API
const originalFetch = window.fetch;
const mockAPI = new MockAPI();

window.fetch = async function(url, options = {}) {
    // Only mock admin API calls
    if (url.includes('/api/admin')) {
        try {
            const response = await mockAPI.handleRequest(url, options);
            return {
                ok: true,
                status: 200,
                json: async () => response
            };
        } catch (error) {
            return {
                ok: false,
                status: 400,
                json: async () => ({ message: error.message })
            };
        }
    }
    
    // Use original fetch for other requests
    return originalFetch(url, options);
};
