// Standalone client-side includes for MVP (no server required)
// This version embeds the header and footer directly instead of using fetch

class ComponentLoader {
    constructor() {
        this.basePath = window.location.pathname.includes('/html/') ? '../' : '';
    }

    loadHeader() {
        const headerHTML = `
<!-- Navigation -->
<nav class="navbar navbar-expand-lg navbar-dark bg-dark border-bottom">
  <div class="container">
    <a class="navbar-brand fw-bold d-flex align-items-center" href="index.html">
      <div class="logo-container me-2">
        <svg width="32" height="32" viewBox="0 0 100 100" class="wastenaut-logo">
          <defs>
            <linearGradient id="logoGradient" x1="0%" y1="0%" x2="100%" y2="100%">
              <stop offset="0%" style="stop-color:#00ff88;stop-opacity:1" />
              <stop offset="100%" style="stop-color:#00d4aa;stop-opacity:1" />
            </linearGradient>
          </defs>
          <!-- WasteNaut Logo -->
          <image href="../resources/images/WasteNautV2.png" x="10" y="10" width="80" height="80"/>
        </svg>
      </div>
      <span class="text-gradient">WasteNaut</span>
    </a>
    <div class="navbar-nav ms-auto d-flex flex-row">
      <a class="nav-link me-3" href="index.html">Home</a>
      <a class="nav-link me-3" href="role-selection.html">Get Started</a>
      <a class="nav-link me-3" href="dashboard.html">Dashboard</a>
      <a class="nav-link me-3" href="organization-foodbank-dashboard.html">Organization Hub</a>
      <a class="nav-link me-3" href="admin-dashboard.html">Admin</a>
      <a class="nav-link" href="#" onclick="logout()">Logout</a>
    </div>
  </div>
</nav>`;
        
        const container = document.getElementById('header-container');
        if (container) {
            container.innerHTML = headerHTML;
        }
    }

    loadFooter() {
        const footerHTML = `
<!-- Footer -->
<footer class="bg-dark border-top py-4">
  <div class="container">
    <div class="row align-items-center">
      <div class="col-md-6">
        <p class="mb-0 text-muted">&copy; 2024 WasteNaut. Navigating the future of sustainable resource management.</p>
      </div>
      <div class="col-md-6 text-md-end">
        <div class="footer-btn-container">
          <button class="footer-btn text-muted me-3" data-info="contact@wastenaut.com" title="Email us">
            <i class="bi bi-envelope"></i>
          </button>
          <button class="footer-btn text-muted me-3" data-info="+1-555-WASTE-NAUT" title="Call us">
            <i class="bi bi-telephone"></i>
          </button>
          <button class="footer-btn text-muted" data-info="WasteNaut - Fighting food waste through space-age technology" title="About us">
            <i class="bi bi-info-circle"></i>
          </button>
        </div>
      </div>
    </div>
  </div>
</footer>`;
        
        const container = document.getElementById('footer-container');
        if (container) {
            container.innerHTML = footerHTML;
        }
    }

    loadAll() {
        this.loadHeader();
        this.loadFooter();
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
            
            // Calculate position more reliably
            const viewportWidth = window.innerWidth;
            const viewportHeight = window.innerHeight;
            const popupWidth = Math.min(300, viewportWidth - 40);
            
            // Position above the button, but ensure it stays within viewport
            let topPosition = buttonRect.top - 60;
            if (topPosition < 10) {
                topPosition = buttonRect.bottom + 10;
            }
            
            // Ensure horizontal centering works on all screen sizes
            let leftPosition = buttonRect.left + (buttonRect.width / 2);
            if (leftPosition < popupWidth / 2) {
                leftPosition = popupWidth / 2;
            } else if (leftPosition > viewportWidth - popupWidth / 2) {
                leftPosition = viewportWidth - popupWidth / 2;
            }
            
            popup.style.cssText = `
                position: fixed;
                top: ${topPosition}px;
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
                width: ${popupWidth}px;
                opacity: 0;
                transition: opacity 0.3s ease;
                cursor: text;
                user-select: text;
            `;
            
            // Add arrow pointing to button
            const arrow = document.createElement('div');
            const arrowDirection = topPosition < buttonRect.top ? 'down' : 'up';
            arrow.style.cssText = `
                position: absolute;
                ${arrowDirection === 'down' ? 'top: 100%;' : 'bottom: 100%;'}
                left: 50%;
                transform: translateX(-50%);
                width: 0;
                height: 0;
                border-left: 6px solid transparent;
                border-right: 6px solid transparent;
                ${arrowDirection === 'down' ? 'border-top: 6px solid #00ff88;' : 'border-bottom: 6px solid #00ff88;'}
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
