document.querySelectorAll('.qty-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        const inputId = this.getAttribute('data-form');
        const input = document.getElementById(inputId);
        if (!input) return;

        let val = parseInt(input.value) || 1;

        if (this.classList.contains('qty-plus')) {
            val = Math.min(val + 1, 999);
        } else if (this.classList.contains('qty-minus')) {
            val = Math.max(val - 1, 1);
        }

        input.value = val;
    });
});
