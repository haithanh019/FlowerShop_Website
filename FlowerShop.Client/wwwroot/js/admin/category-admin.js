// ── Search table ──────────────────────────────────────────────────────────────
document.getElementById('categorySearch')?.addEventListener('input', function () {
    const q = this.value.toLowerCase();
    document.querySelectorAll('#categoryTable tbody tr').forEach(row => {
        row.style.display = row.textContent.toLowerCase().includes(q) ? '' : 'none';
    });
});

// ── Edit modal ────────────────────────────────────────────────────────────────
function openEditModal(id, name, desc) {
    document.getElementById('editName').value = name;
    document.getElementById('editDesc').value = desc;
    document.getElementById('formEdit').action = `/Admin/Category/Edit/${id}`;
    openModal('modalEdit');
}

// ── Delete modal ──────────────────────────────────────────────────────────────
function openDeleteModal(id, name) {
    document.getElementById('deleteNameDisplay').textContent = name;
    document.getElementById('formDelete').action = `/Admin/Category/Delete/${id}`;
    openModal('modalDelete');
}
