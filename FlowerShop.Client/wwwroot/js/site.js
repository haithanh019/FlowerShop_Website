(function () {
    /* ── Scroll: shrink navbar ──────────────────────────────────────────────── */
    const navbar = document.querySelector('.navbar-floral');
    if (navbar) {
        window.addEventListener('scroll', () => {
            navbar.classList.toggle('is-scrolled', window.scrollY > 40);
        }, { passive: true });
    }

    /* ── Search overlay ─────────────────────────────────────────────────────── */
    const searchToggle = document.getElementById('searchToggle');
    const searchClose = document.getElementById('searchClose');
    const searchOverlay = document.getElementById('searchOverlay');
    const searchInput = document.getElementById('searchInput');

    function openSearch() {
        searchOverlay?.classList.add('is-open');
        searchToggle?.setAttribute('aria-expanded', 'true');
        setTimeout(() => searchInput?.focus(), 80);
        document.body.style.overflow = 'hidden';
    }

    function closeSearch() {
        searchOverlay?.classList.remove('is-open');
        searchToggle?.setAttribute('aria-expanded', 'false');
        document.body.style.overflow = '';
    }

    searchToggle?.addEventListener('click', openSearch);
    searchClose?.addEventListener('click', closeSearch);
    searchOverlay?.addEventListener('click', e => {
        if (e.target === searchOverlay) closeSearch();
    });

    /* ── Mobile Drawer ──────────────────────────────────────────────────────── */
    const drawerToggle = document.getElementById('drawerToggle');
    const drawerClose = document.getElementById('drawerClose');
    const mobileDrawer = document.getElementById('mobileDrawer');
    const drawerBackdrop = document.getElementById('drawerBackdrop');

    function openMobileDrawer() {
        mobileDrawer?.classList.add('is-open');
        drawerBackdrop?.classList.add('is-open');
        drawerToggle?.classList.add('is-open');
        drawerToggle?.setAttribute('aria-expanded', 'true');
        document.body.style.overflow = 'hidden';
    }

    function closeMobileDrawer() {
        mobileDrawer?.classList.remove('is-open');
        drawerBackdrop?.classList.remove('is-open');
        drawerToggle?.classList.remove('is-open');
        drawerToggle?.setAttribute('aria-expanded', 'false');
        document.body.style.overflow = '';
    }

    drawerToggle?.addEventListener('click', openMobileDrawer);
    drawerClose?.addEventListener('click', closeMobileDrawer);
    drawerBackdrop?.addEventListener('click', closeMobileDrawer);

    /* ── Keyboard shortcuts ─────────────────────────────────────────────────── */
    document.addEventListener('keydown', e => {
        if (e.key === 'Escape') { closeSearch(); closeMobileDrawer(); }
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            openSearch();
        }
    });

    /* ── Active nav link highlight ──────────────────────────────────────────── */
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.nav-link-floral, .drawer-nav a').forEach(link => {
        try {
            const linkPath = new URL(link.href).pathname.toLowerCase();
            const isActive = linkPath === '/'
                ? currentPath === '/'
                : currentPath.startsWith(linkPath);
            if (isActive) link.classList.add('active');
        } catch (_) { }
    });

    /* ── Toast utility (User) ───────────────────────────────────────────────── */
    window.showToast = function (msg, type = 'success') {
        const toast = document.getElementById('userToast');
        const msgEl = document.getElementById('userToastMsg');
        const iconEl = document.getElementById('userToastIcon');
        if (!toast) return;
        msgEl.textContent = msg;
        iconEl.textContent = type === 'success' ? '✅' : '❌';
        toast.className = 'user-toast ' + type + ' show';
        clearTimeout(toast._timer);
        toast._timer = setTimeout(() => toast.classList.remove('show'), 3500);
    };

    const toastEl = document.getElementById('toastData');
    if (toastEl) window.showToast(toastEl.dataset.msg, toastEl.dataset.type);

})();
