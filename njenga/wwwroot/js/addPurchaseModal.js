document.addEventListener("DOMContentLoaded", function () {
    var modal = document.getElementById("addPurchaseModal");
    var openModalButton = document.getElementById("openModalButton");
    var closeModalSpan = document.querySelector("#addPurchaseModal .close");
    var cancelModalButton = document.getElementById("cancelModalButton");
    var addPurchaseForm = document.getElementById("addPurchaseForm");
    

   // Open modal when the add button is clicked
    openModalButton.addEventListener("click", function () {
        modal.style.display = "block";

        // 🔹 Fade-in effect on modal load
        modal.style.opacity = 0;
        modal.style.transition = "opacity 0.3s ease-in-out";
        setTimeout(() => {
            modal.style.opacity = 1;
        }, 100);
    });

    // Close modal when the 'x' is clicked
    closeModalSpan.addEventListener("click", function () {
        modal.style.display = "none";
        addPurchaseForm.reset();
    });

    // Close modal when the Cancel button is clicked
    cancelModalButton.addEventListener("click", function () {
        modal.style.display = "none";
        addPurchaseForm.reset();
    });

    // Close modal if the user clicks outside the modal content
    window.addEventListener("click", function (event) {
        if (event.target === modal) {
            modal.style.display = "none";
            addPurchaseForm.reset();
        }
    });
});
