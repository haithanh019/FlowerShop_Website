(function () {
    const PaymentMethodEnum = { COD: 0, PayOS: 1 };

    // Highlight payment option khi chọn
    document.querySelectorAll('input[name="paymentMethod"]').forEach(radio => {
        radio.addEventListener('change', () => {
            document.querySelectorAll('.payment-option').forEach(el =>
                el.classList.remove('border-primary', 'bg-light'));
            document.querySelector('input[name="paymentMethod"]:checked')
                ?.closest('.payment-option')
                ?.classList.add('border-primary', 'bg-light');
        });
    });
    document.getElementById('optionCOD')?.classList.add('border-primary', 'bg-light');

    function setLoading(on) {
        document.getElementById('btnPlaceOrder').disabled = on;
        document.getElementById('btnText').textContent = on ? 'Đang xử lý...' : 'Đặt hàng ngay';
        document.getElementById('btnSpinner').classList.toggle('d-none', !on);
    }

    function validate() {
        let ok = true;
        const phone = document.getElementById('phoneNumber');
        const address = document.getElementById('shippingAddress');
        const errPhone = document.getElementById('err-phone');
        const errAddress = document.getElementById('err-address');

        [phone, address].forEach(el => el.classList.remove('is-invalid'));

        if (!phone.value.trim()) {
            phone.classList.add('is-invalid');
            errPhone.textContent = 'Vui lòng nhập số điện thoại.';
            ok = false;
        } else if (!/^[0-9]{9,11}$/.test(phone.value.trim())) {
            phone.classList.add('is-invalid');
            errPhone.textContent = 'Số điện thoại không hợp lệ.';
            ok = false;
        }

        if (!address.value.trim()) {
            address.classList.add('is-invalid');
            errAddress.textContent = 'Vui lòng nhập địa chỉ giao hàng.';
            ok = false;
        }

        return ok;
    }

    document.getElementById('btnPlaceOrder')?.addEventListener('click', async () => {
        if (!validate()) return;

        const cartID = document.getElementById('cartID').value;
        const shippingFee = parseFloat(document.getElementById('shippingFee').value) || 0;
        const totalAmount = parseFloat(document.getElementById('totalAmount').value) || 0;
        const paymentMethodStr = document.querySelector('input[name="paymentMethod"]:checked')?.value || 'COD';

        const payload = {
            cartID,
            shippingAddress: document.getElementById('shippingAddress').value.trim(),
            phoneNumber: document.getElementById('phoneNumber').value.trim(),
            note: document.getElementById('orderNote').value.trim(),
            shippingFee,
            paymentMethod: PaymentMethodEnum[paymentMethodStr]  // 0 = COD, 1 = PayOS
        };

        setLoading(true);
        try {
            // BƯỚC 1: Tạo đơn hàng
            const orderRes = await fetch('/Order/PlaceOrder', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            if (!orderRes.ok) {
                window.showToast(`Lỗi server (${orderRes.status}). Vui lòng thử lại.`, 'error');
                setLoading(false);
                return;
            }

            const orderData = await orderRes.json();
            if (!orderData.success) {
                window.showToast(orderData.message || 'Đặt hàng thất bại.', 'error');
                setLoading(false);
                return;
            }

            const orderID = orderData.orderID;

            // BƯỚC 2: Xử lý theo phương thức thanh toán
            if (paymentMethodStr === 'COD') {
                await fetch(`/Order/CreateCODPayment?orderID=${orderID}&amount=${totalAmount}`, {
                    method: 'POST'
                });
                window.location.href = `/Order/Confirmation/${orderID}`;
            } else {
                const payRes = await fetch(`/Order/CreatePayOSPayment?orderID=${orderID}&amount=${totalAmount}`, {
                    method: 'POST'
                });
                const payData = await payRes.json();
                if (payData.success && payData.paymentUrl) {
                    window.location.href = payData.paymentUrl;
                } else {
                    window.showToast(payData.message || 'Không thể tạo link thanh toán.', 'error');
                    setLoading(false);
                }
            }
        } catch {
            window.showToast('Lỗi kết nối.', 'error');
            setLoading(false);
        }
    });
})();