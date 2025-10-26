// Client-side header include system
// This script loads the shared header into pages that include it

class HeaderInclude {
    constructor() {
        this.headerLoaded = false;
        this.init();
    }

    async init() {
        await this.loadHeader();
        this.headerLoaded = true;
    }

    async loadHeader() {
        try {
            const response = await fetch('../html/partials/header.html');
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const headerHTML = await response.text();
            const headerContainer = document.getElementById('header-container');
            
            if (headerContainer) {
                headerContainer.innerHTML = headerHTML;
                console.log('Header loaded successfully');
                
                // Initialize header manager after header is loaded
                setTimeout(() => {
                    if (window.headerManager) {
                        window.headerManager.initializeNavigation();
                        window.headerManager.updateLogoLink();
                    }
                }, 100);
            } else {
                console.warn('Header container not found');
            }
        } catch (error) {
            console.error('Error loading header:', error);
            console.log('Loading fallback header...');
            this.loadFallbackHeader();
        }
    }

    loadFallbackHeader() {
        const headerContainer = document.getElementById('header-container');
        if (!headerContainer) return;

        const fallbackHTML = `
            <nav class="navbar navbar-expand-lg navbar-dark app-header" role="navigation" aria-label="Main navigation">
                <div class="container">
                    <a class="navbar-brand app-logo" href="index.html" aria-label="WasteNaut Home" id="wastenaut-logo">
                        <button 
                            class="btn app-back-btn me-2" 
                            onclick="goBack()" 
                            aria-label="Go back to previous page"
                            title="Go back"
                            id="back-button"
                            style="display: none;"
                        >
                            <i class="bi bi-arrow-left" aria-hidden="true"></i>
                        </button>
                        <div class="logo-container me-2">
                            <svg width="32" height="32" viewBox="0 0 100 100" class="wastenaut-logo" aria-hidden="true">
                                <defs>
                                    <linearGradient id="logoGradient" x1="0%" y1="0%" x2="100%" y2="100%">
                                        <stop offset="0%" style="stop-color:#00ff88;stop-opacity:1" />
                                        <stop offset="100%" style="stop-color:#00d4aa;stop-opacity:1" />
                                    </linearGradient>
                                </defs>
                                <image href="../resources/images/WasteNautV2.png" x="10" y="10" width="80" height="80"/>
                            </svg>
                        </div>
                        <span class="text-gradient">WasteNaut</span>
                    </a>

                    <button
                        class="navbar-toggler app-nav-toggle"
                        type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#appNavbar"
                        aria-controls="appNavbar"
                        aria-expanded="false"
                        aria-label="Toggle navigation menu"
                    >
                        <span class="navbar-toggler-icon"></span>
                    </button>

                    <div class="collapse navbar-collapse" id="appNavbar">
                        <ul class="navbar-nav ms-auto app-nav-menu" id="navigation-menu" role="menubar">
                        </ul>
                        
                        <div class="app-page-nav" id="page-navigation" style="display: none;">
                        </div>
                        
                        <div class="app-nav-actions">
                            <button 
                                class="btn app-logout-btn" 
                                onclick="confirmLogout()" 
                                aria-label="Logout from WasteNaut"
                                title="Logout"
                                id="logout-button"
                                style="display: none;"
                            >
                                <i class="bi bi-box-arrow-right" aria-hidden="true"></i>
                            </button>
                        </div>
                    </div>
                </div>
            </nav>
            <div id="notification-area" class="app-notification-area" aria-live="polite" aria-atomic="true"></div>
        `;
        
        headerContainer.innerHTML = fallbackHTML;
        console.log('Fallback header loaded');
        console.log('Header container content:', headerContainer.innerHTML.substring(0, 200) + '...');
        
        // Initialize header manager after fallback is loaded
        setTimeout(() => {
            if (window.headerManager) {
                window.headerManager.initializeNavigation();
                window.headerManager.updateLogoLink();
            }
        }, 100);
    }
}

// Initialize header include when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    new HeaderInclude();
});
