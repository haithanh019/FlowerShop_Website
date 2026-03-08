/**
 * FlowerShop — Home Page JS
 * Countdown · Wishlist · Add to Cart · Load More · Category filter
 */
(function () {
    'use strict';

    /* ── 1. Flash Sale Countdown ───────────────────────────── */
    function initCountdown() {
        const hh = document.getElementById('cd-hh');
        const mm = document.getElementById('cd-mm');
        const ss = document.getElementById('cd-ss');
        if (!hh) return;

        const end = new Date();
        end.setHours(23, 59, 59, 0);

        const pad = n => String(n).padStart(2, '0');

        (function tick() {
            const d = end - Date.now();
            if (d <= 0) { hh.textContent = mm.textContent = ss.textContent = '00'; return; }
            const t = Math.floor(d / 1000);
            hh.textContent = pad(Math.floor(t / 3600));
            mm.textContent = pad(Math.floor((t % 3600) / 60));
            ss.textContent = pad(t % 60);
            setTimeout(tick, 1000);
        })();
    }

    /* ── 2. Wishlist toggle ────────────────────────────────── */
    function initWishlist() {
        document.querySelectorAll('.p-wish').forEach(btn => {
            btn.addEventListener('click', function (e) {
                e.preventDefault(); e.stopPropagation();
                this.classList.toggle('on');
                const path = this.querySelector('path');
                if (path) path.setAttribute('fill', this.classList.contains('on') ? 'currentColor' : 'none');
                // TODO: fetch(`/Wishlist/Toggle/${this.dataset.id}`, { method: 'POST' });
            });
        });
    }

    /* ── 3. Add to cart ────────────────────────────────────── */
    function initAddToCart() {
        document.querySelectorAll('.p-add').forEach(btn => {
            btn.addEventListener('click', function (e) {
                e.preventDefault(); e.stopPropagation();
                const orig = this.innerHTML;
                this.innerHTML = `<svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><polyline points="20 6 9 17 4 12"/></svg> Đã thêm!`;
                this.style.background = 'linear-gradient(90deg, #6bbfaa, #8ecfbe)';
                setTimeout(() => { this.innerHTML = orig; this.style.background = ''; }, 1500);
                // TODO: fetch('/Cart/Add', { method:'POST', body: JSON.stringify({ id: this.dataset.id, qty: 1 }) });
            });
        });
    }

    /* ── 4. Category filter ────────────────────────────────── */
    function initCatFilter() {
        const chips = document.querySelectorAll('.cat-chip[data-cat]');
        const items = document.querySelectorAll('.p-grid-item');
        if (!chips.length) return;

        chips.forEach(chip => {
            chip.addEventListener('click', function (e) {
                e.preventDefault();
                chips.forEach(c => c.classList.remove('active'));
                this.classList.add('active');
                const cat = this.dataset.cat;

                items.forEach(item => {
                    const show = !cat || item.dataset.cat === cat;
                    item.style.display = show ? '' : 'none';
                });
            });
        });
    }

    /* ── 5. Load More ──────────────────────────────────────── */
    function initLoadMore() {
        const btn = document.getElementById('btnLoadMore');
        if (!btn) return;

        let page = 1;

        btn.addEventListener('click', async function () {
            page++;
            btn.classList.add('loading');

            try {
                const res = await fetch(`/Home/LoadMore?page=${page}`, {
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                });
                if (!res.ok) throw new Error();

                const html = await res.text();
                const grid = document.getElementById('productGrid');
                const tmp = document.createElement('div');
                tmp.innerHTML = html;

                tmp.querySelectorAll('.p-grid-item').forEach((item, i) => {
                    item.style.opacity = '0';
                    item.style.transform = 'translateY(12px)';
                    grid.appendChild(item);
                    setTimeout(() => {
                        item.style.transition = 'opacity .3s ease, transform .3s ease';
                        item.style.opacity = '1';
                        item.style.transform = 'translateY(0)';
                    }, i * 55);
                });

                if (!tmp.querySelectorAll('.p-grid-item').length || tmp.dataset.last === 'true') {
                    btn.parentElement.remove();
                }

                initWishlist(); initAddToCart();

            } catch (_) {
                page--;
            } finally {
                btn.classList.remove('loading');
            }
        });
    }

    /* ── 6. Wheel scroll on category row ──────────────────── */
    function initCatWheel() {
        const wrap = document.querySelector('.cat-scroll');
        if (!wrap) return;
        wrap.addEventListener('wheel', function (e) {
            if (e.deltaY) { e.preventDefault(); this.scrollLeft += e.deltaY; }
        }, { passive: false });
    }

    /* ── INIT ─────────────────────────────────────────────── */
    document.addEventListener('DOMContentLoaded', function () {
        initCountdown();
        initWishlist();
        initAddToCart();
        initCatFilter();
        initLoadMore();
        initCatWheel();
    });

})();
