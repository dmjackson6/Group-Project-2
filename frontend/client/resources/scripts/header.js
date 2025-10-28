// WasteNaut Shared Header Functionality
// This file provides consistent header behavior across all pages

class HeaderManager {
    constructor() {
        this.notificationArea = null;
        this.init();
    }

    init() {
        // Initialize notification area
        this.notificationArea = document.getElementById('notification-area');
        if (!this.notificationArea) {
            console.warn('Notification area not found. Creating fallback.');
            this.createNotificationArea();
        }

        // Initialize navigation
        this.initializeNavigation();
        
        // Show/hide logout button based on authentication
        this.updateLogoutButton();
        
        // Show/hide back button based on current page
        this.updateBackButton();
        
        // Handle logo link behavior
        this.updateLogoLink();
    }

    createNotificationArea() {
        console.log('Creating fallback notification area...');
        const area = document.createElement('div');
        area.id = 'notification-area';
        area.className = 'app-notification-area';
        area.setAttribute('aria-live', 'polite');
        area.setAttribute('aria-atomic', 'true');
        
        // Ensure proper positioning styles are applied
        area.style.cssText = `
            position: fixed !important;
            top: 20px !important;
            right: 20px !important;
            z-index: 9999 !important;
            max-width: 400px !important;
            pointer-events: none;
        `;
        
        document.body.appendChild(area);
        this.notificationArea = area;
        console.log('Fallback notification area created:', area);
        console.log('Area computed styles:', window.getComputedStyle(area));
    }

    initializeNavigation() {
        const navMenu = document.getElementById('navigation-menu');
        if (!navMenu) return;

        const userRole = window.getUserRole ? window.getUserRole() : 'guest';
        const navigationItems = window.getNavigationForRole ? window.getNavigationForRole(userRole) : [];

        // Clear existing navigation
        navMenu.innerHTML = '';

        // Add navigation items
        navigationItems.forEach(item => {
            const navItem = document.createElement('li');
            navItem.className = 'nav-item';
            navItem.setAttribute('role', 'none');

            const navLink = document.createElement('a');
            navLink.className = 'nav-link';
            navLink.href = item.href;
            navLink.innerHTML = `<i class="${item.icon} me-1" aria-hidden="true"></i>${item.text}`;
            navLink.setAttribute('role', 'menuitem');
            navLink.setAttribute('aria-label', item.text);
            
            // Handle onclick for logout button
            if (item.onclick) {
                navLink.addEventListener('click', function(e) {
                    e.preventDefault();
                    eval(item.onclick);
                });
            }

            navItem.appendChild(navLink);
            navMenu.appendChild(navItem);
        });
        
        // Initialize scrollable navigation
        this.initializeScrollableNavigation();
    }
    
    initializeScrollableNavigation() {
        const navContainer = document.querySelector('.nav-scroll-container');
        const navMenu = document.getElementById('navigation-menu');
        const leftBtn = document.getElementById('nav-scroll-left');
        const rightBtn = document.getElementById('nav-scroll-right');
        
        if (!navContainer || !navMenu || !leftBtn || !rightBtn) return;
        
        let scrollPosition = 0;
        const scrollAmount = 200;
        
        const updateScrollButtons = () => {
            const maxScroll = navMenu.scrollWidth - navContainer.clientWidth;
            
            leftBtn.style.display = scrollPosition > 0 ? 'flex' : 'none';
            rightBtn.style.display = scrollPosition < maxScroll ? 'flex' : 'none';
            
            leftBtn.disabled = scrollPosition <= 0;
            rightBtn.disabled = scrollPosition >= maxScroll;
        };
        
        const scrollLeft = () => {
            scrollPosition = Math.max(0, scrollPosition - scrollAmount);
            navMenu.style.transform = `translateX(-${scrollPosition}px)`;
            updateScrollButtons();
        };
        
        const scrollRight = () => {
            const maxScroll = navMenu.scrollWidth - navContainer.clientWidth;
            scrollPosition = Math.min(maxScroll, scrollPosition + scrollAmount);
            navMenu.style.transform = `translateX(-${scrollPosition}px)`;
            updateScrollButtons();
        };
        
        leftBtn.addEventListener('click', scrollLeft);
        rightBtn.addEventListener('click', scrollRight);
        
        // Add smooth transition
        navMenu.style.transition = 'transform 0.3s ease';
        
        // Initial check
        setTimeout(updateScrollButtons, 100);
        
        // Check on window resize
        window.addEventListener('resize', updateScrollButtons);
    }

    updateLogoutButton() {
        const logoutBtn = document.getElementById('logout-button');
        if (!logoutBtn) return;

        const isAuthenticated = window.isAuthenticated ? window.isAuthenticated() : false;
        logoutBtn.style.display = isAuthenticated ? 'inline-block' : 'none';
    }

    updateBackButton() {
        const backBtn = document.getElementById('back-button');
        if (!backBtn) return;

        const currentPage = this.getCurrentPageName();
        // Hide back button on home page
        if (currentPage === 'index') {
            backBtn.style.display = 'none';
        } else {
            backBtn.style.display = 'inline-block';
        }
    }

    updateLogoLink() {
        const logoLink = document.getElementById('wastenaut-logo') || document.querySelector('.navbar-brand.app-logo');
        if (!logoLink) return;

        const currentPage = this.getCurrentPageName();
        
        // If on home page, remove the link functionality
        if (currentPage === 'index') {
            // Remove href and add click prevention
            logoLink.removeAttribute('href');
            logoLink.style.cursor = 'default';
            logoLink.style.pointerEvents = 'none';
            logoLink.addEventListener('click', function(e) {
                e.preventDefault();
                e.stopPropagation();
                return false;
            });
        } else {
            // Ensure it has the proper href for other pages
            logoLink.href = 'index.html';
            logoLink.style.cursor = 'pointer';
            logoLink.style.pointerEvents = 'auto';
        }
    }

    getCurrentPageName() {
        // Handle both server and file:// protocol
        let filename = 'index.html';
        
        if (window.location.pathname) {
            filename = window.location.pathname.split('/').pop() || 'index.html';
        } else if (window.location.href) {
            // For file:// protocol, extract from href
            const urlParts = window.location.href.split('/');
            filename = urlParts[urlParts.length - 1] || 'index.html';
        }
        
        // Remove .html extension and handle index case
        const pageName = filename.replace('.html', '');
        return pageName === '' ? 'index' : pageName;
    }

    // Notification system
    showNotification(message, type = 'info', duration = 5000) {
        console.log('showNotification called:', message, type);
        console.log('Notification area:', this.notificationArea);
        
        if (!this.notificationArea) {
            console.error('Notification area not available');
            return;
        }

        const notification = document.createElement('div');
        notification.className = `app-notification ${type}`;
        notification.setAttribute('role', 'alert');
        notification.setAttribute('aria-live', 'assertive');
        
        // Ensure notifications can be interacted with
        notification.style.pointerEvents = 'auto';

        const icon = this.getNotificationIcon(type);
        notification.innerHTML = `
            <div class="d-flex align-items-center">
                <i class="${icon} me-2" aria-hidden="true"></i>
                <span>${message}</span>
                <button type="button" class="btn-close ms-auto" onclick="this.parentElement.parentElement.remove()" aria-label="Close notification"></button>
            </div>
        `;

        this.notificationArea.appendChild(notification);
        console.log('Notification added to area:', notification);
        console.log('Notification computed styles:', window.getComputedStyle(notification));
        console.log('Notification area computed styles:', window.getComputedStyle(this.notificationArea));

        // Trigger animation
        setTimeout(() => {
            notification.classList.add('show');
        }, 10);

        // Auto-remove
        if (duration > 0) {
            setTimeout(() => {
                this.removeNotification(notification);
            }, duration);
        }

        return notification;
    }

    removeNotification(notification) {
        if (notification && notification.parentNode) {
            notification.classList.remove('show');
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.remove();
                }
            }, 300);
        }
    }

    getNotificationIcon(type) {
        switch(type) {
            case 'success': return 'bi bi-check-circle';
            case 'error': return 'bi bi-exclamation-triangle';
            case 'warning': return 'bi bi-exclamation-circle';
            case 'info': return 'bi bi-info-circle';
            default: return 'bi bi-info-circle';
        }
    }

    // Back button functionality
    goBack() {
        if (window.history.length > 1) {
            window.history.back();
        } else {
            // Fallback to home page if no history
            window.location.href = 'index.html';
        }
    }

    // Logout functionality
    confirmLogout() {
        const isAuthenticated = window.isAuthenticated ? window.isAuthenticated() : false;
        if (!isAuthenticated) {
            this.showNotification('You are not logged in', 'warning');
            return;
        }

        // Logout immediately without confirmation
        this.performLogout();
    }

    async performLogout() {
        try {
            // Show loading notification
            const loadingNotification = this.showNotification('Logging out...', 'info', 0);

            // Call logout API (if available)
            if (window.authManager && window.authManager.logout) {
                window.authManager.logout();
            } else {
                // Fallback: clear localStorage
                localStorage.removeItem('wastenaut_user');
                localStorage.removeItem('wastenaut_token');
            }

            // Remove loading notification
            this.removeNotification(loadingNotification);

            // Show success and redirect
            this.showNotification('Logged out successfully', 'success', 2000);
            
            setTimeout(() => {
                window.location.href = 'index.html';
            }, 1000);

        } catch (error) {
            console.error('Logout error:', error);
            this.showNotification('Logout failed. Please try again.', 'error');
        }
    }
}

// Global functions for easy access
window.goBack = function() {
    if (window.headerManager) {
        window.headerManager.goBack();
    }
};

window.confirmLogout = function() {
    if (window.headerManager) {
        window.headerManager.confirmLogout();
    }
};

window.showNotification = function(message, type, duration) {
    if (window.headerManager) {
        return window.headerManager.showNotification(message, type, duration);
    }
};

// Initialize header manager when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('Creating HeaderManager...');
    window.headerManager = new HeaderManager();
    console.log('HeaderManager created:', window.headerManager);
});

// Re-initialize navigation when auth state changes
window.addEventListener('storage', function(e) {
    if (e.key === 'wastenaut_user' && window.headerManager) {
        window.headerManager.initializeNavigation();
        window.headerManager.updateLogoutButton();
    }
});
