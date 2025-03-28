document.addEventListener("DOMContentLoaded", function () {
    const urlParams = new URLSearchParams(window.location.search);

    // Define default active tabs for left and right columns
    const defaultTabs = {
        left: "products",
        right: "stock-alert"
    };

    function triggerAddProductModal() {
        const modal = document.getElementById("addProductModal");
        if (modal) {
            modal.classList.add("show");
            console.log("Add Product modal triggered.");
        } else {
            console.warn("Modal with id 'addProductModal' not found.");
        }
    }

    // Use the new function based on URL parameters
    if (urlParams.has("editId") || urlParams.get("action") === "add") {
        triggerAddProductModal();
    } else {
        activateTab(defaultTabs.left, "left");
        activateTab(defaultTabs.right, "right");
    }

    // Attach click event listeners to all tab links
    document.querySelectorAll(".tab-link").forEach(link => {
        link.addEventListener("click", function (e) {
            e.preventDefault();
            const column = this.dataset.column; // Determine column (left/right)
            activateTab(this.dataset.tab, column);
        });
    });

    document.addEventListener("DOMContentLoaded", function () {
        document.getElementById("purchases-content").addEventListener("click", function (e) {
            if (e.target.classList.contains("btn-remove")) {
                let productId = e.target.getAttribute("data-id");

                fetch(`?handler=RemovePurchase&productId=${productId}`, { method: "POST" })
                    .then(() => loadPurchases());
            }
        });
    });

    function activateTab(tabId, column) {
        if (!tabId || !column) return;

        // Remove active class from all tab contents within the same column
        document.querySelectorAll(`.tab-content[data-column="${column}"]`).forEach(tab => {
            tab.classList.remove("active");
        });

        // Remove active class from all tab links within the same column
        document.querySelectorAll(`.tab-link[data-column="${column}"]`).forEach(link => {
            link.classList.remove("active");
        });

        // Activate the selected tab content
        const selectedTabContent = document.getElementById(tabId);
        if (selectedTabContent) {
            selectedTabContent.classList.add("active");
        }

        // Activate the selected tab link
        const selectedTabLink = document.querySelector(`.tab-link[data-tab="${tabId}"]`);
        if (selectedTabLink) {
            selectedTabLink.classList.add("active");
        }
    }


    // Auto-switch to Expiry Calendar if clicked from a notification
    document.querySelector(".alert-warning a")?.addEventListener("click", function (e) {
        e.preventDefault();
        activateTab("expiry-calendar", "left");
    });


    //action button iside products table   
        window.toggleActionDropdown = function (icon) {
            const dropdown = icon.parentElement.querySelector('.action-menu');
            if (dropdown) {
                // Toggle display property
                dropdown.style.display = dropdown.style.display === "block" ? "none" : "block";
            } else {
                console.warn("No action menu found.");
            }
    };
    window.confirmDelete = function () {
        return confirm("warning! This product is still in stock. If the product have expired, navigate to stock taking page to remove it.");
    }
});