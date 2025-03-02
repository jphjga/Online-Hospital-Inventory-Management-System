document.addEventListener("DOMContentLoaded", function () {
    const productSelect = document.getElementById("productSelect");
    const selectedProductContainer = document.getElementById("selectedProductsContainer");
    const proceedSaleBtn = document.getElementById("proceedSaleBtn");
    const hiddenProductId = document.getElementById("selectedProductId");
    const hiddenProductQuantity = document.getElementById("selectedProductQuantity");

    // When the dropdown changes, update the selected product card and hidden fields.
    productSelect.addEventListener("change", function () {
        const productId = productSelect.value;
        if (!productId) {
            // Clear if nothing is selected
            selectedProductContainer.innerHTML = "";
            hiddenProductId.value = "0";
            hiddenProductQuantity.value = "0";
            proceedSaleBtn.style.display = "none";
            return;
        }

        // Get data from the selected option
        const selectedOption = productSelect.options[productSelect.selectedIndex];
        const productName = selectedOption.getAttribute("data-name");
        const productPrice = selectedOption.getAttribute("data-price");
        const productStock = selectedOption.getAttribute("data-quantity");

        // Create a product card if it doesn't already exist
        if (!document.getElementById(`product-card-${productId}`)) {
            const card = document.createElement("div");
            card.classList.add("sale-product-card");
            card.id = `product-card-${productId}`;
            card.innerHTML = `
                <h3>${productName}</h3>
                <p>Price: $${productPrice}</p>
                <p>Available: ${productStock}</p>
                <label>Quantity to sell:
                  <input type="number" id="qty-${productId}" class="form-control" value="1" min="1" max="${productStock}" />
                </label>
            `;
            selectedProductContainer.innerHTML = ""; // Only allow one product for this example.
            selectedProductContainer.appendChild(card);
        }
        // Update hidden fields with selected product data
        hiddenProductId.value = productId;
        hiddenProductQuantity.value = document.getElementById("qty-" + productId).value;
        proceedSaleBtn.style.display = "block";

        // Update hidden quantity when input changes
        document.getElementById("qty-" + productId).addEventListener("change", function () {
            hiddenProductQuantity.value = this.value;
        });
    });
});
