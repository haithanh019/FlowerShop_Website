document.addEventListener('DOMContentLoaded', function () {

    /* ── Toggle show/hide password ───────────────────────── */
    (function () {
        const btn = document.getElementById('togglePassword');
        const input = document.getElementById('passwordInput');
        const icon = document.getElementById('eyeIcon');

        if (!btn || !input) return;

        const eyeOpen = `<path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/>`;
        const eyeClosed = `<path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94"/><path d="M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19"/><line x1="1" y1="1" x2="23" y2="23"/>`;

        btn.addEventListener('click', function () {
            const isHidden = input.type === 'password';
            input.type = isHidden ? 'text' : 'password';
            icon.innerHTML = isHidden ? eyeClosed : eyeOpen;
        });
    })();

    /* ── Falling petals animation ────────────────────────── */
    (function () {
        const page = document.querySelector('.login-page');
        if (!page) return; // Thêm check an toàn nếu không tìm thấy class này

        const colors = ['#f7b8c4', '#f093a7', '#b8e0d4', '#8ecfbe', '#f7c5d0', '#d4e8e0'];
        const count = 12;

        for (let i = 0; i < count; i++) {
            const petal = document.createElement('div');
            petal.classList.add('petal');

            const size = 6 + Math.random() * 10;
            const color = colors[Math.floor(Math.random() * colors.length)];
            const delay = Math.random() * 14;
            const dur = 10 + Math.random() * 14;
            const left = Math.random() * 100;

            // Ellipse petal shape via SVG
            petal.innerHTML = `<svg width="${size * 2}" height="${size * 3}" viewBox="0 0 ${size * 2} ${size * 3}" aria-hidden="true"><ellipse cx="${size}" cy="${size * 1.5}" rx="${size}" ry="${size * 1.5}" fill="${color}" opacity="0.7"/></svg>`;

            Object.assign(petal.style, {
                left: left + 'vw',
                top: '-60px',
                animationDuration: dur + 's',
                animationDelay: delay + 's',
                animationIterationCount: 'infinite',
            });

            page.appendChild(petal);
        }
    })();

});