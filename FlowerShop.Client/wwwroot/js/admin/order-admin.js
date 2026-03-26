// ── Biến toàn cục ─────────────────────────────────────────────────────────
let _currentOrderID = null;

const STATUS_LABEL = {
    Pending: 'Chờ xác nhận',
    Confirmed: 'Đã xác nhận',
    Shipping: 'Đang giao',
    Completed: 'Hoàn thành',
    Cancelled: 'Đã hủy'
};

const STATUS_BADGE = {
    Pending: 'badge-pending',
    Confirmed: 'badge-confirmed',
    Shipping: 'badge-shipping',
    Completed: 'badge-completed',
    Cancelled: 'badge-cancelled'
};

const STATUS_VALUE = {
    Pending: 0, Confirmed: 1, Shipping: 2, Completed: 3, Cancelled: 4
};

document.addEventListener('click', function (e) {
    const btn = e.target.closest('.btn-order-status');
    if (!btn) return;
    openStatusModal(btn.dataset.orderId, btn.dataset.status, btn.dataset.code);
});

document.addEventListener('click', function (e) {
    const btn = e.target.closest('.btn-order-detail');
    if (!btn) return;
    try {
        const order = JSON.parse(btn.dataset.order);
        openDetailModal(order);
    } catch (err) {
        console.error('Parse order JSON thất bại:', err);
        showAdminToast('Không thể đọc dữ liệu đơn hàng.', 'error');
    }
});

// ── Modal cập nhật trạng thái ─────────────────────────────────────────────
function openStatusModal(orderID, currentStatus, orderCode) {
    _currentOrderID = orderID;
    document.getElementById('statusOrderCode').textContent = orderCode;

    const nextMap = {
        Pending: ['Confirmed', 'Cancelled'],
        Confirmed: ['Shipping', 'Cancelled'],
        Shipping: ['Completed', 'Cancelled']
    };
    const options = nextMap[currentStatus] ?? [];

    const select = document.getElementById('selectStatus');
    select.innerHTML = options.map(s =>
        `<option value="${STATUS_VALUE[s]}">${STATUS_LABEL[s]}</option>`
    ).join('');

    openModal('modalStatus');
}

async function submitStatus() {
    const statusValue = parseInt(document.getElementById('selectStatus').value);

    try {                                         
        const res = await fetch(`/Admin/Order/UpdateStatus?id=${_currentOrderID}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ orderStatus: statusValue })
        });

        const data = await res.json();

        if (data.success) {
            showAdminToast('Cập nhật trạng thái thành công!', 'success');
            closeModal('modalStatus');

            const statusKey = Object.keys(STATUS_VALUE)
                .find(k => STATUS_VALUE[k] === statusValue);

            const badge = document.getElementById('badge-' + _currentOrderID);
            if (badge && statusKey) {
                badge.className = `order-admin-badge ${STATUS_BADGE[statusKey]}`;
                badge.textContent = STATUS_LABEL[statusKey];
            }

            if (statusKey === 'Completed' || statusKey === 'Cancelled') {
                const row = badge?.closest('tr');
                row?.querySelector('.btn-order-status')?.remove();
            }
        } else {
            showAdminToast(data.message || 'Cập nhật thất bại.', 'error');
        }
    } catch (err) {                              
        console.error(err);
        showAdminToast('Lỗi kết nối.', 'error');
    }
}

function openDetailModal(order) {
    const items = order.orderItems ?? [];
    const subtotal = order.subtotal ?? 0;
    const shipping = order.shippingFee ?? 0;
    const total = order.totalAmount ?? 0;

    const itemsHtml = items.map(it => `
        <div style="display:flex;justify-content:space-between;
                    align-items:center;padding:6px 0;
                    border-bottom:1px dashed #f0e0e0;font-size:.87rem">
            <div>
                <div style="font-weight:600">${it.flowerName}</div>
                <div style="color:#aaa">${it.unitPrice.toLocaleString('vi-VN')} đ × ${it.quantity}</div>
            </div>
            <div style="font-weight:700;color:#d66b6b">
                ${it.lineTotal.toLocaleString('vi-VN')} đ
            </div>
        </div>`).join('');

    document.getElementById('detailBody').innerHTML = `
        <div style="font-size:.85rem;color:#888;margin-bottom:12px">
            <strong>📍 Giao đến:</strong> ${order.shippingAddress ?? '—'}<br/>
            <strong>📞 SĐT:</strong> ${order.phoneNumber ?? '—'}
        </div>
        <div>${itemsHtml || '<p style="color:#aaa;text-align:center">Không có sản phẩm</p>'}</div>
        <div style="margin-top:14px;font-size:.87rem">
            <div style="display:flex;justify-content:space-between;color:#888">
                <span>Tạm tính</span><span>${subtotal.toLocaleString('vi-VN')} đ</span>
            </div>
            <div style="display:flex;justify-content:space-between;color:#888">
                <span>Phí vận chuyển</span><span>${shipping.toLocaleString('vi-VN')} đ</span>
            </div>
            <div style="display:flex;justify-content:space-between;
                        font-weight:700;font-size:1rem;margin-top:8px;color:#d66b6b">
                <span>Tổng cộng</span><span>${total.toLocaleString('vi-VN')} đ</span>
            </div>
        </div>`;

    openModal('modalDetail');
}

// ── Filter + Search ───────────────────────────────────────────────────────
document.getElementById('filterStatus')?.addEventListener('change', applyFilter);
document.getElementById('orderSearch')?.addEventListener('input', applyFilter);

function applyFilter() {
    const status = document.getElementById('filterStatus').value.toLowerCase();
    const keyword = document.getElementById('orderSearch').value.toLowerCase();

    document.querySelectorAll('#orderTable tbody tr[data-status]').forEach(row => {
        const matchStatus = !status || row.dataset.status.toLowerCase() === status;
        const matchKeyword = !keyword || row.dataset.search.toLowerCase().includes(keyword);
        row.style.display = matchStatus && matchKeyword ? '' : 'none';
    });
}                                                  
