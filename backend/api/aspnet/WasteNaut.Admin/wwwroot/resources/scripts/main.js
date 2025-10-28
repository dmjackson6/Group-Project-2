// Supply Connect - Main JavaScript File
// Shared functionality for the Supply Connect platform

// Utility functions
const Utils = {
  // Show alert messages
  showAlert: function(message, type = 'info', duration = 5000) {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
    alertDiv.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px; max-width: 400px;';
    alertDiv.innerHTML = `
      <div class="d-flex align-items-center">
        <i class="bi bi-${this.getAlertIcon(type)} me-2"></i>
        <span>${message}</span>
        <button type="button" class="btn-close ms-auto" data-bs-dismiss="alert"></button>
      </div>
    `;
    document.body.appendChild(alertDiv);

    // Auto-remove after specified duration
    setTimeout(() => {
      if (alertDiv.parentNode) {
        alertDiv.remove();
      }
    }, duration);
  },

  // Get appropriate icon for alert type
  getAlertIcon: function(type) {
    const icons = {
      'success': 'check-circle',
      'danger': 'exclamation-triangle',
      'warning': 'exclamation-circle',
      'info': 'info-circle'
    };
    return icons[type] || 'info-circle';
  },

  // Format phone number
  formatPhoneNumber: function(phoneNumber) {
    const cleaned = phoneNumber.replace(/\D/g, '');
    const match = cleaned.match(/^(\d{3})(\d{3})(\d{4})$/);
    if (match) {
      return `(${match[1]}) ${match[2]}-${match[3]}`;
    }
    return phoneNumber;
  },

  // Validate email
  validateEmail: function(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
  },

  // Validate phone number
  validatePhone: function(phone) {
    const cleaned = phone.replace(/\D/g, '');
    return cleaned.length === 10;
  },

  // Validate ZIP code
  validateZipCode: function(zip) {
    const re = /^\d{5}(-\d{4})?$/;
    return re.test(zip);
  },

  // Get user profile from localStorage
  getUserProfile: function() {
    const profile = localStorage.getItem('userProfile');
    return profile ? JSON.parse(profile) : null;
  },

  // Save user profile to localStorage
  saveUserProfile: function(profile) {
    localStorage.setItem('userProfile', JSON.stringify(profile));
  },

  // Clear user profile
  clearUserProfile: function() {
    localStorage.removeItem('userProfile');
  },

  // Get selected role
  getSelectedRole: function() {
    return localStorage.getItem('selectedRole');
  },

  // Set selected role
  setSelectedRole: function(role) {
    localStorage.setItem('selectedRole', role);
  }
};

// Form validation utilities
const FormValidator = {
  // Validate required fields
  validateRequired: function(form) {
    const requiredFields = form.querySelectorAll('[required]');
    let isValid = true;

    requiredFields.forEach(field => {
      if (!field.value.trim()) {
        field.classList.add('is-invalid');
        isValid = false;
      } else {
        field.classList.remove('is-invalid');
        field.classList.add('is-valid');
      }
    });

    return isValid;
  },

  // Validate email fields
  validateEmails: function(form) {
    const emailFields = form.querySelectorAll('input[type="email"]');
    let isValid = true;

    emailFields.forEach(field => {
      if (field.value && !Utils.validateEmail(field.value)) {
        field.classList.add('is-invalid');
        isValid = false;
      } else if (field.value) {
        field.classList.remove('is-invalid');
        field.classList.add('is-valid');
      }
    });

    return isValid;
  },

  // Validate phone fields
  validatePhones: function(form) {
    const phoneFields = form.querySelectorAll('input[type="tel"]');
    let isValid = true;

    phoneFields.forEach(field => {
      if (field.value && !Utils.validatePhone(field.value)) {
        field.classList.add('is-invalid');
        isValid = false;
      } else if (field.value) {
        field.classList.remove('is-invalid');
        field.classList.add('is-valid');
      }
    });

    return isValid;
  },

  // Validate ZIP code fields
  validateZipCodes: function(form) {
    const zipFields = form.querySelectorAll('input[name*="zipCode"], input[name*="zip"]');
    let isValid = true;

    zipFields.forEach(field => {
      if (field.value && !Utils.validateZipCode(field.value)) {
        field.classList.add('is-invalid');
        isValid = false;
      } else if (field.value) {
        field.classList.remove('is-invalid');
        field.classList.add('is-valid');
      }
    });

    return isValid;
  },

  // Validate entire form
  validateForm: function(form) {
    const requiredValid = this.validateRequired(form);
    const emailsValid = this.validateEmails(form);
    const phonesValid = this.validatePhones(form);
    const zipsValid = this.validateZipCodes(form);

    return requiredValid && emailsValid && phonesValid && zipsValid;
  }
};

// Progress tracking
const ProgressTracker = {
  // Update progress bar
  updateProgress: function(form, progressBarId) {
    const progressBar = document.getElementById(progressBarId);
    if (!progressBar) return;

    const requiredFields = form.querySelectorAll('[required]');
    const filledFields = Array.from(requiredFields).filter(field => {
      if (field.type === 'checkbox' || field.type === 'radio') {
        return field.checked;
      }
      return field.value.trim() !== '';
    });

    const progress = (filledFields.length / requiredFields.length) * 100;
    progressBar.style.width = progress + '%';
    progressBar.setAttribute('aria-valuenow', progress);
  },

  // Initialize progress tracking for a form
  init: function(form, progressBarId) {
    const updateProgress = () => this.updateProgress(form, progressBarId);
    
    form.querySelectorAll('input, select, textarea').forEach(field => {
      field.addEventListener('input', updateProgress);
      field.addEventListener('change', updateProgress);
    });

    // Initial progress update
    updateProgress();
  }
};

// Navigation utilities
const Navigation = {
  // Smooth scroll to element
  scrollTo: function(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth' });
    }
  },

  // Handle role selection
  selectRole: function(role) {
    Utils.setSelectedRole(role);
    
    switch(role) {
      case 'individual':
        window.location.href = 'individual-profile.html';
        break;
      case 'organization':
        window.location.href = 'organization-profile.html';
        break;
      case 'donor':
        window.location.href = 'donor-profile.html';
        break;
      default:
        Utils.showAlert('Invalid role selection', 'danger');
    }
  }
};

// Form submission handler
const FormHandler = {
  // Handle form submission
  handleSubmit: function(form, profileType) {
    form.addEventListener('submit', function(event) {
      event.preventDefault();
      event.stopPropagation();

      if (FormValidator.validateForm(form)) {
        // Collect form data
        const formData = new FormData(form);
        const profileData = Object.fromEntries(formData);
        
        // Add metadata
        profileData.role = profileType;
        profileData.timestamp = new Date().toISOString();
        profileData.id = 'user_' + Date.now();

        // Handle special cases
        if (profileData.donationTypes) {
          profileData.donationTypes = Array.from(form.querySelectorAll('input[name="donationTypes"]:checked')).map(cb => cb.value);
        }

        // Save profile
        Utils.saveUserProfile(profileData);
        
        // Show success message
        Utils.showAlert(`${profileType.charAt(0).toUpperCase() + profileType.slice(1)} profile created successfully!`, 'success');
        
        // Redirect to dashboard
        setTimeout(() => {
          window.location.href = 'dashboard.html';
        }, 2000);
      } else {
        Utils.showAlert('Please correct the errors in the form', 'danger');
      }

      form.classList.add('was-validated');
    });
  }
};

// Initialize common functionality
document.addEventListener('DOMContentLoaded', function() {
  // Add loading states to buttons
  const submitButtons = document.querySelectorAll('button[type="submit"]');
  submitButtons.forEach(button => {
    button.addEventListener('click', function() {
      this.classList.add('loading');
      this.disabled = true;
    });
  });

  // Add phone number formatting
  const phoneInputs = document.querySelectorAll('input[type="tel"]');
  phoneInputs.forEach(input => {
    input.addEventListener('input', function() {
      this.value = Utils.formatPhoneNumber(this.value);
    });
  });

  // Add smooth scrolling to anchor links
  const anchorLinks = document.querySelectorAll('a[href^="#"]');
  anchorLinks.forEach(link => {
    link.addEventListener('click', function(e) {
      e.preventDefault();
      const targetId = this.getAttribute('href').substring(1);
      Navigation.scrollTo(targetId);
    });
  });

  // Add keyboard navigation support
  document.addEventListener('keydown', function(e) {
    // ESC key to close modals/alerts
    if (e.key === 'Escape') {
      const alerts = document.querySelectorAll('.alert');
      alerts.forEach(alert => {
        const closeBtn = alert.querySelector('.btn-close');
        if (closeBtn) closeBtn.click();
      });
    }
  });
});

// Export utilities for global use
window.SupplyConnect = {
  Utils,
  FormValidator,
  ProgressTracker,
  Navigation,
  FormHandler
};
