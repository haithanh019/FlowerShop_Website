document.getElementById('flowerSearch')?.addEventListener('input', function () {
    const q = this.value.toLowerCase();
    document.querySelectorAll('#flowerTable tbody tr').forEach(row => {
        row.style.display = row.textContent.toLowerCase().includes(q) ? '' : 'none';
    });
});

function previewCreateImages(input) {
    const preview = document.getElementById('createImagePreview');
    preview.innerHTML = '';
    if (input.files.length > 5) {
        alert('Chỉ được chọn tối đa 5 ảnh.');
        input.value = '';
        return;
    }
    [...input.files].forEach(file => {
        const url = URL.createObjectURL(file);
        preview.innerHTML += `
            <img src="${url}" alt="${file.name}"
                 style="width:64px;height:64px;object-fit:cover;
                        border-radius:8px;border:1px solid #eee"
                 title="${file.name}" />`;
    });
}

function previewEditImages(input) {
    const preview = document.getElementById('editImagePreview');
    preview.innerHTML = '';
    [...input.files].forEach(file => {
        const url = URL.createObjectURL(file);
        preview.innerHTML += `
            <img src="${url}" alt="${file.name}"
                 style="width:64px;height:64px;object-fit:cover;
                        border-radius:8px;border:1px solid #eee"
                 title="${file.name}" />`;
    });
}

function toggleDeleteOverlay(checkbox) {
    const label = checkbox.closest('label');
    const img = label.querySelector('img');
    const overlay = label.querySelector('.delete-overlay');
    if (checkbox.checked) {
        img.style.opacity = '0.4';
        img.style.border = '2px solid #d66b6b';
        overlay.style.display = 'flex';
    } else {
        img.style.opacity = '1';
        img.style.border = '2px solid #eee';
        overlay.style.display = 'none';
    }
}

function openFlowerEditModal(id, name, desc, price, stock, isActive, catId, images) {
    document.getElementById('eFlowerName').value = name;
    document.getElementById('eDesc').value = desc;
    document.getElementById('ePrice').value = price;
    document.getElementById('eStock').value = stock;
    document.getElementById('eIsActive').checked = isActive;
    document.getElementById('eCategoryID').value = catId;
    document.getElementById('formFlowerEdit').action = `/Admin/Flower/Edit/${id}`;

    document.getElementById('editFileInput').value = '';
    document.getElementById('editImagePreview').innerHTML = '';

    document.querySelectorAll('#formFlowerEdit input[name="DeleteImageIds"]')
        .forEach(el => el.remove());

    const container = document.getElementById('editCurrentImages');
    container.innerHTML = '';

    if (!images || !images.length) {
        container.innerHTML = '<span style="color:#aaa;font-size:.85rem">Chưa có ảnh</span>';
    } else {
        images.forEach(img => {
            const canDelete = img.publicID && img.publicID.trim() !== '';

            const label = document.createElement('label');
            label.style.cssText = `position:relative;display:inline-block;
                cursor:${canDelete ? 'pointer' : 'default'}`;
            label.title = canDelete ? 'Tích để xóa' : 'Không thể xóa ảnh này';

            if (canDelete) {
                const cb = document.createElement('input');
                cb.type = 'checkbox';
                cb.name = 'DeleteImageIds';
                cb.value = img.publicID;
                cb.style.display = 'none';
                cb.addEventListener('change', () => toggleDeleteOverlay(cb));
                label.appendChild(cb);
            }

            const imgEl = document.createElement('img');
            imgEl.src = img.url;
            imgEl.alt = 'ảnh';
            imgEl.style.cssText = `width:72px;height:72px;object-fit:cover;
                border-radius:10px;border:2px solid #eee;display:block`;
            label.appendChild(imgEl);

            const overlay = document.createElement('div');
            overlay.className = 'delete-overlay';
            overlay.textContent = '✕';
            overlay.style.cssText = `display:none;position:absolute;inset:0;
                border-radius:10px;background:rgba(214,107,107,.6);
                color:#fff;font-size:1.4rem;
                align-items:center;justify-content:center`;
            label.appendChild(overlay);

            if (!canDelete) {
                const badge = document.createElement('div');
                badge.textContent = 'no ID';
                badge.style.cssText = `position:absolute;bottom:2px;left:0;right:0;
                    text-align:center;font-size:.6rem;color:#aaa`;
                label.appendChild(badge);
            }

            container.appendChild(label);
        });
    }

    openModal('modalEdit');
}

function openFlowerDeleteModal(id, name) {
    document.getElementById('flowerDeleteName').textContent = name;
    document.getElementById('formFlowerDelete').action = `/Admin/Flower/Delete/${id}`;
    openModal('modalDelete');
}
