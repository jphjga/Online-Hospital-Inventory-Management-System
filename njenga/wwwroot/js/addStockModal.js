document.addEventListener("DOMContentLoaded", function () {
    var productSelect = document.getElementById("productSelect");

    // Populate form fields when a product is selected
    productSelect.addEventListener("change", function () {
        var selectedOption = this.options[this.selectedIndex];

        if (!selectedOption.value) return;

        document.getElementById("productId").value = selectedOption.value;
        document.getElementById("institutionId").value = selectedOption.getAttribute("data-institutionid");
        document.getElementById("productName").value = selectedOption.text;
        document.getElementById("productDescription").value = selectedOption.getAttribute("data-description");
        document.getElementById("productPrice").value = selectedOption.getAttribute("data-price");
        document.getElementById("productQuantity").value = selectedOption.getAttribute("data-quantity");
        document.getElementById("productExpiry").value = selectedOption.getAttribute("data-expiry");
        document.getElementById("productBarcode").value = selectedOption.getAttribute("data-barcode");
        document.getElementById("productQuantityAlert").value = selectedOption.getAttribute("data-quantityalert");
        document.getElementById("productCategory").value = selectedOption.getAttribute("data-category");
    });

    // Handle "Proceed" Button
    document.getElementById("proceedStockTaking").addEventListener("click", function () {
        var productData = {
            Product_id: document.getElementById("productId").value,
            InstitutionId: document.getElementById("institutionId").value,
            Name: document.getElementById("productName").value,
            Description: document.getElementById("productDescription").value,
            Price: document.getElementById("productPrice").value,
            Quantity: document.getElementById("productQuantity").value,
            Expiry: document.getElementById("productExpiry").value,
            Barcode: document.getElementById("productBarcode").value,
            Quantity_alert: document.getElementById("productQuantityAlert").value,
            Category: document.getElementById("productCategory").value
        };

        // Retrieve existing stock-taking data from session
        var stockTakingData = JSON.parse(sessionStorage.getItem("StockTakingProducts")) || [];

        // Add new product if it doesn't already exist
        var existing = stockTakingData.find(p => p.Product_id === productData.Product_id);
        if (!existing) {
            stockTakingData.push(productData);
        }

        // Save updated data to session storage
        sessionStorage.setItem("StockTakingProducts", JSON.stringify(stockTakingData));

        // Update table
        updateStockTakingTable();

        // Close the modal
        document.getElementById("productDetailsModal").style.display = "none";
    });

    // Function to update Stock Taking table
    function updateStockTakingTable() {
        let tableBody = document.getElementById("stockTakingTableBody");
        tableBody.innerHTML = ""; // Clear existing table rows

        let stockTakingProducts = JSON.parse(sessionStorage.getItem("StockTakingProducts") || "[]");

        stockTakingProducts.forEach((product, index) => {
            let row = document.createElement("tr");

            row.innerHTML = `
            <td>${index + 1}</td>
            <td>${product.Name}</td>
            <td>${product.Description}</td>
            <td>${product.Price}</td>
            <td>
                <input type="number" name="StockTakingProducts[${index}].Quantity" value="${product.Quantity}" min="0" required>
            </td>
            <td>${product.Expiry}</td>
            <td>${product.Barcode}</td>
            <td>${product.Quantity_alert}</td>
            <td>${product.Category}</td>
            <td>
                <input type="hidden" name="StockTakingProducts[${index}].Product_id" value="${product.Product_id}">
                <button type="button" class="btn btn-danger remove-product" data-id="${product.Product_id}">Remove</button>
            </td>
        `;

            tableBody.appendChild(row);
        });
  


        document.getElementById("saveStockBtn").addEventListener("click", function (event) {
            event.preventDefault(); // Prevent page refresh

            let form = document.getElementById("stockTakingForm");
            let tableBody = document.getElementById("stockTakingTableBody");

            // Remove existing hidden inputs to avoid duplicates
            document.querySelectorAll(".hidden-stock-data").forEach(e => e.remove());

            let rows = tableBody.querySelectorAll("tr");

            rows.forEach(row => {
                let productId = row.dataset.productId;
                let quantityInput = row.querySelector(".product-quantity");

                if (quantityInput) { // Ensure quantity field exists
                    let quantity = quantityInput.value;

                    let inputProductId = document.createElement("input");
                    inputProductId.type = "hidden";
                    inputProductId.name = `StockTakingProducts[${productId}].Product_id`;
                    inputProductId.value = productId;
                    inputProductId.classList.add("hidden-stock-data");

                    let inputQuantity = document.createElement("input");
                    inputQuantity.type = "hidden";
                    inputQuantity.name = `StockTakingProducts[${productId}].Quantity`;
                    inputQuantity.value = quantity;
                    inputQuantity.classList.add("hidden-stock-data");

                    form.appendChild(inputProductId);
                    form.appendChild(inputQuantity);
                }
            });

            form.submit(); // Submit form
        });



        // Add event listener to remove buttons
        document.querySelectorAll(".remove-product").forEach(button => {
            button.addEventListener("click", function () {
                var productId = this.getAttribute("data-id");
                removeProductFromStockTaking(productId);
            });
        });
    }
    // Handle Modal Open
    document.getElementById("openProductModal").addEventListener("click", function () {
        document.getElementById("productDetailsModal").style.display = "block";
    });

    // Handle Modal Close
    document.getElementById("closeProductModal").addEventListener("click", function () {
        document.getElementById("productDetailsModal").style.display = "none";
    });

    // Handle Proceed Button (For Future Session Storage Feature)
    document.getElementById("proceedStockTaking").addEventListener("click", function () {
        alert("Product added to Stock Taking!");
        document.getElementById("productDetailsModal").style.display = "none";
    });

    document.getElementById("clearStockTaking").addEventListener("click", function () {
        sessionStorage.removeItem("StockTakingProducts");
        updateStockTakingTable(); // Refresh table
    });

    // Function to remove a product from session storage
    function removeProductFromStockTaking(productId) {
        var stockTakingData = JSON.parse(sessionStorage.getItem("StockTakingProducts")) || [];
        stockTakingData = stockTakingData.filter(p => p.Product_id !== productId);
        sessionStorage.setItem("StockTakingProducts", JSON.stringify(stockTakingData));
        updateStockTakingTable(); // Refresh table
    }

    // Load session data on page load
    updateStockTakingTable();
});
