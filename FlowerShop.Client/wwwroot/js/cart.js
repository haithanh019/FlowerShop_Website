(function () {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    const MAX_PER_ITEM = 10;
    const MAX_CART_QTY = 10;
    const CONTACT_MSG = 'Vui lòng liên hệ trực tiếp người bán để đặt số lượng lớn.';

    function debounce(fn, delay) {
        let timer;
        return function (...args) {
            clearTimeout(timer);
            timer = setTimeout(() => fn.apply(this, args), delay);
        };
    }

    function getTotalCartQty(excludeItemID) {
        let total = 0;
        document.querySelectorAll('.cart-item-row').forEach(row => {
            if (row.dataset.itemId === excludeItemID) return;
            total += parseInt(row.querySelector('.qty-input')?.value) || 0;
        });
        return total;
    }

    function updateItemTotal(row, itemID, qty) {
        const unitPrice = parseFloat(row.dataset.unitPrice) || 0;
        const totalEl = document.getElementById('total-' + itemID);
        if (totalEl) totalEl.textContent = (qty * unitPrice).toLocaleString('vi-VN') + ' đ';
    }

    function recalcGrandTotal() {
        let total = 0;
        let totalQty = 0;
        let itemCount = 0;

        document.querySelectorAll('.cart-item-row').forEach(row => {
            const qty = parseInt(row.querySelector('.qty-input')?.value) || 0;
            const unitPrice = parseFloat(row.dataset.unitPrice) || 0;
            total += qty * unitPrice;
            totalQty += qty;
            itemCount++;
        });

        const grandTotalEl = document.getElementById('grandTotal');
        const totalQtyEl = document.getElementById('summaryTotalQty');
        const itemCountEl = document.getElementById('summaryItemCount');

        if (grandTotalEl) grandTotalEl.textContent = total.toLocaleString('vi-VN') + ' đ';
        if (totalQtyEl) totalQtyEl.textContent = totalQty;
        if (itemCountEl) itemCountEl.textContent = itemCount;
    }

    async function updateQty(cartItemID, quantity, savingEl) {
        if (savingEl) savingEl.style.display = 'block';
        try {
            const res = await fetch(`/Cart/UpdateQuantityAjax?cartItemID=${cartItemID}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({ quantity })
            });
            const data = await res.json();
            if (!data.success) window.showToast(data.message || 'Cập nhật thất bại.', 'error');
        } catch {
            window.showToast('Lỗi kết nối.', 'error');
        } finally {
            if (savingEl) savingEl.style.display = 'none';
        }
    }

    async function removeItemByID(itemID) {
        try {
            const res = await fetch(`/Cart/RemoveItemAjax?cartItemID=${itemID}`, {
                method: 'POST',
                headers: { 'RequestVerificationToken': token }
            });
            const data = await res.json();
            if (data.success) {
                document.getElementById('item-' + itemID)?.remove();
                recalcGrandTotal();
                window.showToast('Đã xóa sản phẩm.', 'success');
            } else {
                window.showToast(data.message || 'Xóa thất bại.', 'error');
            }
        } catch {
            window.showToast('Lỗi kết nối.', 'error');
        }
    }

    document.querySelectorAll('.cart-item-row').forEach(row => {
        const itemID = row.dataset.itemId;
        const input = row.querySelector('.qty-input');
        const savingEl = document.getElementById('saving-' + itemID);

        const debouncedUpdate = debounce(qty => updateQty(itemID, qty, savingEl), 600);

        row.querySelector('.qty-plus')?.addEventListener('click', () => {
            const cur = parseInt(input.value) || 1;
            const next = cur + 1;

            if (next > MAX_PER_ITEM) {
                window.showToast(`Tối đa ${MAX_PER_ITEM} bó / loại hoa. ${CONTACT_MSG}`, 'error');
                return;
            }
            if (getTotalCartQty(itemID) + next > MAX_CART_QTY) {
                window.showToast(`Giỏ hàng tối đa ${MAX_CART_QTY} bó hoa. ${CONTACT_MSG}`, 'error');
                return;
            }

            input.value = next;
            updateItemTotal(row, itemID, next);
            recalcGrandTotal();
            debouncedUpdate(next);
        });

        row.querySelector('.qty-minus')?.addEventListener('click', () => {
            const cur = parseInt(input.value) || 1;
            if (cur <= 1) {
                if (!confirm('Xóa sản phẩm này khỏi giỏ?')) return;
                removeItemByID(itemID);
                return;
            }
            const val = cur - 1;
            input.value = val;
            updateItemTotal(row, itemID, val);
            recalcGrandTotal();
            debouncedUpdate(val);
        });

        input?.addEventListener('input', debounce(() => {
            let val = parseInt(input.value) || 1;

            if (val > MAX_PER_ITEM) {
                window.showToast(`Tối đa ${MAX_PER_ITEM} bó / loại hoa. ${CONTACT_MSG}`, 'error');
                val = MAX_PER_ITEM;
            }

            const otherQty = getTotalCartQty(itemID);
            if (otherQty + val > MAX_CART_QTY) {
                window.showToast(`Giỏ hàng tối đa ${MAX_CART_QTY} bó hoa. ${CONTACT_MSG}`, 'error');
                val = Math.max(1, MAX_CART_QTY - otherQty);
            }

            val = Math.max(1, val);
            input.value = val;
            updateItemTotal(row, itemID, val);
            recalcGrandTotal();
            updateQty(itemID, val, savingEl);
        }, 800));
    });

    window.removeItem = function (btn) {
        const itemID = btn.dataset.itemId;
        if (!confirm('Xóa sản phẩm này khỏi giỏ?')) return;
        removeItemByID(itemID);
    };

})();
