async function cancelOrder(orderID, btn) {
    if (!confirm('Bạn có chắc muốn hủy đơn hàng này không?')) return;

    btn.disabled = true;
    btn.textContent = 'Đang hủy...';

    try {
        const res = await fetch(`/Order/CancelOrder?id=${orderID}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });

        const data = await res.json();

        if (data.success) {
            window.showToast('Đã hủy đơn hàng thành công.', 'success');

            const card = document.getElementById('order-' + orderID);
            if (card) {
                const badge = card.querySelector('.order-badge');
                if (badge) {
                    badge.className = 'order-badge badge-cancelled';
                    badge.textContent = 'Đã hủy';
                }
                btn.remove();
            }
        } else {
            window.showToast(data.message || 'Hủy thất bại.', 'error');
            btn.disabled = false;
            btn.textContent = 'Hủy đơn';
        }
    } catch (err) {
        console.error(err);
        window.showToast('Lỗi kết nối.', 'error');
        btn.disabled = false;
        btn.textContent = 'Hủy đơn';
    }
}
