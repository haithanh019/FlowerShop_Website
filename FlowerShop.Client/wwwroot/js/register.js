(function () {
    /* ─── Helpers ──────────────────────────────────────── */
    const eyeOpen = `<path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/>`;
    const eyeClosed = `<path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94"/><path d="M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19"/><line x1="1" y1="1" x2="23" y2="23"/>`;

    function makeToggle(btnId, inputId, iconId) {
        const btn = document.getElementById(btnId);
        const input = document.getElementById(inputId);
        const icon = document.getElementById(iconId);

        if (!btn || !input || !icon) return;

        btn.addEventListener("click", () => {
            const hidden = input.type === "password";
            input.type = hidden ? "text" : "password";
            icon.innerHTML = hidden ? eyeClosed : eyeOpen;
        });
    }

    makeToggle("togglePassword", "passwordInput", "eyeIcon1");
    makeToggle("toggleConfirm", "confirmInput", "eyeIcon2");

    /* ─── Password strength ────────────────────────────── */
    const passInput = document.getElementById("passwordInput");
    const strengthWrap = document.getElementById("strengthWrap");
    const strengthLbl = document.getElementById("strengthLabel");

    const bars = [
        document.getElementById("sb1"),
        document.getElementById("sb2"),
        document.getElementById("sb3"),
        document.getElementById("sb4"),
    ].filter(Boolean);

    const levels = [
        { cls: "active-weak", label: "Yếu", labelCls: "weak" },
        { cls: "active-fair", label: "Trung bình", labelCls: "fair" },
        { cls: "active-good", label: "Khá mạnh", labelCls: "good" },
        { cls: "active-strong", label: "Rất mạnh 🌸", labelCls: "strong" },
    ];

    function calcStrength(pw) {
        let score = 0;
        if (pw.length >= 8) score++;
        if (/[A-Z]/.test(pw)) score++;
        if (/[0-9]/.test(pw)) score++;
        if (/[^A-Za-z0-9]/.test(pw)) score++;
        return score;
    }

    function updateStrength(pw) {
        if (!strengthWrap) return;

        if (!pw) {
            strengthWrap.style.display = "none";
            return;
        }

        strengthWrap.style.display = "block";
        const score = Math.max(1, calcStrength(pw));

        bars.forEach((bar, i) => {
            bar.className = "strength-bar";
            if (i < score) bar.classList.add(levels[score - 1].cls);
        });

        if (strengthLbl) {
            strengthLbl.textContent = levels[score - 1].label;
            strengthLbl.className = "strength-label " + levels[score - 1].labelCls;
        }
    }

    /* ─── Confirm match indicator ──────────────────────── */
    const confirmInput = document.getElementById("confirmInput");
    const matchIndicator = document.getElementById("matchIndicator");
    const matchIcon = document.getElementById("matchIcon");
    const matchText = document.getElementById("matchText");

    const iconCheck = `<polyline points="20 6 9 17 4 12"/>`;
    const iconCross = `<line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/>`;

    function checkMatch() {
        if (!matchIndicator) return;

        const pw = passInput?.value || "";
        const cpw = confirmInput?.value || "";

        if (!cpw) {
            matchIndicator.classList.remove("show");
            return;
        }

        matchIndicator.classList.add("show");

        if (pw === cpw) {
            matchIndicator.className = "match-indicator show match";
            if (matchIcon) matchIcon.innerHTML = iconCheck;
            if (matchText) matchText.textContent = "Mật khẩu khớp";
        } else {
            matchIndicator.className = "match-indicator show no-match";
            if (matchIcon) matchIcon.innerHTML = iconCross;
            if (matchText) matchText.textContent = "Mật khẩu chưa khớp";
        }
    }

    passInput?.addEventListener("input", () => {
        updateStrength(passInput.value);
        checkMatch();
    });

    confirmInput?.addEventListener("input", checkMatch);

    /* ─── Enable submit when terms checked ─────────────── */
    const termsCheck = document.getElementById("termsCheck");
    const submitBtn = document.getElementById("submitBtn");

    function syncSubmit() {
        if (!submitBtn) return;

        const checked = termsCheck?.checked ?? true;
        submitBtn.disabled = !checked;
        submitBtn.style.opacity = checked ? "1" : "0.55";
    }

    termsCheck?.addEventListener("change", syncSubmit);
    syncSubmit();

    /* ─── Falling petals ────────────────────────────────── */
    const page = document.querySelector(".login-page");
    const colors = ["#f7b8c4", "#f093a7", "#b8e0d4", "#8ecfbe", "#f7c5d0", "#d4e8e0"];

    if (page) {
        for (let i = 0; i < 12; i++) {
            const petal = document.createElement("div");
            petal.classList.add("petal");

            const size = 6 + Math.random() * 10;
            const color = colors[Math.floor(Math.random() * colors.length)];
            const delay = Math.random() * 14;
            const dur = 10 + Math.random() * 14;

            petal.innerHTML = `
                <svg width="${size * 2}" height="${size * 3}" viewBox="0 0 ${size * 2} ${size * 3}">
                    <ellipse cx="${size}" cy="${size * 1.5}" rx="${size}" ry="${size * 1.5}" fill="${color}" opacity="0.7"/>
                </svg>
            `;

            Object.assign(petal.style, {
                left: Math.random() * 100 + "vw",
                top: "-60px",
                animationDuration: dur + "s",
                animationDelay: delay + "s",
                animationIterationCount: "infinite",
            });

            page.appendChild(petal);
        }
    }
})();