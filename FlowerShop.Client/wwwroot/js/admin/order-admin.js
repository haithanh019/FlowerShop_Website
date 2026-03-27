// ── Biến toàn cục ─────────────────────────────────────────────────────────
let _currentOrderID = null;

const STATUS_LABEL = {
    Processing: 'Chờ xác nhận',
    Confirmed: 'Đã xác nhận',
    Delivering: 'Đang giao',
    Completed: 'Hoàn thành',
    Cancelled: 'Đã hủy'
};

const STATUS_BADGE = {
    Processing: 'badge-pending',
    Confirmed: 'badge-confirmed',
    Delivering: 'badge-shipping',
    Completed: 'badge-completed',
    Cancelled: 'badge-cancelled'
};

const STATUS_VALUE = {
    Processing: 0, Confirmed: 1, Delivering: 2, Completed: 3, Cancelled: 4
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
        Confirmed: ['Delivering', 'Cancelled'],
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

    // Map enum PaymentMethod
    const PAYMENT_LABEL = { 0: '💵 COD (Tiền mặt)', 1: '💳 PayOS (Chuyển khoản)' };
    const paymentLabel = PAYMENT_LABEL[order.paymentMethod] ?? '—';

    const itemsHtml = items.map(it => {
        const imgHtml = it.imageUrl
            ? `<img src="${it.imageUrl}" alt="${it.flowerName}"
                    style="width:48px;height:48px;object-fit:cover;
                           border-radius:10px;flex-shrink:0;
                           border:1px solid #f0e0e0" />`
            : `<div style="width:48px;height:48px;border-radius:10px;
                           background:linear-gradient(135deg,rgba(248,193,193,.25),rgba(179,224,245,.25));
                           display:flex;align-items:center;justify-content:center;
                           font-size:1.3rem;flex-shrink:0">🌺</div>`;
        return `
        <div style="display:flex;align-items:center;gap:12px;
                    padding:8px 0;border-bottom:1px dashed #f0e0e0">
            ${imgHtml}
            <div style="flex:1;min-width:0">
                <div style="font-weight:600;font-size:.88rem;
                            white-space:nowrap;overflow:hidden;text-overflow:ellipsis">
                    ${it.flowerName}
                </div>
                <div style="color:#aaa;font-size:.78rem">
                    ${it.unitPrice.toLocaleString('vi-VN')} đ × ${it.quantity}
                </div>
            </div>
            <div style="font-weight:700;color:#d66b6b;white-space:nowrap;font-size:.88rem">
                ${it.lineTotal.toLocaleString('vi-VN')} đ
            </div>
        </div>`;
    }).join('');

    document.getElementById('detailBody').innerHTML = `
        <!-- Thông tin giao hàng -->
        <div style="font-size:.85rem;color:#666;margin-bottom:14px;
                    padding:10px 12px;background:rgba(248,193,193,.08);
                    border-radius:10px;border:1px dashed rgba(214,107,107,.2)">
            <div>📍 <strong>Giao đến:</strong> ${order.shippingAddress ?? '—'}</div>
            <div style="margin-top:4px">📞 <strong>SĐT:</strong> ${order.phoneNumber ?? '—'}</div>
            <div style="margin-top:4px">💳 <strong>Thanh toán:</strong> ${paymentLabel}</div>
            ${order.note ? `<div style="margin-top:4px">📝 <strong>Ghi chú:</strong> ${order.note}</div>` : ''}
        </div>

        <!-- Danh sách sản phẩm -->
        <div style="margin-bottom:14px">
            ${itemsHtml || '<p style="color:#aaa;text-align:center;padding:16px 0">Không có sản phẩm</p>'}
        </div>

        <!-- Tổng tiền -->
        <div style="font-size:.87rem;padding-top:10px;border-top:1px solid #f0e0e0">
            <div style="display:flex;justify-content:space-between;color:#888;margin-bottom:4px">
                <span>Tạm tính</span><span>${subtotal.toLocaleString('vi-VN')} đ</span>
            </div>
            <div style="display:flex;justify-content:space-between;color:#888;margin-bottom:8px">
                <span>Phí vận chuyển</span><span>${shipping.toLocaleString('vi-VN')} đ</span>
            </div>
            <div style="display:flex;justify-content:space-between;
                        font-weight:700;font-size:1rem;color:#d66b6b">
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
