document.addEventListener("DOMContentLoaded", function () {
    const editModal = document.getElementById("editProfileModal");
    const deleteInput = document.getElementById("deleteConfirmText");
    const deleteButton = document.getElementById("btnConfirmDelete");

    if (editModal) {
        editModal.addEventListener("show.bs.modal", function (event) {
            const button = event.relatedTarget;
            if (!button) return;

            document.getElementById("editUserId").value = button.getAttribute("data-userid") || "";
            document.getElementById("editEmail").value = button.getAttribute("data-email") || "";
            document.getElementById("editFullName").value = button.getAttribute("data-fullname") || "";
            document.getElementById("editPhoneNumber").value = button.getAttribute("data-phone") || "";
        });
    }

    if (deleteInput && deleteButton) {
        const toggleDeleteButton = () => {
            const isValid = deleteInput.value.trim().toUpperCase() === "XOA TAI KHOAN";
            deleteButton.disabled = !isValid;
        };

        deleteInput.addEventListener("input", toggleDeleteButton);
        toggleDeleteButton();
    }
});
