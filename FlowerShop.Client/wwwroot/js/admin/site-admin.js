// ── Toast utility ─────────────────────────────────────────────────────────────
function showAdminToast(msg, type = 'success') {
    const toast = document.getElementById('adminToast');
    const msgEl = document.getElementById('adminToastMsg');
    const iconEl = document.getElementById('adminToastIcon');
    if (!toast) return;

    msgEl.textContent = msg;
    iconEl.textContent = type === 'success' ? '✅' : '❌';
    toast.className = 'admin-toast ' + type + ' show';

    clearTimeout(toast._timer);
    toast._timer = setTimeout(() => toast.classList.remove('show'), 3500);
}

// ── Modal helpers ─────────────────────────────────────────────────────────────
function openModal(id) {
    document.getElementById(id).classList.add('open');
    document.body.style.overflow = 'hidden';
}

function closeModal(id) {
    document.getElementById(id).classList.remove('open');
    document.body.style.overflow = '';
}

// ── DOMContentLoaded ──────────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {

    // Auto-show toast từ TempData (qua data attribute — an toàn)
    const toastEl = document.getElementById('toastData');
    if (toastEl) showAdminToast(toastEl.dataset.msg, toastEl.dataset.type);

    // Close modal khi click overlay
    document.querySelectorAll('.admin-modal-overlay').forEach(overlay => {
        overlay.addEventListener('click', function (e) {
            if (e.target === this) closeModal(this.id);
        });
    });

    // Bind nút [data-close-modal]
    document.querySelectorAll('[data-close-modal]').forEach(btn => {
        btn.addEventListener('click', () => closeModal(btn.dataset.closeModal));
    });
});
