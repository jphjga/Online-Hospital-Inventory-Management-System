document.addEventListener("DOMContentLoaded", function () {
    // Global functions to open/close the Add Product modal

    window.openAddProductModal = function () {
        const modal = document.getElementById("addProductModal");
        if (modal) {
            modal.classList.add("show");
            console.log("Add Product modal opened.");
        } else {
            console.warn("Modal with id 'addProductModal' not found.");
        }
    };

    window.closeAddProductModal = function () {
        const modal = document.getElementById("addProductModal");
        if (modal) {
            modal.classList.remove("show");
            console.log("Add Product modal closed.");
        }
    };

    // Optional: Close modal when clicking outside modal-content
    window.addEventListener("click", function (event) {
        const modal = document.getElementById("addProductModal");
        if (modal && event.target === modal) {
            modal.classList.remove("show");
        }
    });
});
