(function () {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

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

        // Reset
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

        const payload = {
            cartID,
            shippingAddress: document.getElementById('shippingAddress').value.trim(),
            phoneNumber: document.getElementById('phoneNumber').value.trim(),
            shippingFee
        };

        setLoading(true);
        try {
            const res = await fetch('/Order/PlaceOrder', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(payload)
            });

            if (!res.ok) {
                const text = await res.text();
                console.error('HTTP Error:', res.status, text);
                window.showToast(`Lỗi server (${res.status}). Vui lòng thử lại.`, 'error');
                setLoading(false);
                return;
            }

            const data = await res.json();

            if (data.success) {
                window.location.href = `/Order/Confirmation/${data.orderID}`;
            } else {
                window.showToast(data.message || 'Đặt hàng thất bại.', 'error');
                setLoading(false);
            }
        } catch {
            window.showToast('Lỗi kết nối.', 'error');
            setLoading(false);
        }
    });

})();
