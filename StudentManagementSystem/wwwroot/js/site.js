// site.js — small helper scripts

// Auto-dismiss toasts after 4 seconds
document.addEventListener('DOMContentLoaded', function () {
    const toasts = document.querySelectorAll('.toast-success, .toast-error');
    toasts.forEach(t => {
        setTimeout(() => t.remove(), 4000);
    });
});
