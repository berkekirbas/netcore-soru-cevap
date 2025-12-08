// Toast Notification System
const ToastNotification = {
    container: null,

    init: function() {
        if (!this.container) {
            this.container = document.createElement('div');
            this.container.className = 'toast-container';
            document.body.appendChild(this.container);
        }
    },

    show: function(message, type = 'info', duration = 5000) {
        this.init();

        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        toast.setAttribute('role', 'alert');
        toast.setAttribute('aria-live', 'assertive');
        toast.setAttribute('aria-atomic', 'true');

        const icons = {
            success: 'fa-check-circle',
            error: 'fa-exclamation-circle',
            warning: 'fa-exclamation-triangle',
            info: 'fa-info-circle'
        };

        const titles = {
            success: 'Başarılı',
            error: 'Hata',
            warning: 'Uyarı',
            info: 'Bilgi'
        };

        toast.innerHTML = `
            <div class="toast-header">
                <i class="fas ${icons[type]} me-2"></i>
                <strong class="me-auto">${titles[type]}</strong>
                <button type="button" class="btn-close" aria-label="Close"></button>
            </div>
            <div class="toast-body">
                ${message}
            </div>
        `;

        this.container.appendChild(toast);

        const closeBtn = toast.querySelector('.btn-close');
        closeBtn.addEventListener('click', () => {
            this.hide(toast);
        });

        if (duration > 0) {
            setTimeout(() => {
                this.hide(toast);
            }, duration);
        }
    },

    hide: function(toast) {
        toast.classList.add('hiding');
        setTimeout(() => {
            toast.remove();
        }, 300);
    },

    success: function(message, duration = 5000) {
        this.show(message, 'success', duration);
    },

    error: function(message, duration = 5000) {
        this.show(message, 'error', duration);
    },

    warning: function(message, duration = 5000) {
        this.show(message, 'warning', duration);
    },

    info: function(message, duration = 5000) {
        this.show(message, 'info', duration);
    }
};

// Loading Overlay
const LoadingOverlay = {
    overlay: null,

    show: function() {
        if (!this.overlay) {
            this.overlay = document.createElement('div');
            this.overlay.className = 'loading-overlay';
            this.overlay.innerHTML = '<div class="spinner-border-custom"></div>';
            document.body.appendChild(this.overlay);
        }
        this.overlay.style.display = 'flex';
    },

    hide: function() {
        if (this.overlay) {
            this.overlay.style.display = 'none';
        }
    }
};

// Global error handler for AJAX requests
$(document).ajaxError(function(event, jqxhr, settings, thrownError) {
    LoadingOverlay.hide();
    ToastNotification.error('Bir hata oluştu. Lütfen tekrar deneyin.');
});

// Make available globally
window.ToastNotification = ToastNotification;
window.LoadingOverlay = LoadingOverlay;
