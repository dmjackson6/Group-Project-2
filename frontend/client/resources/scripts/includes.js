// Client-side includes for shared components
// This script loads header and footer HTML fragments into designated containers

class ComponentLoader {
    constructor() {
        this.basePath = window.location.pathname.includes('/html/') ? '../' : '';
    }

    async loadComponent(containerId, componentPath) {
        try {
            const response = await fetch(`${this.basePath}resources/scripts/${componentPath}`);
            if (!response.ok) {
                throw new Error(`Failed to load ${componentPath}: ${response.status}`);
            }
            const html = await response.text();
            const container = document.getElementById(containerId);
            if (container) {
                container.innerHTML = html;
            } else {
                console.warn(`Container with id '${containerId}' not found`);
            }
        } catch (error) {
            console.error(`Error loading component ${componentPath}:`, error);
        }
    }

    async loadHeader() {
        await this.loadComponent('header-container', 'header.html');
    }

    async loadFooter() {
        await this.loadComponent('footer-container', 'footer.html');
    }

    async loadAll() {
        await Promise.all([
            this.loadHeader(),
            this.loadFooter()
        ]);
        
        // Initialize footer button functionality after loading
        this.initializeFooterButtons();
    }

    initializeFooterButtons() {
        // Footer button popup functionality
        const emailBtn = document.querySelector('button[data-info="contact@wastenaut.com"]');
        const phoneBtn = document.querySelector('button[data-info="+1-555-WASTE-NAUT"]');
        const infoBtn = document.querySelector('button[data-info="WasteNaut - Fighting food waste through space-age technology"]');
        
        function createPopup(text, button) {
            // Remove any existing popups
            const existingPopups = document.querySelectorAll('.wastenaut-popup');
            existingPopups.forEach(popup => popup.remove());
            
            // Create popup element
            const popup = document.createElement('div');
            popup.className = 'wastenaut-popup';
            popup.textContent = text;
            
            // Position popup above the button
            const buttonRect = button.getBoundingClientRect();
            let leftPosition = buttonRect.left + (buttonRect.width / 2);
            let maxWidth = 'auto';
            let whiteSpace = 'nowrap';
            
            // For long text (info button), allow wrapping but keep centered
            if (text.length > 30) {
                maxWidth = '300px';
                whiteSpace = 'normal';
            }
            
            popup.style.cssText = `
                position: fixed;
                bottom: ${window.innerHeight - buttonRect.top + 10}px;
                left: ${leftPosition}px;
                transform: translateX(-50%);
                background: rgba(10, 10, 10, 0.95);
                border: 1px solid #00ff88;
                border-radius: 8px;
                padding: 12px 16px;
                color: #00ff88;
                font-size: 14px;
                font-family: 'Exo 2', sans-serif;
                z-index: 9999;
                box-shadow: 0 4px 12px rgba(0, 255, 136, 0.3);
                backdrop-filter: blur(10px);
                white-space: ${whiteSpace};
                max-width: ${maxWidth};
                opacity: 0;
                transition: opacity 0.3s ease;
                cursor: text;
                user-select: text;
            `;
            
            // Add arrow pointing down
            const arrow = document.createElement('div');
            arrow.style.cssText = `
                position: absolute;
                top: 100%;
                left: 50%;
                transform: translateX(-50%);
                width: 0;
                height: 0;
                border-left: 6px solid transparent;
                border-right: 6px solid transparent;
                border-top: 6px solid #00ff88;
            `;
            popup.appendChild(arrow);
            
            // Add to body
            document.body.appendChild(popup);
            
            // Fade in
            setTimeout(() => {
                popup.style.opacity = '1';
            }, 10);
            
            // Add hover functionality to popup
            popup.onmouseenter = function() {
                popup.style.opacity = '1';
                if (popup.hideTimeout) {
                    clearTimeout(popup.hideTimeout);
                    popup.hideTimeout = null;
                }
            };
            
            popup.onmouseleave = function() {
                popup.style.opacity = '0';
                popup.hideTimeout = setTimeout(() => {
                    if (popup.parentNode) {
                        popup.remove();
                    }
                }, 300);
            };
        }
        
        function removePopup() {
            const existingPopups = document.querySelectorAll('.wastenaut-popup');
            existingPopups.forEach(popup => {
                popup.style.opacity = '0';
                setTimeout(() => {
                    if (popup.parentNode) {
                        popup.remove();
                    }
                }, 300);
            });
        }
        
        if (emailBtn) {
            emailBtn.onmouseenter = function() {
                createPopup('contact@wastenaut.com', this);
            };
            emailBtn.onmouseleave = function() {
                setTimeout(() => {
                    const popup = document.querySelector('.wastenaut-popup');
                    if (popup && !popup.matches(':hover')) {
                        removePopup();
                    }
                }, 100);
            };
        }
        
        if (phoneBtn) {
            phoneBtn.onmouseenter = function() {
                createPopup('+1-555-WASTE-NAUT', this);
            };
            phoneBtn.onmouseleave = function() {
                setTimeout(() => {
                    const popup = document.querySelector('.wastenaut-popup');
                    if (popup && !popup.matches(':hover')) {
                        removePopup();
                    }
                }, 100);
            };
        }
        
        if (infoBtn) {
            infoBtn.onmouseenter = function() {
                createPopup('WasteNaut - Fighting food waste through space-age technology', this);
            };
            infoBtn.onmouseleave = function() {
                setTimeout(() => {
                    const popup = document.querySelector('.wastenaut-popup');
                    if (popup && !popup.matches(':hover')) {
                        removePopup();
                    }
                }, 100);
            };
        }
    }
}

// Initialize component loader when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    const loader = new ComponentLoader();
    loader.loadAll();
});

// Global logout function
function logout() {
    if (confirm('Are you sure you want to logout?')) {
        // Clear any stored authentication data
        localStorage.removeItem('wastenaut_token');
        localStorage.removeItem('wastenaut_user');
        
        // Redirect to home page
        window.location.href = 'index.html';
    }
}
