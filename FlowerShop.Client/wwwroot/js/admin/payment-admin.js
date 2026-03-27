let _currentPayID = null;

const PAY_STATUS_LABEL = { 0: 'Chờ thanh toán', 1: 'Đã thanh toán', 2: 'Thất bại', 3: 'Đã hoàn tiền' };
const PAY_STATUS_KEY = { 0: 'Pending', 1: 'Paid', 2: 'Failed', 3: 'Refunded' };
const PAY_STATUS_BADGE = {
    Pending: 'badge-pending', Paid: 'badge-completed',
    Failed: 'badge-cancelled', Refunded: 'badge-shipping'
};

document.addEventListener('click', function (e) {
    const btn = e.target.closest('.btn-pay-update');
    if (!btn) return;
    _currentPayID = btn.dataset.payId;
    document.getElementById('payOrderCode').textContent = btn.dataset.orderCode;
    openModal('modalPayStatus');
});

async function submitPayStatus() {
    const statusValue = parseInt(document.getElementById('selectPayStatus').value);
    try {
        const res = await fetch(`/Admin/Payment/UpdateStatus?id=${_currentPayID}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ paymentStatus: statusValue })
        });
        const data = await res.json();
        if (data.success) {
            showAdminToast('Cập nhật thành công!', 'success');
            closeModal('modalPayStatus');
            const key = PAY_STATUS_KEY[statusValue];
            const badge = document.getElementById('paybadge-' + _currentPayID);
            if (badge && key) {
                badge.className = `order-admin-badge ${PAY_STATUS_BADGE[key]}`;
                badge.textContent = PAY_STATUS_LABEL[statusValue];
            }
            if (key !== 'Pending') {
                badge?.closest('tr')?.querySelector('.btn-pay-update')?.remove();
            }
        } else {
            showAdminToast(data.message || 'Cập nhật thất bại.', 'error');
        }
    } catch {
        showAdminToast('Lỗi kết nối.', 'error');
    }
}

// Filter + Search
document.getElementById('filterPayStatus')?.addEventListener('change', applyPayFilter);
document.getElementById('paySearch')?.addEventListener('input', applyPayFilter);

function applyPayFilter() {
    const status = document.getElementById('filterPayStatus').value.toLowerCase();
    const keyword = document.getElementById('paySearch').value.toLowerCase();
    document.querySelectorAll('#paymentTable tbody tr[data-paystatus]').forEach(row => {
        const matchStatus = !status || row.dataset.paystatus.toLowerCase() === status;
        const matchKeyword = !keyword || row.dataset.search.toLowerCase().includes(keyword);
        row.style.display = matchStatus && matchKeyword ? '' : 'none';
    });
}