// ── Filter & Search ───────────────────────────────────────────────────────────
(() => {
    const searchInput = document.getElementById('flowerSearch');
    const stockFilter = document.getElementById('stockFilter');
    const resetButton = document.getElementById('resetFilter');
    const items = Array.from(document.querySelectorAll('.flower-item'));
    const noResultBox = document.getElementById('noResultBox');

    function applyFilter() {
        const keyword = (searchInput?.value || '').trim().toLowerCase();
        const stock = stockFilter?.value || 'all';
        let visibleCount = 0;

        items.forEach(item => {
            const name = item.dataset.name || '';
            const itemStock = item.dataset.stock || '';

            const matchKeyword = !keyword || name.includes(keyword);
            const matchStock = stock === 'all' || stock === itemStock;
            const show = matchKeyword && matchStock;

            item.classList.toggle('d-none', !show);
            if (show) visibleCount++;
        });

        if (noResultBox) {
            noResultBox.classList.toggle('d-none', visibleCount !== 0);
        }
    }

    searchInput?.addEventListener('input', applyFilter);
    stockFilter?.addEventListener('change', applyFilter);

    resetButton?.addEventListener('click', () => {
        if (searchInput) searchInput.value = '';
        if (stockFilter) stockFilter.value = 'all';
        applyFilter();
    });
})();
